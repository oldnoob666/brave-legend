using System;
using System.IO;
using System.Reflection;
using Cinemachine;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

// Request with Temp/SceneTransitionCheck.request; exercises the actual play-mode path.
[InitializeOnLoad]
public static class SceneTransitionCheck
{
    private const string Request = "Temp/SceneTransitionCheck.request";
    private const string Result = "Logs/SceneTransitionCheck.txt";
    private static int phase;
    private static double deadline;
    private static Vector3 destination;
    private static GameSceneSO target;

    static SceneTransitionCheck() { EditorApplication.update += Tick; }

    private static void Tick()
    {
        try
        {
            if (!File.Exists(Request) || EditorApplication.isCompiling) return;
            if (!EditorApplication.isPlaying)
            {
                if (!EditorApplication.isPlayingOrWillChangePlaymode)
                    EditorApplication.isPlaying = true;
                return;
            }
            if (phase == 0) { deadline = EditorApplication.timeSinceStartup + 40; phase = 1; }
            if (EditorApplication.timeSinceStartup > deadline) throw new Exception("Timed out in phase " + phase);
            var loader = UnityEngine.Object.FindObjectOfType<SceneLoader>();
            var player = UnityEngine.Object.FindObjectOfType<Player>();
            if (loader == null || player == null) return;
            if (phase == 1)
            {
                var portal = UnityEngine.Object.FindObjectOfType<TelereportPoint>();
                if (portal == null || !SceneManager.GetSceneByName("Forest").isLoaded) return;
                var busy = typeof(SceneLoader).GetField("isLoading", BindingFlags.NonPublic | BindingFlags.Instance);
                if (busy != null && (bool)busy.GetValue(loader)) return;
                // Keep physics from obscuring the immediate placement assertion.
                player.rb.simulated = false;
                player.transform.position = portal.transform.position;
                destination = portal.positionToGo;
                target = portal.sceneToGo;
                portal.TriggerAction();
                phase = 2;
                return;
            }
            if (phase == 2)
            {
                if (!SceneManager.GetSceneByName("Desert").isLoaded) return;
                phase = 3;
                deadline = EditorApplication.timeSinceStartup + 2;
                return;
            }
            if (phase == 3)
            {
                var current = typeof(SceneLoader).GetField("currentLoadedScene", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(loader);
                var confiner = UnityEngine.Object.FindObjectOfType<CinemachineConfiner2D>();
                string report = "Desert loaded: True\nPlayer position: " + player.transform.position + " expected: " + destination
                    + "\nPlayer at destination: " + (Vector3.Distance(player.transform.position, destination) < 0.1f)
                    + "\nCurrent scene updated: " + (current == target)
                    + "\nDesert camera bounds: " + (confiner != null && confiner.m_BoundingShape2D != null && confiner.m_BoundingShape2D.gameObject.scene.name == "Desert");
                Finish(report);
            }
        }
        catch (Exception ex) { Finish("ERROR: " + ex); }
    }

    private static void Finish(string result)
    {
        File.WriteAllText(Result, result);
        File.Delete(Request);
        EditorApplication.isPlaying = false;
        phase = 0;
    }
}
