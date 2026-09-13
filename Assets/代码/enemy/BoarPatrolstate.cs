using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoarPatrolstate : BaseState
{
    public override void OnEnter(enemy enemy)
    {
        currentEnemy = enemy;// 保存进入该状态的敌人对象，后续逻辑都通过 currentEnemy 操作这只野猪。
        currentEnemy.currentspeed = currentEnemy.normalSpeed;
        Debug.Log("切换巡逻形态");
    }


    public override void LogicUpdate()
    {

        if (currentEnemy.isAttack)
        {
            currentEnemy.SwitchState(NPCState.Drop);
        }

        if (currentEnemy.FoundPlayer())// 巡逻时持续检测前方范围，如果发现玩家，就切换到追击状态。
        {
            //Debug.Log("isFound");
            currentEnemy.SwitchState(NPCState.Chase);
        }


        if (!currentEnemy.PhysicsChack.isGround || (currentEnemy.PhysicsChack.touchLeftWall && currentEnemy.faceDir.x < 0) || (currentEnemy.PhysicsChack.touchRightWall && currentEnemy.faceDir.x > 0))// 前方没有地面，或移动方向正好撞到对应方向的墙时，先停下来等待，避免继续往坑里或墙上走。
        {
            
            currentEnemy.wait = true;// 开启等待计时，计时结束后由 enemy.TimeCounter 负责转身。
            currentEnemy.anim.SetBool("walk", false);// 等待时关闭走路动画，让停顿和转身看起来更自然。
        }
        else
        {
            currentEnemy.anim.SetBool("walk", true);
        }
    }

    public override void PhysicsUpdate()
    {
        currentEnemy.Move();
    }


    public override void OnExit()
    {
        currentEnemy.anim.SetBool("walk", false);
        //Debug.Log("退出");
    }

}
