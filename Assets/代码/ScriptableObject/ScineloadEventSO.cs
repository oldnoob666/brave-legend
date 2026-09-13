using UnityEngine.Events;
using UnityEngine;

[CreateAssetMenu(menuName ="Event/SceneLoadEventSO")]
public class SceneLoadEventSO:ScriptableObject
{
    public UnityAction<GameSceneSO, Vector3, bool> LoadRequestEvent;


    public void RaiseLoadRequestEvent(GameSceneSO locationToLoad, Vector3 posToGo, bool fadeScreen)//要加载的场景，加载后要去的位置，是否淡入淡出
    {
        LoadRequestEvent?.Invoke(locationToLoad, posToGo, fadeScreen);
    }
}


