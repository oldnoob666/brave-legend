using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("ÊÂ¼þ¼àÌý")]
    public CharacterEventSO healthEvent;
    public PlayerStatBar playerStatBar;
    private void OnEnable()
    {
        healthEvent.OnEventRised += OnHealthEvent;
    }
    private void OnDisable()
    {
        healthEvent.OnEventRised -= OnHealthEvent;
    }

    private void OnHealthEvent(character character)
    {
        var presentage = character.currentHealth/character.maxHealth;
        playerStatBar.OnHealthChange(presentage);
    }
}
