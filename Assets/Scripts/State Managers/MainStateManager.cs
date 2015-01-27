using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainStateManager : MonoBehaviour
{
	public static MainStateManager instance;
	public bool initialized = false;

	public float playerPadRadius = 1.0f;
	public float courtPadRadius = 0.5f;

	public SimpleStateMachine stateMachine;
	SimpleState setupState, onState, overState, finishedState;

	void Start()
	{
		instance = this;
		initialized = true;

		setupState = new SimpleState(null, SetupUpdate, null, "[SETUP]");
		onState = new SimpleState(OnEnter, OnUpdate, OnExit, "[GAME ON]");
		overState = new SimpleState(OverEnter, OverUpdate, OverExit, "[GAME OVER]");
		finishedState = new SimpleState(null, null, null, "[FINISHED]");

		stateMachine.SwitchStates(setupState);
	}
	
	void Update () { stateMachine.Execute(); }

	public void SetupUpdate()
	{
		if (Interaction.instance.initialized)
		{
			// Player Pads
			Interaction.instance.CreatePlayerPad(new Vector2(0f, -3.0f), playerPadRadius);
			Interaction.instance.CreatePlayerPad(new Vector2(0f, 3.0f), playerPadRadius);

			// Court Pads
			Interaction.instance.CreateCourtPad(new Vector2(-2.5f, courtPadRadius));
			Interaction.instance.CreateCourtPad(new Vector2(-1.5f, courtPadRadius));
			Interaction.instance.CreateCourtPad(new Vector2(-0.5f, courtPadRadius));
			Interaction.instance.CreateCourtPad(new Vector2(0.5f, courtPadRadius));
			Interaction.instance.CreateCourtPad(new Vector2(1.5f, courtPadRadius));
			Interaction.instance.CreateCourtPad(new Vector2(2.5f, courtPadRadius));

			stateMachine.SwitchStates(onState);
		}
	}

	public void OnEnter(){}
	public void OnUpdate()
	{
		Interaction.instance.HandleFingerGrabs();
		Interaction.instance.HandleJointCreation();
		Interaction.instance.HandleJointPhysics();
		Interaction.instance.HandleJointCreation();
	}
	public void OnExit(){}

	public void OverEnter(){}
	public void OverUpdate(){}
	public void OverExit(){}	
}
