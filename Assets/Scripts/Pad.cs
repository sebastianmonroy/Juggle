using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pad : MonoBehaviour 
{
	public Color color;
	public float radius = 0.75f;

	public float mass;
	public Vector2 position;
	public Vector2 velocity;

	public Finger finger;
	public bool isHeld;

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
				position = finger.position;
				velocity = finger.velocity;
			}
			else 
			{
				isHeld = false;
				finger = null;
				radius = 0.75f;
			}
		}


		Move();
		Friction();

		this.mass = this.radius;
		this.transform.position = new Vector3(this.position.x, this.position.y, 0f);
		this.transform.localScale = Vector3.one * radius;
		this.GetComponent<SpriteRenderer>().color = color;
	}

	void Move() 
	{
		this.position += this.velocity * Time.deltaTime;
	}

	void Friction() 
	{
		this.velocity *= 0.975f;
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
		lineRenderer.SetPosition(0, this.transform.position);
		lineRenderer.SetPosition(1, other.transform.position);
	}

	public void Hold(Finger finger) 
	{
		finger.isEmpty = false;

		this.finger = finger;
		this.isHeld = true;
		this.radius = 1f;
	}
}
