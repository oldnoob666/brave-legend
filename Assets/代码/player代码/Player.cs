using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("输入控制")]
    public PlayerInputControl inputControl;
    private CapsuleCollider2D cap;
    private PlayerAnimater PlayerAnimater;

    [Header("移动参数")]
    public Vector2 InputDirection;
    public float speed;
    private float runSpeed ;
    public float shiftSpeed;
    private float walkSpeed => speed / 2.5f;
    public Rigidbody2D rb;

    [Header("跳跃参数")]
    public float jumpForce;
    private PhysicsCheck physicsCheck;

    [Header("地面与下蹲状态")]
    public bool isGround;
    public bool isCrouch;
    private Vector2 originalOffset;
    private Vector2 originalSize;

    [Header("物理材质")]
    public PhysicsMaterial2D normal;
    public PhysicsMaterial2D wall;


    [Header("战斗与受伤状态")]
    public bool isHurt;
    public bool isAttack;
    public float hurtForce;

    [Header("死亡状态")]
    public bool isDead;
    private void Awake()
    {
        inputControl = new PlayerInputControl();
        cap = GetComponent<CapsuleCollider2D>();
        PlayerAnimater = GetComponent<PlayerAnimater>();

        originalOffset = cap.offset;
        originalSize = cap.size;

        inputControl.GamePlayer.Jump.started += Jump;

        inputControl.GamePlayer.attack.started += Attack;

        runSpeed = speed;
        physicsCheck = GetComponent<PhysicsCheck>();
        inputControl.GamePlayer.walkButton.performed += ctx =>// 按住慢走键时触发 performed；只有在地面上才把移动速度切换成慢走速度。
        {
            if (physicsCheck.isGround) speed = walkSpeed;
        };

        inputControl.GamePlayer.walkButton.canceled += ctx =>
        {
             speed = runSpeed;
        };
        inputControl.GamePlayer.shift.performed += ctx =>
        {
            if (physicsCheck.isGround)
            {
                speed = shiftSpeed;
                //Debug.Log("chongci");
            }
        };
        inputControl.GamePlayer.shift.canceled += ctx =>
        {
             speed = runSpeed;
        };
  }

    

    private void OnEnable()
    {
        inputControl.Enable();
    }

    private void OnDisable()
    {
        inputControl.Disable();
    }

    private void Update()
    {
        InputDirection = inputControl.GamePlayer.Move.ReadValue<Vector2>();
        ChackState();
        
    }
    private void FixedUpdate()
    {
        if ( !isHurt && !isAttack)
        Move();
    }

    // 预留触发器检测入口，需要调试触发器对象时可以打开下面的 Debug.Log。
    private void OnTriggerStay2D(Collider2D other) 
    {
        //Debug.Log(other.name);
       
    }

    public void Move()
    {
        rb.velocity = new Vector2(InputDirection.x * speed * Time.deltaTime,rb.velocity.y);
        int faceDir = (int)transform.localScale.x;
        if (InputDirection.x > 0) faceDir = 1;
        if (InputDirection.x < 0) faceDir = -1;
        transform.localScale = new Vector3(faceDir, 1, 1); // 只把朝向限制为 1 或 -1，避免 localScale 变成 0 导致角色不可见。

        isCrouch  = InputDirection.y < -0.5f && physicsCheck.isGround;// 向下输入超过阈值，并且角色在地面上时，才进入下蹲状态。
        if (isCrouch)
        {
            cap.offset = new Vector2(-0.05f, 0.85f);
            cap.size = new Vector2(0.7f, 1.7f);
        }
        else
        {
            cap.size = originalSize;
            cap.offset = originalOffset;
        }
    }
    private void Jump(InputAction.CallbackContext context)
    {
        if (physicsCheck.isGround)
           rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);// Impulse 表示瞬间施加一次力，适合用来做跳跃起跳。
    }

    private void Attack (InputAction.CallbackContext context)
    {
            rb.velocity = new Vector2(0, rb.velocity.y);// 攻击开始时清掉横向速度，避免冲刺速度残留导致角色向前滑很远。
            PlayerAnimater.PlayAttack();
            isAttack = true;
            
    }





    public void GetHurt(Transform attacker)
    {
        isHurt = true; 
        rb.velocity = Vector2.zero;
        Vector2 dir = new Vector2((transform.position.x - attacker.position.x),0).normalized;


        rb.AddForce(dir * hurtForce, ForceMode2D.Impulse);
    }

    public void PlayerDead()
    {
        isDead = true;
        inputControl.GamePlayer.Disable();
    }

    private void ChackState()
    {

        cap.sharedMaterial = physicsCheck.isGround ? normal : wall;

    }


}
  
