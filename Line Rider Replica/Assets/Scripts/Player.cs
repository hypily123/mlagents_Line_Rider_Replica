using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {

	public Rigidbody2D rb;

	private void Start()
	{
	}
	void Update ()
	{
		if (Input.GetButtonDown("Play"))
		{
			rb.bodyType = RigidbodyType2D.Dynamic;
		}
		float player_x = rb.position.x;
		float player_y = rb.position.y;
		float goal_x = GameController.getGoal_x();
		float goal_y = GameController.getGoal_y();

		if(Mathf.Abs(player_x-goal_x) <= 0.1 && Mathf.Abs(player_y - goal_y) <= 0.6)
		{
			Debug.Log(Mathf.Abs(player_y - goal_y));
			Debug.Log("gotcha");
			UnityEditor.EditorApplication.isPlaying = false;
		}

		if(player_x >= 14 || player_y <= -9)
		{
			Debug.Log("failed");
			UnityEditor.EditorApplication.isPlaying = false;
		}
	}

}
