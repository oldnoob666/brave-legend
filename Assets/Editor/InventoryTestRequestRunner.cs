using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

[InitializeOnLoad]
public static class InventoryTestRequestRunner
{
    private static TestRunnerApi runner;
    private static ResultCallbacks callbacks;

    static InventoryTestRequestRunner()
    {
        EditorApplication.delayCall += RunRequestedTests;
    }

    [MenuItem("Tools/Inventory/Run Core Tests")]
    public static void RequestFromMenu()
    {
        File.WriteAllText(RequestPath, "run");
        RunRequestedTests();
    }

    private static string ProjectRoot => Directory.GetParent(Application.dataPath).FullName;
    private static string RequestPath => Path.Combine(ProjectRoot, "Temp", "InventoryTests.request");
    private static string RunningPath => Path.Combine(ProjectRoot, "Temp", "InventoryTests.running");
    private static string ResultPath => Path.Combine(ProjectRoot, "Logs", "InventoryCoreTests-result.txt");

    private static void RunRequestedTests()
    {
        if (!File.Exists(RequestPath) || EditorApplication.isCompiling || EditorApplication.isPlayingOrWillChangePlaymode)
        {
            return;
        }

        Directory.CreateDirectory(Path.GetDirectoryName(ResultPath));
        if (File.Exists(RunningPath))
        {
            File.Delete(RunningPath);
        }

        File.Move(RequestPath, RunningPath);
        runner = ScriptableObject.CreateInstance<TestRunnerApi>();
        callbacks = new ResultCallbacks(ResultPath, RunningPath, CleanupRunner);
        runner.RegisterCallbacks(callbacks);

        var filter = new Filter
        {
            testMode = TestMode.EditMode,
            assemblyNames = new[] { "Inventory.EditModeTests" }
        };

        runner.Execute(new ExecutionSettings(filter) { runSynchronously = true });
    }

    private static void CleanupRunner()
    {
        if (runner != null && callbacks != null)
        {
            runner.UnregisterCallbacks(callbacks);
            Object.DestroyImmediate(runner);
        }

        runner = null;
        callbacks = null;
    }

    private sealed class ResultCallbacks : ICallbacks
    {
        private readonly string resultPath;
        private readonly string runningPath;
        private readonly System.Action onFinished;

        public ResultCallbacks(string resultPath, string runningPath, System.Action onFinished)
        {
            this.resultPath = resultPath;
            this.runningPath = runningPath;
            this.onFinished = onFinished;
        }

        public void RunStarted(ITestAdaptor testsToRun) { }
        public void TestStarted(ITestAdaptor test) { }
        public void TestFinished(ITestResultAdaptor result) { }

        public void RunFinished(ITestResultAdaptor result)
        {
            var output = new StringBuilder();
            output.AppendLine($"ResultState: {result.ResultState}");
            output.AppendLine($"PassCount: {result.PassCount}");
            output.AppendLine($"FailCount: {result.FailCount}");
            output.AppendLine($"SkipCount: {result.SkipCount}");
            AppendFailures(result, output);
            File.WriteAllText(resultPath, output.ToString());

            if (File.Exists(runningPath))
            {
                File.Delete(runningPath);
            }

            onFinished();
            if (Application.isBatchMode)
            {
                EditorApplication.Exit(result.FailCount == 0 ? 0 : 1);
            }
        }

        private static void AppendFailures(ITestResultAdaptor result, StringBuilder output)
        {
            if (result.ResultState.StartsWith("Failed") && !result.HasChildren)
            {
                output.AppendLine($"Failure: {result.FullName}");
                output.AppendLine(result.Message);
                output.AppendLine(result.StackTrace);
            }

            if (!result.HasChildren)
            {
                return;
            }

            foreach (var child in result.Children)
            {
                AppendFailures(child, output);
            }
        }
    }
}
