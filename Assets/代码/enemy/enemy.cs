using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy : MonoBehaviour
{
    [Header("移动参数")]
    public float normalSpeed;
    public float chaseSpeed;
    [HideInInspector]public float currentspeed;

    [Header("击退力")]
    public float hurtForce;

    [Header("朝向与检测状态")]
    public Vector3 faceDir;
    [HideInInspector] public PhysicsCheck PhysicsChack;

    [Header("计时器")]
    public float waitime;
    public float waitimeCounter;
    public bool wait;
    public float lostTime;
    public float lostTimeCenter;

    [Header("生命状态")]
    public bool isHurt;
    public bool isDead;
    public bool isAttack;
    public Transform attacker;
    Rigidbody2D rb;
    [HideInInspector] public Animator anim;

    [Header("发现玩家检测")]
    public Vector2 centerOffset;
    public Vector2 checkSize;
    public float checkDistance;
    public LayerMask attackLayer;



    private BaseState currentState;// 当前正在执行的状态。只要替换这个变量，就能让敌人在不同状态之间切换。
    protected BaseState patrolState;// 巡逻状态：默认来回移动，并检测是否发现玩家。
    protected BaseState chaseState;// 追击状态：发现玩家后使用，具体追击逻辑由子类状态实现。
    protected BaseState dropState;
    // 之后如果要增加攻击、眩晕、返回出生点等状态，也可以继续声明新的 BaseState 字段。


 


    protected  virtual void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        currentspeed = normalSpeed;
        PhysicsChack = GetComponent<PhysicsCheck>();
        waitimeCounter = waitime; // 初始化等待计时器，默认从设定的等待时间开始。
    }


    private void OnEnable()
    {
        currentState = patrolState;//这里设置默认为巡逻状态
        currentState.OnEnter(this);// 把当前 enemy 对象传给状态类，让状态类可以控制这个敌人。
    }

    private void Update()
    {
        faceDir = new Vector3(-transform.localScale.x, 0, 0);// 根据本地缩放计算面朝方向，用于移动、寻敌和墙体判断。
      
        currentState.LogicUpdate();// 每帧执行当前状态的普通逻辑，例如发现玩家或决定是否停下。
        TimeCounter();// 等待计时依赖 Time.deltaTime，放在 Update 中处理更合适。
        if(FoundPlayer())
        {
            lostTimeCenter = lostTime;
        }
    }
    private void FixedUpdate()
    {
        currentState.PhysicsUpdate();// 固定帧执行当前状态的物理逻辑，方便以后把移动或碰撞相关代码放进状态里。
    }

    private void OnDisable()
    {
        currentState.OnExit();
    }


    public bool FoundPlayer()
    {
        return Physics2D.BoxCast(transform.position + (Vector3)centerOffset, checkSize, 0, faceDir, checkDistance, attackLayer); 
    }

    

    public void SwitchState(NPCState state)// 根据传入的枚举值切换敌人状态。
    {
        var newState = state switch// 用 switch 表达式把 NPCState 枚举转换成真正要执行的状态对象。
        {
            NPCState.Patrol => patrolState,
            NPCState.Chase => chaseState,
            NPCState.Drop => dropState,
            _ => null
        };
        currentState.OnExit();
        currentState = newState;
        currentState.OnEnter(this);



    }




    public virtual void Move()// 敌人的基础移动逻辑，子类可以 override 改成自己的移动方式。
    {
        if(!isHurt&!isDead&!wait)
           rb.velocity = new Vector2(currentspeed * faceDir.x * Time.deltaTime, rb.velocity.y);// 只修改 x 方向速度，y 方向保留重力和跳落效果。受伤、死亡或等待时不移动。
    }


    public void TimeCounter()// 处理巡逻等待和转身的计时器。
    {
        if (wait)
        {
            waitimeCounter -= Time.deltaTime;// 每帧扣除经过的时间，直到等待时间结束。
            if(waitimeCounter <= 0)
            {
                wait = false;
                waitimeCounter = waitime;// 重置计时器，方便下次再次进入等待。
                transform.localScale = new Vector3(faceDir.x, 1, 1);// 等待结束后翻转朝向，让敌人往反方向巡逻。
            }
        }

        if(!FoundPlayer()&&lostTimeCenter>0)
        {
            
            lostTimeCenter -= Time.deltaTime;
        }
        

    }


    public void OnTakeDamage(Transform attackerTrans)
    {
        isAttack = true;
        attacker = attackerTrans;
        if(attacker.transform.position.x-transform.position.x>0)// 攻击者在敌人右侧时，让敌人朝向右侧，保证受击动画和击退方向看起来正确。
        {
            transform.localScale = new Vector3(-1, 1, 1);
            wait = false;
            waitimeCounter = waitime;
        }
        if (attacker.transform.position.x - transform.position.x<0)
        {
            transform.localScale = new Vector3(1, 1, 1);
            wait = false;
            waitimeCounter = waitime;
        }
        isHurt = true;
        anim.SetTrigger("hurt");
        Vector2 dir = new Vector2(transform.position.x - attackerTrans.position.x,0).normalized;// 计算从攻击者指向敌人的水平单位方向，用作受击击退方向。
        StartCoroutine(OnHurt(dir));// 开启受伤协程，先击退一小段时间，再恢复可移动状态。
       
    }
    private IEnumerator OnHurt( Vector2 dir)// 受伤恢复流程。协程可以等待一段时间，让击退效果播放完成后再结束受伤状态。
    {
        rb.AddForce(dir * hurtForce, ForceMode2D.Impulse);// 用 Impulse 立即施加一次击退力。
        yield return new WaitForSeconds(0.60f);// 等待 0.45 秒，给受击动画和击退留出表现时间。
        isHurt = false;// 受伤结束，允许敌人重新移动。
        isAttack = false;
    }


    public void OnDead()
    {
        gameObject.layer = 2;
        anim.SetBool("dead",true);
        isDead = true;
    }


    public void DestoryAfterAnim()
    {
        Destroy(this.gameObject);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position + (Vector3)centerOffset+new Vector3(checkDistance*-transform.localScale.x,0), 0.2f);
    }


}
