using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TelereportPoint : MonoBehaviour, IInteractable
{
    public SceneLoadEventSO loadEventSO;
    public Vector3 positionToGo;
    public GameSceneSO sceneToGo;
    public void TriggerAction()
    {
        Debug.Log("chuansong");
        loadEventSO.RaiseLoadRequestEvent(sceneToGo, positionToGo, true);

    }
}
