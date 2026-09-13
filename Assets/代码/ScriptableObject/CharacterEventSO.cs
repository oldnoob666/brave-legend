using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event/CharacterEventSO")]
public class CharacterEventSO : ScriptableObject
{
    public UnityAction<character> OnEventRised;//创建一个接收并传递character的事件

    public void EventRised(character character)
    {
        OnEventRised?.Invoke(character);
    }

}
