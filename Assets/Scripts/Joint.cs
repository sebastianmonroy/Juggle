using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Joint 
{
	public Pad A;
	public Pad B;
	public float distance;
	public float rigidity;

	public Joint() {}

	public Joint(Pad A, Pad B) {
		Initialize(A, B);
	}

	public void Initialize(Pad A, Pad B)
	{
		this.A = A;
		this.B = B;

		this.SetDistance(Vector2.Distance(A.position, B.position));
		this.rigidity = 15f;
	}

	public void SetDistance(float distance)
	{
		if (A.IsPlayerPad())
		{
			distance = Mathf.Clamp(distance, 0.75f, 3f);
			B.SetRadius(Mathf.Lerp(0.75f, 0.25f, (distance - 0.75f)/2.25f));

			Vector2 AtoB = B.position - A.position;
			B.SetPosition(A.GetPosition() + AtoB.normalized * distance);
			B.SetVelocity(Vector2.zero);
		} 
		else if (B.IsPlayerPad())
		{
			distance = Mathf.Clamp(distance, 0.75f, 3f);
			A.SetRadius(Mathf.Lerp(0.75f, 0.25f, (distance - 0.75f)/2.25f));

			Vector2 BtoA = A.position - B.position;
			A.SetPosition(B.GetPosition() + BtoA.normalized * distance);
			A.SetVelocity(Vector2.zero);
		}

		this.distance = distance;
	}
}