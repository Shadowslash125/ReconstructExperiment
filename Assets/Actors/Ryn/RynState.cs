using UnityEngine;

public class RynState
{
    protected RynControllerA ryn;

    public void Init(RynControllerA ryn)
    {
        this.ryn = ryn;
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
