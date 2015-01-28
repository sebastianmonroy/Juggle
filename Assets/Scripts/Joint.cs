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

		B.SetMother(A);

		this.distance = Vector2.Distance(A.position, B.position);
		this.rigidity = 4f;
	}
}