using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;//.Addressable的命名空间
using UnityEngine.SceneManagement;//unity场景加载的命名空间：LoadSceneMode
public class SceneLoader : MonoBehaviour
{
    [Header("事件监听")]
    public GameSceneSO firstLoadScene;//获取so中第一个场景的内容，变量是GameSceneSO，所以可以获取这个里面的内容
    public SceneLoadEventSO loadEventSO;//获取so中场景加载事件的内容，变量是LoadEventSO，所以可以获取这个里面的内容


    private GameSceneSO currentLoadedScene;//当前场景的内容
    private GameSceneSO sceneToLoad;
    private Vector3 positionToGo;
    private bool fadeScreen;
    private bool isLoading;
    public float fadeDuration;

    private void Awake()
    {
        isLoading = true;
        currentLoadedScene = firstLoadScene;
        currentLoadedScene.sceneReference.LoadSceneAsync(LoadSceneMode.Additive).Completed += handle =>
        {
            isLoading = false;
            if (handle.Status == AsyncOperationStatus.Succeeded)
                RefreshCameraBounds(handle.Result.Scene);
            else
                Debug.LogError("Failed to load initial scene: " + handle.OperationException, this);
        };//LoadSceneAsync使用异步加载场景的方法，（加载内容，加载模式（Additive））
    }

    private void OnEnable()
    {
        loadEventSO.LoadRequestEvent += OnLoadRequestEvent;
    }


    private void OnDisable()
    {
        loadEventSO.LoadRequestEvent -= OnLoadRequestEvent;
    }

    private void OnLoadRequestEvent(GameSceneSO locationToLoad, Vector3 posToGo, bool fadeScreen)
    {
        if (isLoading || locationToLoad == currentLoadedScene)
            return;
        if (locationToLoad == null || !locationToLoad.sceneReference.RuntimeKeyIsValid())
        {
            Debug.LogError("Scene load request has no valid target scene.", this);
            return;
        }

        isLoading = true;
        sceneToLoad = locationToLoad;
        positionToGo = posToGo;
        this.fadeScreen = fadeScreen;//监听到事件后，把传过来的参数赋值给本地变量

        StartCoroutine(UnLoadPreviousScene());

    }


    private IEnumerator UnLoadPreviousScene()
    {
        if(fadeScreen)
        {

        }

        yield return new WaitForSeconds(fadeDuration);

        if(currentLoadedScene != null)
          yield return  currentLoadedScene.sceneReference.UnLoadScene();
        yield return LoadNewScene();
    }

    private IEnumerator LoadNewScene()
    {
        var handle = sceneToLoad.sceneReference.LoadSceneAsync(LoadSceneMode.Additive, true);
        yield return handle;
        if (handle.Status != AsyncOperationStatus.Succeeded)
        {
            isLoading = false;
            Debug.LogError("Failed to load destination scene: " + handle.OperationException, this);
            yield break;
        }

        currentLoadedScene = sceneToLoad;
        var player = FindObjectOfType<Player>();
        if (player != null)
        {
            player.transform.position = positionToGo;
            if (player.rb != null)
            {
                player.rb.position = positionToGo;
                player.rb.velocity = Vector2.zero;
                player.rb.angularVelocity = 0f;
            }
        }
        RefreshCameraBounds(handle.Result.Scene);
        isLoading = false;
    }

    private void RefreshCameraBounds(Scene scene)
    {
        foreach (var cameraControl in FindObjectsOfType<CameraCountrol>())
            cameraControl.RefreshForScene(scene);
    }


}
