using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoarChaseState : BaseState// 野猪的追击状态。之后可以在这里加入追玩家、攻击或丢失目标后的状态切换逻辑。
{
    public override void OnEnter(enemy enemy)
    {
        currentEnemy = enemy;
        currentEnemy.lostTimeCenter = currentEnemy.lostTime;
        Debug.Log("切换追击状态");
        currentEnemy.currentspeed = currentEnemy.chaseSpeed;
        currentEnemy.anim.SetBool("run",true);
    }
    public override void LogicUpdate()
    {
        if(currentEnemy.isAttack)
        {
            currentEnemy.SwitchState(NPCState.Drop);
        }
        if(currentEnemy.lostTimeCenter<=0)
        {
            currentEnemy.SwitchState(NPCState.Patrol);
        }
        if (!currentEnemy.PhysicsChack.isGround || (currentEnemy.PhysicsChack.touchLeftWall && currentEnemy.faceDir.x < 0) || (currentEnemy.PhysicsChack.touchRightWall && currentEnemy.faceDir.x > 0))// 前方没有地面，或移动方向正好撞到对应方向的墙时，先停下来等待，避免继续往坑里或墙上走。
        {
            currentEnemy.transform.localScale = new Vector3(currentEnemy.faceDir.x, 1,1);
            
        }
    }

    public override void PhysicsUpdate()
    {
        currentEnemy.Move();
    }
    public override void OnExit()
    {
        currentEnemy.anim.SetBool("run",false);
        

    }
}
