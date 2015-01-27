using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainStateManager : MonoBehaviour
{
	public static MainStateManager instance;
	public SimpleStateMachine stateMachine;
	SimpleState setupState, onState, overState, finishedState;

	void Start()
	{
		instance = this;
		setupState = new SimpleState(SetupEnter, SetupUpdate, SetupExit, "[SETUP]");
		onState = new SimpleState(OnEnter, OnUpdate, OnExit, "[GAME ON]");
		overState = new SimpleState(OverEnter, OverUpdate, OverExit, "[GAME OVER]");
		finishedState = new SimpleState(null, null, null, "[FINISHED]");

		stateMachine.SwitchStates(setupState);
	}
	
	void Update () { Execute();	}

	public void Execute () { stateMachine.Execute(); }

	public void SetupEnter(){}
	public void SetupUpdate(){}
	public void SetupExit(){}	

	public void OnEnter(){}
	public void OnUpdate(){}
	public void OnExit(){}

	public void OverEnter(){}
	public void OverUpdate(){}
	public void OverExit(){}	
}
