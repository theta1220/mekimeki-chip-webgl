public abstract class SceneBase
{
    public StateMachine StateMachine;

    public SceneBase()
    {
        StateMachine = new StateMachine();
    }

    public virtual void OnEnter(){}
    
    public virtual void Update()
    {
        StateMachine.Update();
    }

    public virtual void Draw()
    {
        StateMachine.Draw();
        DrawableManager.Instance.Draw();
    }
}