using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoarDropState : BaseState
{
    public override void OnEnter(enemy enemy)
    {
        currentEnemy = enemy;
        Debug.Log("drop");
    }
    public override void PhysicsUpdate()
    {

    }
    public override void LogicUpdate()
    {
        if(!currentEnemy.isAttack&&currentEnemy.PhysicsChack.isGround)
        {
            currentEnemy.SwitchState(NPCState.Chase);
        }

    }

   

    public override void OnExit()
    {
        
    }

   
}
