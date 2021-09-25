using UnityEngine;

public class FSM
{
	public class StateBase
	{
		[HideInInspector] public FSM fsm;
		public virtual void Update(float deltaTime) { }
		public virtual void LateUpdate(float deltaTime) { }
		public virtual void FixedUpdate(float fixedDeltaTime) { }
		public virtual void Exit() { }
		public virtual void UpdateVisual() { }
	}

	public class State : StateBase { public virtual void Enter() { } }
	public abstract class State1Param <T> : StateBase { public abstract void Enter(T p); }
	public abstract class State2Param<T0, T1> : StateBase { public abstract void Enter(T0 p0, T1 p1); }
	public abstract class State3Param<T0, T1, T2> : StateBase { public abstract void Enter(T0 p0, T1 p1, T2 p2); }


	public StateBase currentState { private set; get; }
	


	private bool ChangeStateBase(StateBase newState)
	{
		bool res = false;

		if(currentState != null)
		{
			currentState.Exit();
		}

		currentState = newState;


		res = newState != null;
		return res;
	}

	public void ChangeState(State newState)
	{
		if (ChangeStateBase(newState))
		{
			newState.Enter();
		}
	}

	public void ChangeState<T>(State1Param<T> newState, T p)
	{
		if (ChangeStateBase(newState))
		{
			newState.Enter(p);
		}
	}

	public void ChangeState<T0, T1>(State2Param<T0, T1> newState, T0 p0, T1 p1)
	{
		if (ChangeStateBase(newState))
		{
			newState.Enter(p0, p1);
		}
	}

	public void ChangeState<T0, T1, T2>(State3Param<T0, T1, T2> newState, T0 p0, T1 p1, T2 p2)
	{
		if (ChangeStateBase(newState))
		{
			newState.Enter(p0, p1, p2);
		}
	}

	public void FixedUpdateFSM(float fixedDeltaTime)
	{
		if (currentState != null)
		{
			currentState.FixedUpdate(fixedDeltaTime);
		}
	}

	public void UpdateFSM(float deltaTime)
	{
		if(currentState != null)
		{
			currentState.Update(deltaTime);
		}
	}

	public void LateUpdateFSM(float deltaTime)
	{
		if(currentState != null)
		{
			currentState.LateUpdate(deltaTime);
		}
	}

	public void UpdateVisual()
	{
		if (currentState != null)
		{
			currentState.UpdateVisual();
		}
	}
}
