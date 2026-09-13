using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [Header("攻击参数")]
    public int damage;
    public float attackRange;
    public float attackRate;

    private void OnTriggerStay2D(Collider2D other)
    {
        other.GetComponent<character>()?.TakeDamage(this);// 如果碰到的物体身上有 character 组件，就把当前 Attack 传过去，让对方读取伤害值和攻击者位置。
    }
}
  
