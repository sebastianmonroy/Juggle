using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pad : MonoBehaviour 
{
	private Color color;
	private float radius = 1f;

	private float mass;
	public Vector2 position;
	public Vector2 velocity;

	public Finger finger;
	public bool isHeld;

	public Pad mother;
	public bool isPlayerPad = false;
	public int player = -1;
	public bool debug; 

	private LineRenderer lineRenderer;

	void Awake () 
	{
		lineRenderer = this.GetComponent<LineRenderer>();
	}
	
	void Update () 
	{
		if (isHeld) 
		{
			if (finger.isValid && finger != null)
			{
				if ((this.isPlayerPad && MainStateManager.instance.stateMachine.currentState == "[GAME ON]")
					|| (!this.isPlayerPad && MainStateManager.instance.stateMachine.currentState == "[SETUP]" 
						&& ((this.mother != null) || (this.mother == null))))
				{
					this.position = Vector2.Lerp(position, finger.position, Time.deltaTime * 2.5f);
					this.velocity = Vector2.Lerp(velocity, finger.velocity, Time.deltaTime * 2.5f);
				}
			}
			else 
			{
				Print("finger unheld");
				isHeld = false;
				finger = null;
			}
		}


		Move();
		Friction();

		this.transform.position = new Vector3(this.position.x, this.position.y, 0f);
		this.transform.localScale = Vector3.one * 2f * radius;
		this.GetComponent<SpriteRenderer>().color = color;
	}

	void LateUpdate()
	{
		if (mother != null)
		{
			DrawJoint(mother);
		}
	}

	void Move() 
	{
		if ((this.isPlayerPad && MainStateManager.instance.stateMachine.currentState != "[SETUP]")
			|| (!this.isPlayerPad))
		{
			this.position += this.velocity * Time.deltaTime;
		}
	}

	void Friction() 
	{
		this.velocity *= 0.975f;
	}

	public void SetPlayerPad(int player)
	{
		this.player = player;
		
		if (player > 0 && player < 3)
		{
			this.isPlayerPad = true;
		}
		else
		{
			this.isPlayerPad = false;
		}
	}

	public bool IsPlayerPad()
	{
		return this.isPlayerPad;
	}

	public float GetRadius()
	{
		return this.radius;
	}

	public void SetRadius(float radius)
	{
		this.radius = radius;
	}

	public Color GetColor()
	{
		return this.color;
	}

	public void SetColor(Color color)
	{
		this.color = color;
	}

	public Vector2 GetPosition() 
	{
		return this.position;
	}

	public Vector2 GetVelocity() 
	{
		return this.velocity;
	}

	public void SetPosition(Vector2 pos) 
	{
		this.position = pos;
	}

	public void SetVelocity(Vector2 vel) 
	{
		this.velocity = vel;
	}

	public void AddVelocity(Vector2 vel) 
	{
		this.velocity += vel;
	}

	public Vector3 GetPosition3() 
	{
		return new Vector3(this.position.x, this.position.y, 0f);
	}

	public Vector3 GetVelocity3() 
	{
		return new Vector3(this.velocity.x, this.velocity.y, 0f);
	}

	public void SetPosition3(Vector3 pos) 
	{
		this.position = new Vector2(pos.x, pos.y);
	}

	public void SetVelocity3(Vector3 vel) 
	{
		this.velocity = new Vector2(vel.x, vel.y);
	}

	public void AddVelocity3(Vector3 vel) 
	{
		this.velocity += new Vector2(vel.x, vel.y);
	}

	public void DrawJoint(Pad other)
	{
		lineRenderer.enabled = true;
		lineRenderer.SetVertexCount(2);
		lineRenderer.SetPosition(0, this.transform.position);
		lineRenderer.SetPosition(1, other.transform.position);
	}

	public void RedrawJoint()
	{
		DrawJoint(mother);
	}

	public void UndrawJoint()
	{
		lineRenderer.SetVertexCount(0);
		lineRenderer.enabled = false;
	}

	public void Hold(Finger finger) 
	{
		Print("isheld " + isHeld);
		finger.isEmpty = false;

		this.finger = finger;
		this.isHeld = true;
	}

	public void SetMother(Pad mother)
	{
		this.mother = mother;
		Interaction.instance.CreateJoint(this, mother);
	}

	public Pad GetMother()
	{
		return this.mother;
	}

	public void DisablePad()
	{
		StartCoroutine(Disable(3.0f));
	}

	IEnumerator Disable(float duration)
	{
		Color enabledColor = this.GetColor();
		Color disabled1Color = enabledColor;
		Color disabled2Color = enabledColor;
		disabled1Color.a = 0.3f;
		disabled2Color.a = 0.6f;
		this.GetComponent<Collider2D>().enabled = false;
		SetColor(disabled1Color);
		//UndrawJoint();

		Timer timer = new Timer(duration);
		while (timer.Percent() < 1f)
		{
			if ((timer.Percent() % 0.2f) < 0.05f)
			{
				SetColor(disabled1Color);
			}
			else
			{
				SetColor(disabled2Color);
			}
			yield return 0;
		}

		this.GetComponent<Collider2D>().enabled = true;
		SetColor(enabledColor);
		//RedrawJoint();
	}

	void Print(string output)
	{
		if (debug)
		{
			Debug.Log(output);
		}
	}

	void OnTriggerEnter2D(Collider2D otherCollider)
	{
		Print("collision");

		
		//Collider2D otherCollider = collision.collider;
		GameObject otherObject = otherCollider.gameObject;
		Pad otherPad = otherCollider.GetComponent<Pad>();

		/*// if pad hit wall
		if (otherObject.tag == "Wall")
		{
			// find overall contact normal axis
			Vector2 contactAxis = Vector2.zero;
			foreach (ContactPoint2D contact in collision.contacts)
			{
				contactAxis += contact.normal;
			}
			contactAxis.Normalize();

			// reflect velocity over contact normal axis
			this.velocity -= 2f * Vector2.Dot(this.velocity, contactAxis) * contactAxis;

		}*/

		// pad hit wall
		if (otherObject.tag == "Wall")
		{
			// find contact normal axis
			this.AddVelocity(-2f * this.GetVelocity());
		}

		// if child hit another pad and other pad's mother isn't me
		if (this.GetMother() != null && otherPad != null && otherPad.GetMother() != this && otherPad.GetMother() != this.GetMother())
		{
			Vector2 collisionTo = otherPad.GetPosition() - this.GetPosition();

			// if hit the other player's mother pad
			if (otherPad.IsPlayerPad() && this.GetMother() != otherPad && !this.isHeld)
			{
				Print("hit mother pad");
				//otherPad.SetRadius(0.9f * otherPad.GetRadius());
				this.AddVelocity(-1.5f * this.GetVelocity());
				AnimationHandler.instance.ChildOnMotherCollision(otherPad, this);
				ScoreHandler.instance.PlayerScored(this.GetMother().player);
				this.DisablePad();
			}
			// else if hit other player's child pad
			else if (otherPad.GetMother() != null)
			{
				Print("hit child pad");

				Vector2 myVelocity = this.GetVelocity();
				Vector2 otherVelocity = otherPad.GetVelocity();


				otherPad.AddVelocity(myVelocity / 2f);
				this.AddVelocity(otherVelocity / 2f);

				//if (myVelocity.magnitude > 3f && myVelocity.magnitude > otherVelocity.magnitude)
				//{
					this.DisablePad();
				//}
				//else if (otherVelocity.magnitude > 3f && myVelocity.magnitude <= otherVelocity.magnitude)
				//{
					otherPad.DisablePad();
				//}

				//Interaction.instance.DestroyJoint(this);
				//Interaction.instance.DestroyJoint(otherPad);
				
				/*if (myVelocity.magnitude > 25f && myVelocity.magnitude >= 2f * otherVelocity.magnitude)
				{
					Interaction.instance.DestroyJoint(otherPad);
				}*/
			}
		}
	}
}
