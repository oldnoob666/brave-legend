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
    public SceneLoadEventSO loadEventSO;//接收玩家按e键触发的事件，监听到事件后，调用OnLoadRequestEvent方法


    [Header("广播")]
    public VoidEventSO afterSceneLoadEvent;
    public FadeEventSO fadeEventSO;

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
        loadEventSO.LoadRequestEvent += OnLoadRequestEvent;//在这里订阅SceneLoadEventSO的事件，当事件触发时调用OnLoadRequestEvent方法
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
            StartCoroutine(UnLoadPreviousScene());//如果当前加载的场景不为空，就先卸载旧场景，再加载新场景
        }
        else
        {
            LoadNewScene();//如果当前加载的场景为空，就直接加载新场景
        }



    }

    private void NewGame()
    {
        sceneToLoad = firstLoadScene;//把第一个场景赋值给sceneToLoad
        OnLoadRequestEvent(sceneToLoad, firstplayerTrans, true);//直接触发OnLoadRequestEvent，游戏开始时加载第一个场景，并把玩家位置设置为firstplayerTrans
    }




    private IEnumerator UnLoadPreviousScene()//协程方法，卸载旧场景,在OnLoadRequestEvent方法里被调用
    {
        if(fadeScreen)//卸载场景时的变黑效果
        {
            fadeEventSO.FadeIn(fadeDuration);//调用FadeEventSO的FadeIn方法，传入fadeDuration参数
        }

        yield return new WaitForSeconds(fadeDuration);//等待fadeDuration秒后再执行下一步操作

        if (currentLoadedScene != null)
          yield return  currentLoadedScene.sceneReference.UnLoadScene();//等待旧场景卸载完成后再加载新场景

        playerTrans.gameObject.SetActive(false);//卸载旧场景后，先把玩家隐藏起来，等新场景加载完成后再显示出来

        LoadNewScene();//加载新场景
    }

    private void LoadNewScene()
    {
       var loadingOption = sceneToLoad.sceneReference.LoadSceneAsync(LoadSceneMode.Additive,true);//加载新场景，LoadSceneMode.Additive表示加载新场景时不卸载旧场景，true表示加载完成后激活新场景
        loadingOption.Completed += OnLoadCompleted;//加载新场景完成后，执行的回调函数
    }

    private void OnLoadCompleted(AsyncOperationHandle<SceneInstance> handle)//加载新场景完成后，执行的回调函数,在上面的方法里被调用
    {
        currentLoadedScene = sceneToLoad;//把当前加载的场景赋值给currentLoadedScene

        playerTrans.position = positionToGo;//把玩家的位置设置为传过来的位置

        playerTrans.gameObject.SetActive(true);//把玩家显示出来
        if (fadeScreen)//
        {
            fadeEventSO.FadeOut(fadeDuration);
        }
        isLoading = false;//把isLoading设置为false，表示场景加载完成，可以再次按e键切换场景
        afterSceneLoadEvent.RaiseEvent();//广播场景加载完成事件
    }
}
