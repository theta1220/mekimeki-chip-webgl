using System.Collections.Generic;
using System.Linq;
using Sirius.Engine;

public class StateMachine
{
    public StateBase Current;
    public List<StateBase> States;

    public StateMachine()
    {
        States = new List<StateBase>();
    }

    public void Register(StateBase state)
    {
        States.Add(state);
    }

    public void New<T>(T state) where T : StateBase
    {
        var sameState = States.FirstOrDefault(_ => _ is T);
        if (sameState != null)
        {
            States.Remove(sameState);
        }
        Register(state);
        Current?.OnExit();
        Current = state;
        Current.OnEnter();
    }

    public void Switch<T>() where T : StateBase
    {
        var next = States.FirstOrDefault(_ => _ is T);
        if (next == null)
        {
            Logger.Error($"{typeof(T)}のステートが見つかりませんでした");
            return;
        }

        Current?.OnExit();
        Current = next;
        Current.OnEnter();
    }

    public void Update()
    {
        Current?.Update();
    }

    public void Draw()
    {
        Current?.Draw();
    }
}