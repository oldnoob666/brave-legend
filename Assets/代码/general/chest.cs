using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chest : MonoBehaviour, IInteractable
{
    [Header("宝箱外观")]
    public Sprite openchest;
    public Sprite closechest;
    public bool isDone;

    [Header("背包奖励")]
    public ItemData rewardItem;
    [Min(1)] public int rewardAmount = 1;
    public AddItemEventSO addItemEvent;

    private SpriteRenderer spriteRenderer;
    private Collider2D interactionCollider;


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        interactionCollider = GetComponent<Collider2D>();

    }
    private void OnEnable()
    {
        spriteRenderer.sprite = isDone ? openchest : closechest;
    }
    public void TriggerAction()
    {
        if (isDone)
        {
            return;
        }

        if (rewardItem == null || addItemEvent == null || rewardAmount <= 0)
        {
            Debug.LogWarning($"Chest '{name}' has an invalid inventory reward configuration.", this);
            return;
        }

        OpenChest();
    }

    private void OpenChest()
    {
        spriteRenderer.sprite = openchest;
        isDone = true;
        addItemEvent.Raise(rewardItem, rewardAmount);
        gameObject.tag = "Untagged";

        if (interactionCollider != null)
        {
            interactionCollider.enabled = false;
        }
    }

}
