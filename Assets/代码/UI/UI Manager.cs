using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("事件监听")]
    public CharacterEventSO healthEvent;//引用CharacterEventSO脚本，方便订阅事件
    public PlayerStatBar playerStatBar;//引用PlayerStatBar脚本，方便调用OnHealthChange方法
    private void OnEnable()
    {
        healthEvent.OnEventRised += OnHealthEvent;//在这里订阅CharacterEventSO的事件，当事件触发时调用OnHealthEvent方法
    }
    private void OnDisable()
    {
        healthEvent.OnEventRised -= OnHealthEvent;//在这里取消订阅CharacterEventSO的事件，避免内存泄漏
    }

    private void OnHealthEvent(character character)//当CharacterEventSO的事件触发时，调用这个方法，传入character参数
    {
        var presentage = character.currentHealth/character.maxHealth;//计算当前血量百分比
        playerStatBar.OnHealthChange(presentage);//调用PlayerStatBar的OnHealthChange方法，传入血量百分比
    }
}
