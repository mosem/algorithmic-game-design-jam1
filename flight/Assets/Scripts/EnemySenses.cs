using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Flight{
public class EnemySenses : MonoBehaviour {

	public GameObject[] players;
	public Vector2[] headings;

	private Rigidbody2D body;

	// Use this for initialization
	void Start () 
	{
		players = GameObject.FindGameObjectsWithTag("Player");
		body = GetComponent<Rigidbody2D>();
	}

	public Vector2 GetClosestPlayerHeading ()
	{
		headings = new Vector2[players.Length];
		int minIdx = 0;
		for (int i = 0; i < players.Length; i++) 
		{
			Rigidbody2D playerBody = players[i].GetComponent<Rigidbody2D>();
			headings[i] = playerBody.position - body.position;
			if (i > 0)
			{
				if (headings [i].sqrMagnitude < headings [minIdx].sqrMagnitude) 
				{
					minIdx = i;
				}
			}
		}

		return headings[minIdx];
	}
}
}
