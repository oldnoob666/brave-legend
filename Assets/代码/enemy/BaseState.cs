

public abstract class BaseState // 敌人状态机的抽象父类。每个具体状态都必须实现进入、逻辑更新、物理更新和退出这几个阶段。
{
    protected enemy currentEnemy;
    public abstract void OnEnter(enemy enemy);// 进入状态时调用，把当前敌人对象传进来，方便状态类读取和控制这个敌人。
    public abstract void LogicUpdate();
    public abstract void PhysicsUpdate();
    public abstract void OnExit();
}
