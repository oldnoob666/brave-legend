using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;//.Addressable的命名空间
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;//unity场景加载的命名空间：LoadSceneMode
public class SceneLoader : MonoBehaviour
{
    public Transform playerTrans;
    [Header("事件监听")]
    public GameSceneSO firstLoadScene;//获取so中第一个场景的内容
    public SceneLoadEventSO loadEventSO;//获取so中场景加载事件的内容


    private GameSceneSO currentLoadedScene;//当前场景的内容
    private GameSceneSO sceneToLoad;//要加载的场景的内容
    private Vector3 positionToGo;
    private bool fadeScreen;
    public float fadeDuration;

    private void Awake()
    {
        currentLoadedScene = firstLoadScene;
        currentLoadedScene.sceneReference.LoadSceneAsync(LoadSceneMode.Additive);//LoadSceneAsync使用异步加载场景的方法，（加载内容，加载模式（Additive））
    }

    private void OnEnable()
    {
        loadEventSO.LoadRequestEvent += OnLoadRequestEvent;
    }


    private void OnDisable()
    {
        loadEventSO.LoadRequestEvent -= OnLoadRequestEvent;
    }

    private void OnLoadRequestEvent(GameSceneSO locationToLoad, Vector3 posToGo, bool fadeScreen)//
    {
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
        LoadNewScene();
    }

    private void LoadNewScene()
    {
       var loadingOption = sceneToLoad.sceneReference.LoadSceneAsync(LoadSceneMode.Additive,true);
        loadingOption.Completed += OnLoadCompleted;
    }

    private void OnLoadCompleted(AsyncOperationHandle<SceneInstance> handle)
    {
        currentLoadedScene = sceneToLoad;

        playerTrans.position = positionToGo;
        if (fadeScreen)
        {
            //TODO:
        }
    }
}
