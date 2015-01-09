using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Interaction : MonoBehaviour {
	public GameObject padPrefab;
	public List<Pad> pads = new List<Pad>();

	void Start () {
	
	}
	
	void Update () {
		foreach (Finger finger in GestureHandler.instance.fingers) 
		{
			// Create new pads for new finger touches
			if (finger.isNew)
			{
				GameObject newPadObject = Instantiate(padPrefab, new Vector3(finger.position.x, finger.position.y, 0f), Quaternion.identity) as GameObject;
				Pad newPad = newPadObject.GetComponent<Pad>();
				newPad.position = finger.position;
				newPad.Hold(finger);
			}

			// Pads are held by fingers that touch them
			foreach (Pad pad in pads) 
			{
				if (!pad.isHeld && Vector2.Distance(pad.position, finger.position) <= pad.radius)
				{
					pad.Hold(finger);
				}
			}
		}
	}
}
