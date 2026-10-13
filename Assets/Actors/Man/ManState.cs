using UnityEngine;

public class ManState
{
    protected ManController man;

    public void Init(ManController man)
    {
        this.man = man;
    }

    public virtual void Enter(string prevState)
    {

    }

    public virtual void Update(float deltaTime)
    {

    }

    public virtual void Exit()
    {

    }
}
