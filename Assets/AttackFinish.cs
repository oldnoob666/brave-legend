using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackFinish : StateMachineBehaviour
{
    // 攻击动画状态退出时调用，说明本次攻击动作已经播放完，可以恢复玩家移动和下一次攻击。
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<Player>().isAttack = false;
    }
}
