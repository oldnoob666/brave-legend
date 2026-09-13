using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PhysicsCheck : MonoBehaviour
{
    [Header("检测结果")]
    public bool isGround;
    public bool touchLeftWall;
    public bool touchRightWall;

    [Header("检测设置")]
    public bool manual;
    public float checkRadius;
    public LayerMask groundLayer;

    [Header("检测点偏移")]
    public Vector2 bottomOffset,leftOffset,rightOffset;
    private CapsuleCollider2D coll;





    private void Awake()
    {
        coll = GetComponent<CapsuleCollider2D>();
        if (!manual)
        {
            rightOffset = new Vector2((coll.bounds.size.x + coll.offset.x) / 2, coll.bounds.size.y / 2);
            leftOffset = new Vector2(-rightOffset.x, rightOffset.y);
        }
    }
    private void Update()
    {
        Check();
        
        
    }
    public void Check()
    {
        isGround = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset*transform.localScale, checkRadius, groundLayer);// 用脚底检测点判断是否站在地面上。bottomOffset 乘以 localScale 后，角色翻转时检测点也能跟着方向变化。


        // 左右墙体检测，用于判断角色或敌人是否碰到墙。
        touchLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, checkRadius, groundLayer);
        touchRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, checkRadius, groundLayer);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset * transform.localScale, checkRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset, checkRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset, checkRadius);
    }
}  
