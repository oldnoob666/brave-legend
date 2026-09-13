using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Sign : MonoBehaviour
{
    public GameObject signSprite;
    public bool canPress;
    private Animator anim;
    public Transform playerTrans;
    public PlayerInputControl playerInput;
    public IInteractable targetItem;
    private void Awake()
    {
        anim = signSprite.GetComponent<Animator>();
        playerInput = new PlayerInputControl();
    }

    private void OnEnable()
    {
        playerInput.Enable();
        playerInput.GamePlayer.Confirm.started += OnConfirm;
    }

    private void OnDisable()
    {
        playerInput.GamePlayer.Confirm.started -= OnConfirm;
        playerInput.Disable();
    }

    private void OnConfirm(InputAction.CallbackContext context)
    {
        if (!canPress || targetItem == null)
        {
            return;
        }

        var interaction = targetItem;
        interaction.TriggerAction();

        if (interaction is Component component
            && (!component.gameObject.activeInHierarchy || !component.gameObject.CompareTag("Interactable")))
        {
            ClearInteraction(interaction);
        }
    }

    private void Update()
    {
        signSprite.GetComponent<SpriteRenderer>().enabled = canPress;
        signSprite.transform.localScale = playerTrans.localScale;
    }


    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Interactable"))
        {
            var interaction = other.GetComponent<IInteractable>();
            if (interaction != null)
            {
                canPress = true;
                targetItem = interaction;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var interaction = other.GetComponent<IInteractable>();
        if (ReferenceEquals(interaction, targetItem))
        {
            ClearInteraction(interaction);
        }
    }

    private void ClearInteraction(IInteractable interaction)
    {
        if (!ReferenceEquals(interaction, targetItem))
        {
            return;
        }

        canPress = false;
        targetItem = null;
    }
}
