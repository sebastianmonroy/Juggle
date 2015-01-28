using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Interaction : MonoBehaviour
{
	public static Interaction instance;
	public bool initialized = false;

	public GameObject padPrefab;
	public List<Pad> pads = new List<Pad>();
	public List<Color> padColors;

	public List<Joint> joints = new List<Joint>();

	void Start()
	{
		instance = this;
		initialized = true;
	}

	void LateUpdate()
	{
		HandleJointRendering();
	}

	public void HandleFingerGrabs()
	{
		// Handle finger grabs
		foreach (Finger finger in GestureHandler.instance.fingers) 
		{
			// Pads are held by fingers that touch them
			foreach (Pad pad in pads) 
			{
				//Debug.Log("" + Vector2.Distance(pad.position, finger.position));
				if (!pad.isHeld && finger.isEmpty && Vector2.Distance(pad.GetPosition(), finger.GetWorldPosition()) <= pad.GetRadius())
				{
					pad.Hold(finger);
				}
			}
		}
	}

	public void CreatePlayerPad(Vector2 position, float radius = 1f)
	{
		GameObject newPadObject = Instantiate(padPrefab, new Vector3(position.x, position.y, 0f), Quaternion.identity) as GameObject;
		Pad newPad = newPadObject.GetComponent<Pad>();
		newPad.SetPosition(position);
		newPad.SetRadius(radius);
		newPad.SetColor(RandomColor());
		newPad.SetPlayerPad(true);
		
		pads.Add(newPad);
	}

	public void CreateCourtPad(Vector2 position, float radius = 0.5f)
	{
		GameObject newPadObject = Instantiate(padPrefab, new Vector3(position.x, position.y, 0f), Quaternion.identity) as GameObject;
		Pad newPad = newPadObject.GetComponent<Pad>();
		newPad.SetPosition(position);
		newPad.SetRadius(radius);
		newPad.SetColor(RandomColor());
		
		pads.Add(newPad);
	}

	public void CreateFingerPad(Finger finger)
	{
		GameObject newPadObject = Instantiate(padPrefab, new Vector3(finger.position.x, finger.position.y, 0f), Quaternion.identity) as GameObject;
		Pad newPad = newPadObject.GetComponent<Pad>();
		newPad.SetPosition(finger.position);
		newPad.SetColor(RandomColor());

		newPad.Hold(finger);

		pads.Add(newPad);
	}

	public Color RandomColor()
	{
		return padColors[Random.Range(0, padColors.Count)];
	}

	public void HandlePadCreation()
	{
		// Handle new pad creation
		foreach (Finger finger in GestureHandler.instance.fingers) 
		{
			// Create new pads for new finger touches that miss all pads
			if (finger.isEmpty)
			{
				CreateFingerPad(finger);
			}
		}
	}

	public void HandleJointCreation()
	{
		// Handle new joint creation
		foreach (Pad pad in pads) 
		{
			// only create joints to a player pad
			if (pad.IsPlayerPad())
			{
				List<Pad> others = pads;

				foreach (Pad other in others) 
				{
					// prevent joints to self, only create joint if other pads is held and not a player pad, only allows one joint per pad
					if (other != pad && !other.IsPlayerPad() && other.isHeld && other.GetMother() == null) 
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
						//Debug.Log("" + distance + " vs " + size);
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
		}
	}

	public void HandleJointPhysics()
	{
		// Handle joint physics
		foreach (Joint joint in joints)
		{
			Pad A = joint.A;	Pad B = joint.B;
			Vector3 A_norm = (B.GetPosition3() - A.GetPosition3());
			Vector3 B_norm = (A.GetPosition3() - B.GetPosition3());

			Vector3 A_norm_axis = (B.GetPosition3() - A.GetPosition3()).normalized;
			Vector3 B_norm_axis = (A.GetPosition3() - B.GetPosition3()).normalized;

			Vector3 A_tang_axis = Vector3.Cross(A_norm_axis, Vector3.forward);
			Vector3 B_tang_axis = Vector3.Cross(B_norm_axis, Vector3.forward);

			if (A.isHeld && B.isHeld)
			{
				Debug.DrawLine(new Vector3(A.position.x, A.position.y, 0f), new Vector3(B.position.x, B.position.y, 0f), Color.green);
			}
			else if (A.isHeld || A.IsPlayerPad())
			{
				Vector3 newVel = Vector3.Dot(B.GetVelocity3(), B_tang_axis) * B_tang_axis;
				newVel += Vector3.Dot(A.GetVelocity3(), A_norm_axis) * A_norm_axis;

				B.SetVelocity3(newVel);

				Debug.DrawLine(new Vector3(A.position.x, A.position.y, 0f), new Vector3(B.position.x, B.position.y, 0f), Color.blue);
			}
			else if (B.isHeld || B.IsPlayerPad())
			{
				Vector3 newVel = Vector3.Dot(A.GetVelocity3(), A_tang_axis) * A_tang_axis;
				newVel += Vector3.Dot(B.GetVelocity3(), B_norm_axis) * B_norm_axis;

				A.SetVelocity3(newVel);

				Debug.DrawLine(new Vector3(A.position.x, A.position.y, 0f), new Vector3(B.position.x, B.position.y, 0f), Color.red);
			}
			else
			{
				Debug.DrawLine(new Vector3(A.position.x, A.position.y, 0f), new Vector3(B.position.x, B.position.y, 0f), Color.yellow);
			}
			/*Vector2 A_nextPos = A.position + A.velocity * Time.deltaTime;
			Vector2 B_nextPos = B.position + B.velocity * Time.deltaTime; 

			float relativeVelocity = Vector2.Dot(B.velocity - A.velocity, axis);
			float relativeDistance = Vector2.Distance(A_nextPos, B_nextPos);

			float impulse = (relativeVelocity + relativeDistance / Time.deltaTime) / (1f/A.mass + 1f/B.mass);

			A.velocity += axis * impulse * 1f/A.mass;
			B.velocity -= axis * impulse * 1f/B.mass;*/
		}

		// Handle Joint Correction
		foreach (Joint joint in joints)
		{
			Pad A = joint.A;	Pad B = joint.B;
			Vector2 A_norm_axis = (B.GetPosition() - A.GetPosition()).normalized;
			Vector2 B_norm_axis = (A.GetPosition() - B.GetPosition()).normalized;	
			Vector3 A_norm_axis3 = (B.GetPosition3() - A.GetPosition3()).normalized;
			Vector3 B_norm_axis3 = (A.GetPosition3() - B.GetPosition3()).normalized;
			Vector3 A_tang_axis3 = Vector3.Cross(A_norm_axis, Vector3.forward);
			Vector3 B_tang_axis3 = Vector3.Cross(B_norm_axis, Vector3.forward);
			Vector2 A_tang_axis = new Vector2(A_tang_axis3.x, A_tang_axis3.y);
			Vector2 B_tang_axis = new Vector2(B_tang_axis3.x, B_tang_axis3.y);

			//float dist_error = (Vector2.Distance(B.GetPosition(), A.GetPosition()) - joint.distance);
			float AB_distance = Vector2.Distance(B.GetPosition(), A.GetPosition());
			Vector2 A_desiredPosition = Vector2.zero;
			Vector2 B_desiredPosition = Vector2.zero;
			Vector2 A_newVelocity = A.GetVelocity();
			Vector2 B_newVelocity = B.GetVelocity();

			float A_normVelocity = Vector2.Dot(A.GetVelocity(), A_norm_axis);
			float B_normVelocity = Vector2.Dot(B.GetVelocity(), B_norm_axis);
			float A_tangVelocity = Vector2.Dot(A.GetVelocity(), A_tang_axis);
			float B_tangVelocity = Vector2.Dot(B.GetVelocity(), B_tang_axis);

			float A_error;
			float B_error;

			if (A.isHeld && B.isHeld)
			{
				joint.distance = AB_distance;
			}
			else if (A.isHeld || A.IsPlayerPad())
			{
				A_desiredPosition = A.GetPosition();

				B_desiredPosition = A.GetPosition() + joint.distance * A_norm_axis;
				B_error = Mathf.Sign(AB_distance - joint.distance) * Vector2.Distance(B.GetPosition(), B_desiredPosition);

				B_normVelocity = Mathf.Lerp(B_normVelocity, B_error / Time.deltaTime, Time.deltaTime * joint.rigidity);
				B_newVelocity = B_tangVelocity * B_tang_axis + B_normVelocity * B_norm_axis;
				//Debug.Log(B_addedVelocity);
			}
			else if (B.isHeld || B.IsPlayerPad())
			{
				B_desiredPosition = B.GetPosition();

				A_desiredPosition = B.GetPosition() + joint.distance * B_norm_axis;
				A_error = Mathf.Sign(AB_distance - joint.distance) * Vector2.Distance(A.GetPosition(), A_desiredPosition);
				
				A_normVelocity = Mathf.Lerp(A_normVelocity, A_error / Time.deltaTime, Time.deltaTime * joint.rigidity);
				A_newVelocity = A_tangVelocity * A_tang_axis + A_normVelocity * A_norm_axis;
				//Debug.Log(A_addedVelocity);
			}
			else 
			{
				float AB_error = AB_distance - joint.distance;
				A_error = AB_error / 2f;
				B_error = AB_error / 2f;

				B_normVelocity = Mathf.Lerp(B_normVelocity, B_error / Time.deltaTime, Time.deltaTime * joint.rigidity);
				A_normVelocity = Mathf.Lerp(A_normVelocity, A_error / Time.deltaTime, Time.deltaTime * joint.rigidity);
				
				A_newVelocity = A_tangVelocity * A_tang_axis + A_normVelocity * A_norm_axis;
				B_newVelocity = B_tangVelocity * B_tang_axis + B_normVelocity * B_norm_axis;
			}

			A.SetVelocity(A_newVelocity);
			B.SetVelocity(B_newVelocity);
		}
	}

	public void HandleJointRendering()
	{
		// Handle Joint Rendering
		foreach (Joint joint in joints)
		{
			Pad A = joint.A;	Pad B = joint.B;

			B.DrawJoint(A);
		}
	}

	public void DestroyJoint(Pad child)
	{
		List<Joint> remainingJoints = new List<Joint>();
		List<Joint> deadJoints = new List<Joint>();

		foreach (Joint joint in joints)
		{
			if (joint.A != child && joint.B != child)
			{
				remainingJoints.Add(joint);
			}
			else
			{
				deadJoints.Add(joint);
			}
		}

		joints = remainingJoints;
		child.UndrawJoint();

		foreach (Joint joint in deadJoints)
		{
			joint.A = null;
			joint.B = null;
		}
	}
}
