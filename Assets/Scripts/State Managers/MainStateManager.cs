using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainStateManager : MonoBehaviour
{
	public static MainStateManager instance;
	public bool initialized = false;

	public float playerPadRadius = 1.0f;
	public float courtPadRadius = 0.5f;

	public Transform LoadingBarTransform;

	public SimpleStateMachine stateMachine;
	SimpleState setupState, onState, overState, finishedState;

	void Start()
	{
		instance = this;
		initialized = true;

		setupState = new SimpleState(SetupEnter, SetupUpdate, null, "[SETUP]");
		onState = new SimpleState(OnEnter, OnUpdate, OnExit, "[GAME ON]");
		overState = new SimpleState(OverEnter, OverUpdate, OverExit, "[GAME OVER]");
		finishedState = new SimpleState(null, null, null, "[FINISHED]");

		stateMachine.SwitchStates(setupState);
	}
	
	void Update () { stateMachine.Execute(); }


	Timer setupTimer;
	bool courtSetUp = false;
	Vector3 originalLoadingBarScale;

	Pad p1Pad;
	Pad p2Pad;

	public void SetupEnter()
	{
		setupTimer = new Timer(10.0f);
		originalLoadingBarScale = LoadingBarTransform.localScale;
	}

	public void SetupUpdate()
	{
		if (!courtSetUp && Interaction.instance.initialized)
		{
			// Player Pads
			p1Pad = Interaction.instance.CreatePlayerPad(1, new Vector2(0f, -3.0f), playerPadRadius);
			p2Pad = Interaction.instance.CreatePlayerPad(2, new Vector2(0f, 3.0f), playerPadRadius);

			// Child Pads
			//Pad p1Child1 = Interaction.instance.CreateCourtPad(new Vector2(-2.5f, 0f), courtPadRadius);

			// Court Pads
			Pad p1Child1 = Interaction.instance.CreateCourtPad(new Vector2(-1.5f, -3f), courtPadRadius);
			Pad p1Child2 = Interaction.instance.CreateCourtPad(new Vector2(0f, -2f), courtPadRadius);
			Pad p1Child3 = Interaction.instance.CreateCourtPad(new Vector2(1.5f, -3f), courtPadRadius);

			Pad p2Child1 = Interaction.instance.CreateCourtPad(new Vector2(-1.5f, 3f), courtPadRadius);
			Pad p2Child2 = Interaction.instance.CreateCourtPad(new Vector2(0f, 2f), courtPadRadius);
			Pad p2Child3 = Interaction.instance.CreateCourtPad(new Vector2(1.5f, 3f), courtPadRadius);

			p1Child1.SetMother(p1Pad); p1Child2.SetMother(p1Pad); p1Child3.SetMother(p1Pad);
			p2Child1.SetMother(p2Pad); p2Child2.SetMother(p2Pad); p2Child3.SetMother(p2Pad);

			courtSetUp = true;
		}

		Interaction.instance.HandleFingerGrabs();
		Interaction.instance.HandleJointCreation();
		Interaction.instance.HandleJointPhysics();

		LoadingBarTransform.localScale = new Vector3(originalLoadingBarScale.x, originalLoadingBarScale.y * (1f - setupTimer.Percent()), originalLoadingBarScale.z);

		if (setupTimer.Percent() >= 1f)
		{			
			stateMachine.SwitchStates(onState);
		}
	}

	public void OnEnter()
	{
		p1Pad.SetVelocity(Vector2.zero);
		p2Pad.SetVelocity(Vector2.zero);
	}
	
	public void OnUpdate()
	{
		Interaction.instance.HandleFingerGrabs();
		Interaction.instance.HandleJointCreation();
		Interaction.instance.HandleJointPhysics();
	}
	public void OnExit(){}

	public void OverEnter(){}
	public void OverUpdate(){}
	public void OverExit(){}	
}
