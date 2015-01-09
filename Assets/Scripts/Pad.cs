using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pad : MonoBehaviour 
{
	public Color color;
	public float radius = 1f;

	public float mass;
	public Vector2 position;
	public Vector2 velocity;

	public Finger finger;
	public bool isHeld;

	void Start () 
	{
		
	}
	
	void Update () 
	{
		if (isHeld) 
		{
			if (finger.isValid && finger != null)
			{
				velocity = finger.velocity;
			}
			else 
			{
				isHeld = false;
				finger = null;
				radius = 1f;
			}
		}


		Move();

		this.mass = this.radius;
		this.transform.position = new Vector3(position.x, position.y, 0f);
		this.transform.localScale = Vector3.one * radius;
		this.GetComponent<SpriteRenderer>().color = color;
	}

	void Move() {
		position += velocity * Time.deltaTime;
	}

	public void Hold(Finger finger) {
		finger.isEmpty = false;
		
		this.finger = finger;
		this.isHeld = true;
		this.radius = 1.5f;
	}
}
