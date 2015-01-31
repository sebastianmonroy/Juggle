using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum AnimationType
{
	ChildVsChild,
	ChildVsMother,
	ChildVsWall,
	MotherVsWall
}

public class Animation
{
	public AnimationType type;
	public Vector3 position;

	public Animation(AnimationType type, Vector2 position)
	{
		this.type = type;
		this.position = new Vector3(position.x, position.y, 0f);
	}
}

public class AnimationHandler : MonoBehaviour
{
	public static AnimationHandler instance;
	private Queue<Animation> animationQueue = new Queue<Animation>();

	public GameObject ChildVsMotherPrefab;

	void Start ()
	{
		instance = this;
	}
	
	void Update ()
	{
		if (animationQueue.Count > 0)
		{
			Animation nextAnimation = animationQueue.Dequeue();

			switch (nextAnimation.type)
			{
				case AnimationType.ChildVsMother:
					GameObject childvsmother = Instantiate(ChildVsMotherPrefab, nextAnimation.position, Quaternion.identity) as GameObject;
					Destroy(childvsmother, 1.0f);
					break;
				default:
					break;
			}
		}
	}

	public void ChildOnMotherCollision(Pad mother, Pad child)
	{
		Vector2 collisionPosition = mother.GetPosition() + mother.GetRadius()/2f * (child.GetPosition() - mother.GetPosition()).normalized; 

		Animation newAnimation = new Animation(AnimationType.ChildVsMother, collisionPosition);
		animationQueue.Enqueue(newAnimation);
	}
}
