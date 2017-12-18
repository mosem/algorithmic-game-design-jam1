using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Flight {
	public class EnemyCollisionDetector : MonoBehaviour {
		public BoardManager boardManager;

		// Use this for initialization
		void Start () {
			boardManager = GameObject.FindGameObjectWithTag("Board").GetComponent<BoardManager>();
		}

		// Update is called once per frame
		void Update () {

		}

		void OnTriggerEnter2D(Collider2D other) {
			if (other.tag == "Footprint")
			{
				boardManager.DestroyFootprint(other.gameObject);
			}
		}
	}

}
