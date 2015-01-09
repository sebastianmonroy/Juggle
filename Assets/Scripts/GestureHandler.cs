using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Finger 
{
	public Touch touch;
	public int id;

	public Vector2 position;
	public List<Vector2> prevPositions = new List<Vector2>();
	public Vector2 velocity;

	public bool isValid;
	public bool isEmpty;

	public Finger() {}

	public Finger(Touch touch) 
	{
		Initialize(touch);
	}

	public void Initialize(Touch touch) 
	{
		this.touch = touch;
		this.id = touch.fingerId;
		this.position = GetWorldPosition();
		this.isValid = true;
		this.isEmpty = true;
	}

	public void Update(Touch touch)
	{
		this.touch = touch;
		this.id = touch.fingerId;

		// don't allow list of previous positions to be longer than 10
		if (prevPositions.Count > 9)
		{
			prevPositions.RemoveAt(0);
		}
		prevPositions.Add(this.position);


		this.position = GetWorldPosition();

		// calculate finger velocity
		Vector2 sumDeltas = Vector2.zero;
		for (int i = 1; i < prevPositions.Count; i++)
		{
			sumDeltas += prevPositions[i] - prevPositions[i-1];
		}
		sumDeltas += this.position - prevPositions[prevPositions.Count-1];
		sumDeltas /= (Time.deltaTime * prevPositions.Count);
		this.velocity = sumDeltas;

		this.isValid = true;

		Debug.DrawRay(new Vector3(this.position.x, this.position.y, -2f), Vector3.forward * 3f, Color.red);
	}

	public Vector2 GetScreenPosition() {
		return new Vector2(this.touch.position.x, this.touch.position.y);
	}

	public Vector2 GetWorldPosition() {
		Vector2 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(this.touch.position.x, this.touch.position.y, 0f));
		return worldPos;
	}

	public override string ToString() {
		string output = "";
		output += "Finger " + id + " ";
		output += "@ {" + position.x + ", " + position.y + "} ";
		output += " with velocity {" + velocity.x + ", " + velocity.y + "}";

		return output;
	}
}

public class GestureHandler : MonoBehaviour 
{
	public static GestureHandler instance;
	public int fingerCount = 0;
	public List<Finger> fingers = new List<Finger>();

	void Start()
	{
		instance = this;
	}

	void Update() 
	{
		// Populate list of all current touches
		List<Touch> newTouches = new List<Touch>();
		foreach (Touch touch in Input.touches) {
			newTouches.Add(touch);
		}

		// Update existing Fingers using reported touches
		foreach (Finger finger in fingers) 
		{
			bool found = false;
			foreach (Touch touch in Input.touches) 
			{
				if (touch.fingerId == finger.id) 
				{
					finger.Update(touch);
					newTouches.Remove(touch);
					found = true;
				}
			}

			if (!found) 
			{
				finger.isValid = false;
			}
		}

		// Clean up invalid fingers
		List<Finger> tempFingers = fingers;
		foreach (Finger finger in fingers) 
		{
			if (!finger.isValid) 
			{
				tempFingers.Remove(finger);
			}
		}
		fingers = tempFingers;

		// Make new Fingers for remaining unaccounted-for touches
		foreach (Touch touch in newTouches) 
		{
			Finger newFinger = new Finger(touch);
			fingers.Add(newFinger);
		}

		fingerCount = fingers.Count;

		PrintAllFingers();
	}

	void PrintAllFingers() {
		if (fingerCount > 0) 
		{
			Debug.Log("");
			Debug.Log("########### " + fingerCount + " Fingers ###########");
			foreach (Finger finger in fingers) 
			{
				Debug.Log("\t" + finger.ToString());
			}
			Debug.Log("#################################");
		} 
		else 
		{
			//Debug.Log("########### 0 Fingers ###########");
		}
	}
}
