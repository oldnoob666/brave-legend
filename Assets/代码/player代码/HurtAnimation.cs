using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtAnimation : StateMachineBehaviour
{
   
    // 受伤动画状态退出时调用，说明硬直结束，可以把玩家从受伤锁定状态恢复出来。
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

       animator.GetComponent<Player>().isHurt = false; 
    }
}
