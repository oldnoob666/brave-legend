using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimater : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private PhysicsCheck physicsCheck;
    private Player player;
    private void Awake()
    {
        anim = GetComponent<Animator>();   
        rb = GetComponent<Rigidbody2D>();
        physicsCheck = GetComponent<PhysicsCheck>();
        player = GetComponent<Player>();
    }

    private void Update()
    {
        SetAnimation();
    }

    public void SetAnimation()
    {
        anim.SetBool("isCroush", player.isCrouch);
        anim.SetFloat("velocityX", Mathf.Abs(rb.velocity.x ));
        anim.SetFloat("velocityY", rb.velocity.y); 
        anim.SetBool("isGround", physicsCheck.isGround);
        anim.SetBool("isDead",player.isDead);
        anim.SetBool("isAttack", player.isAttack);
    }

    public void PlayHurt()
    {
        anim.SetTrigger("hurt");
    } 

    public void PlayAttack()
    {
        anim.SetTrigger("attack");
    }
}
