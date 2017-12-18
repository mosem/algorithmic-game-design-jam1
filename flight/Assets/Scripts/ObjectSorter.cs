using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Flight{
	public class ObjectSorter : MonoBehaviour {

		// Use this for initialization
		void Start () {
			
		}

		// Update is called once per frame
		void Update () {

		}

		void LateUpdate()
		{
			GetComponent<SpriteRenderer>().sortingOrder = -(int)(transform.position.y * 100);
		}
	}
}

