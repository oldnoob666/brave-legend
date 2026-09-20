using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TelereportPoint : MonoBehaviour, IInteractable
{
    public SceneLoadEventSO loadEventSO;
    public Vector3 positionToGo;
    public GameSceneSO sceneToGo;
    public void TriggerAction()//当玩家按e键触发事件时，调用这个方法，这个方法在sign.cs中被调用
    {
        Debug.Log("chuansong");
        loadEventSO.RaiseLoadRequestEvent(sceneToGo, positionToGo, true);//调用SceneLoadEventSO中的RaiseLoadRequestEvent方法，传入要加载的场景，加载后要去的位置，是否淡入淡出

    }
}
