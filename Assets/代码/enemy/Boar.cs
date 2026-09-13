using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boar : enemy// 野猪敌人类，继承 enemy 的通用移动、受伤、死亡和状态切换逻辑。
{
    protected override void Awake()
    {
        base.Awake();
        patrolState = new BoarPatrolstate();
        chaseState = new BoarChaseState();
        dropState = new BoarDropState();
    }



}
