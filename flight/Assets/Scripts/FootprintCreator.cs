using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Flight
{
	public class FootprintCreator : MonoBehaviour {

		private Vector2 previousPosition;

		public GameObject footprint;

		public BoardManager board;

		[SerializeField] private float footprintSpacing = .5f;
		[SerializeField] private float footprintLifetime = 10.0f;

		private Rigidbody2D body;
		[SerializeField] private Vector2 footprintOffset = new Vector2(0,-0.4f);

		// Use this for initialization
		void Start () {
			body = GetComponent<Rigidbody2D>();
			previousPosition = body.position;
		}

		// Update is called once per frame
		void Update () {
			Invoke("AddFootprint", 1.0f);
		}

		public void AddFootprint(){
			if ((body.position - previousPosition).magnitude > footprintSpacing)
			{
				board.AddFootprint(footprint,body.position + footprintOffset, footprintLifetime);
				previousPosition = body.position;
			}
		}

	}
}

