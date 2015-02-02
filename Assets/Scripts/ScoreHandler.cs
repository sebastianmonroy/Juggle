using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ScoreHandler : MonoBehaviour
{
	public static ScoreHandler instance;

	public Text p1ScoreText;
	private int p1Score;
	public Text p2ScoreText;
	private int p2Score;

	public void Start()
	{
		instance = this;
	}

	public void PlayerScored(int i)
	{
		if (i == 1)
		{
			p1Score++;
			p1ScoreText.text = "" + p1Score;
		}
		else if (i == 2)
		{
			p2Score++;
			p2ScoreText.text = "" + p2Score;
		}

		if (p1Score > 9 || p2Score > 9)
		{
			Application.LoadLevel(Application.loadedLevel);
		}
	}
}
