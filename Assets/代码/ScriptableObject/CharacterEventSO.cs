using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event/CharacterEventSO")]
public class CharacterEventSO : ScriptableObject//创建一个继承自ScriptableObject的类CharacterEventSO，并在Unity编辑器中创建一个菜单项"Event/CharacterEventSO"来创建这个脚本对象
{
    public UnityAction<character> OnEventRised;// 新建一个接收并传递character的事件

    public void EventRised(character character)//当事件触发时，调用这个方法，传入character参数
    {
        OnEventRised?.Invoke(character);//触发事件，并传入character参数
    }

}
