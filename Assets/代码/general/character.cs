using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class character : MonoBehaviour
{
    [Header("生命值")]
    public float maxHealth;
    public float currentHealth;

    [Header("受伤无敌")]
    public float invulnerableDuration;// 受伤后的无敌持续时间，避免角色在短时间内连续扣血。
    private float invulnerableCounter;// 无敌状态的倒计时，减到 0 后重新允许受伤。
    public bool invulnerable;


    [Header("角色事件")]
    public UnityEvent<Transform> OnTakeDamage;// 受伤事件。把攻击者的 Transform 传出去，方便播放击退、受击朝向或特效。
    public UnityEvent OnDie;
    public UnityEvent<character> OnHealthChange;
    private void Start()
    {
        currentHealth = maxHealth;// 游戏开始时把当前血量恢复到最大血量。
        OnHealthChange?.Invoke(this);
    }

    private void Update()
    {
        if (invulnerable)// 处于无敌状态时，每帧更新倒计时。
        {
            invulnerableCounter -= Time.deltaTime;// 扣除本帧经过的时间。
            if (invulnerableCounter <= 0)// 倒计时结束后关闭无敌状态。
            {
                invulnerable = false;// 角色可以再次受到伤害。
            }
        }
    }

    public void TakeDamage(Attack attacker)// 接收攻击组件，读取其中的伤害值，并通过 attacker.transform 判断攻击来源。
    {
        //Debug.Log(attacker.damage);
        if (invulnerable)// 无敌期间直接返回，不扣血，也不重复触发受伤事件。
            return;
        if (currentHealth - attacker.damage > 0)// 扣除本次伤害后仍然有血量，说明只是受伤，还没有死亡。
        {
            currentHealth -= attacker.damage;// 从当前血量中扣掉攻击伤害。
            TriggerInvulnerable();// 开启短暂无敌，防止同一个攻击判定连续扣血。
            OnTakeDamage?.Invoke(attacker.transform);// 通知外部“我受伤了”，并把攻击者位置传给监听者处理击退、动画或特效。
            
        }
        else
        {
            currentHealth = 0;// 伤害足以致死时，把血量归零。
            OnDie?.Invoke();// 触发死亡事件，让动画、禁用输入或销毁对象等逻辑在外部响应。
        }

        OnHealthChange?.Invoke(this);
    
    } 

    private void TriggerInvulnerable()// 开启受伤后的短暂无敌。
    {
        if (!invulnerable)// 只有当前不在无敌状态时才重新开启计时。
        {
            invulnerable = true;// 标记为无敌，TakeDamage 会根据这个 bool 跳过后续伤害。
            invulnerableCounter = invulnerableDuration;// 从配置的无敌时间开始倒计时。
        }
    }


    private void OnTriggerStay2D(Collider2D other)
    {
        
        if(other.CompareTag("Water"))
        {
            //Debug.Log("jinshui");
            currentHealth = 0;
            OnHealthChange?.Invoke(this);
            OnDie?.Invoke();
        }
    }
}
