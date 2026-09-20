using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Sign : MonoBehaviour//这是一个挂在玩家身上的脚本，负责玩家与可交互物体的交互
{
    public GameObject signSprite;//e键
    public bool canPress;//玩家是否可以按e键
    private Animator anim;
    public Transform playerTrans;
    public PlayerInputControl playerInput;
    public IInteractable targetItem;//玩家当前可以交互的物体
    private void Awake()
    {
        anim = signSprite.GetComponent<Animator>();
        playerInput = new PlayerInputControl();//new一个playerInput出来，当需要玩家执行操作的时候，就写这个
    }

    private void OnEnable()
    {
        playerInput.Enable();//启动输入
        playerInput.GamePlayer.Confirm.started += OnConfirm;//监听确认按键事件
    }

    private void OnDisable()
    {
        canPress = false;
        playerInput.GamePlayer.Confirm.started -= OnConfirm;
        playerInput.Disable();
    }

    private void OnConfirm(InputAction.CallbackContext context)//括号里面的参数是玩家按下e键之后传过来的参数
    {
        if (!canPress || targetItem == null)
        {
            return;
        }

        var interaction = targetItem;//获取当前玩家可以交互的物体
        interaction.TriggerAction();//调用当前物体的交互方法

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
