using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Joint 
{
	public Pad A;
	public Pad B;
	public float distance;

	public Joint() {}

	public Joint(Pad A, Pad B) {
		Instantiate(A, B);
	}

	public void Instantiate(Pad A, Pad B)
	{
		this.A = A;
		this.B = B;
		this.distance = Vector2.Distance(A.position, B.position);
	}
}

public class Interaction : MonoBehaviour 
{
	public GameObject padPrefab;
	public List<Pad> pads = new List<Pad>();
	public List<Color> padColors;

	public List<Joint> joints = new List<Joint>();

	void Start () {
	
	}
	
	void Update () {
		//Debug.Log("" + pads.Count);
		// Handle finger grabs
		foreach (Finger finger in GestureHandler.instance.fingers) 
		{
			// Pads are held by fingers that touch them
			foreach (Pad pad in pads) 
			{
				//Debug.Log("" + Vector2.Distance(pad.position, finger.position));
				if (!pad.isHeld && finger.isEmpty && Vector2.Distance(pad.position, finger.position) <= pad.radius)
				{
					pad.Hold(finger);
				}
			}
		}
		
		// Handle new pad creation
		foreach (Finger finger in GestureHandler.instance.fingers) 
		{
			// Create new pads for new finger touches that miss all pads
			if (finger.isEmpty)
			{
				GameObject newPadObject = Instantiate(padPrefab, new Vector3(finger.position.x, finger.position.y, 0f), Quaternion.identity) as GameObject;
				Pad newPad = newPadObject.GetComponent<Pad>();
				newPad.position = finger.position;
				newPad.color = padColors[Random.Range(0, padColors.Count)];
				newPad.Hold(finger);

				pads.Add(newPad);
			}
		}

		// Handle new joint creation
		foreach (Pad pad in pads) 
		{
			List<Pad> others = pads;

			foreach (Pad other in others) 
			{
				// prevent joints to self
				if (pad != other) 
				{
					bool duplicate = false;
					foreach (Joint joint in joints)
					{
						// prevent duplicate joints
						if ((joint.A == pad && joint.B == other) || (joint.A == other && joint.B == pad))
						{
							duplicate = true;
						}
					}

					float distance = Vector2.Distance(pad.position, other.position);
					float size = pad.transform.lossyScale.x + other.transform.lossyScale.x;
					Debug.Log("" + distance + " vs " + size);
					if (!duplicate && distance <= 1f)
					{
						// create new joint if it is not a duplicate and the 
						Joint newJoint = new Joint(pad, other);
						joints.Add(newJoint);

						Debug.Log("JOIN");
					}
				}
			}
		}

		// Handle joint physics
		foreach (Joint joint in joints)
		{
			Pad A = joint.A;	Pad B = joint.B;
			Vector2 axis = (B.position - A.position).normalized;

			Vector2 A_nextPos = A.position + A.velocity * Time.deltaTime;
			Vector2 B_nextPos = B.position + B.velocity * Time.deltaTime; 

			float relativeVelocity = Vector2.Dot(B.velocity - A.velocity, axis);
			float relativeDistance = Vector2.Distance(A_nextPos, B_nextPos);

			float impulse = (relativeVelocity + relativeDistance / Time.deltaTime) / (1f/A.mass + 1f/B.mass);

			A.velocity += axis * impulse * 1f/A.mass;
			B.velocity -= axis * impulse * 1f/B.mass;

			Debug.DrawLine(new Vector3(A.position.x, A.position.y, 0f), new Vector3(B.position.x, B.position.y, 0f), Color.blue);
		}
	}
}
