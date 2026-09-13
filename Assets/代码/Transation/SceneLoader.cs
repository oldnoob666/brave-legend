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
    public Vector3 firstplayerTrans;
    [Header("事件监听")]
    public GameSceneSO firstLoadScene;//获取so中第一个场景的内容
    public SceneLoadEventSO loadEventSO;//获取so中场景加载事件的内容


    [Header("广播")]
    public VoidEventSO afterSceneLoadEvent;

    private GameSceneSO currentLoadedScene;//当前场景的内容
    private GameSceneSO sceneToLoad;//要加载的场景的内容
    private Vector3 positionToGo;
    private bool fadeScreen;
    public float fadeDuration;
    private bool isLoading;

    private void Awake()
    {
    //    currentLoadedScene = firstLoadScene;
    //    currentLoadedScene.sceneReference.LoadSceneAsync(LoadSceneMode.Additive);//LoadSceneAsync使用异步加载场景的方法，（加载内容，加载模式（Additive）），负责游戏开始时加载第一个场景
    }

    private void OnEnable()
    {
        loadEventSO.LoadRequestEvent += OnLoadRequestEvent;
    }


    private void OnDisable()
    {
        loadEventSO.LoadRequestEvent -= OnLoadRequestEvent;
    }


    private void Start()
    {
        NewGame();
    }

    private void OnLoadRequestEvent(GameSceneSO locationToLoad, Vector3 posToGo, bool fadeScreen)//这个是玩家按e键之后要触发的事件
    {
        if(isLoading)
            return;//防止玩家一直按e切换场景
        isLoading = true;
        sceneToLoad = locationToLoad;
        positionToGo = posToGo;
        this.fadeScreen = fadeScreen;//监听到事件后，把传过来的参数赋值给本地变量

        if(currentLoadedScene != null )
        {
            StartCoroutine(UnLoadPreviousScene());
        }
        else
        {
            LoadNewScene();
        }



    }

    private void NewGame()
    {
        sceneToLoad = firstLoadScene;
        OnLoadRequestEvent(sceneToLoad, firstplayerTrans, true);
    }




    private IEnumerator UnLoadPreviousScene()
    {
        if(fadeScreen)
        {

        }

        yield return new WaitForSeconds(fadeDuration);

        if(currentLoadedScene != null)
          yield return  currentLoadedScene.sceneReference.UnLoadScene();//等待旧场景卸载完成后再加载新场景

        playerTrans.gameObject.SetActive(false);//卸载旧场景后，先把玩家隐藏起来，等新场景加载完成后再显示出来

        LoadNewScene();//加载新场景
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

        playerTrans.gameObject.SetActive(true);
        if (fadeScreen)
        {
            //TODO:
        }
        isLoading = false;
        afterSceneLoadEvent.RaiseEvent();//广播场景加载完成事件
    }
}
