using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Flight{
	public class PlayerCollisionDetector : MonoBehaviour {
		private GameManager gameManager;
		private GridMove gridMove;
		// Use this for initialization
		void Start () {
			gameManager = GetComponentInParent<GameManager>();
			gridMove = GetComponent<GridMove>();
		}

		// Update is called once per frame
		void Update () {

		}

		void OnTriggerEnter2D(Collider2D other) {

			if (other.gameObject.tag == "Player")
			{
				gameManager.Win();
			}
			else if (other.tag == "Enemy")
			{
				gridMove.isAlive = false;
				gameManager.Lose();
			}
		}
	}
}

