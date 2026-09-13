using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(menuName ="Game Scene/GameSceneSO")]
public class GameSceneSO : ScriptableObject
{
    public SceneType SceneType; 
    public AssetReference sceneReference;//资源引用，用于引用addressable打包好的场景
}