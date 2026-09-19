using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event/FadeEventSO")] 
public class FadeEventSO : ScriptableObject
{

    public UnityAction<Color,float,bool> OnEventRised;// 新建一个接收并传递Color,float,bool的事件   


    public void FadeIn (float duration)//变黑
    {
        RiseEvent(Color.black,duration,true);   
    }

    public void FadeOut (float duration)//变透明
    {
        RiseEvent(Color.clear,duration,false);
    }

    public void RiseEvent(Color target, float duration,bool fadeIn)//当事件触发时，调用这个方法，传入Color,float,bool参数
    {
        OnEventRised?.Invoke(target,duration,fadeIn);//触发事件，并传入Color,float,bool参数
    }
}
