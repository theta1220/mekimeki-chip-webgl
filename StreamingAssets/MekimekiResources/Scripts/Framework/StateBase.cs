public abstract class StateBase
{
    public virtual void OnEnter()
    {
    }

    public virtual void OnExit()
    {
    }

    public abstract void Update();
    public abstract void Draw();
}