using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Flight {
	public class BoardManager : MonoBehaviour {
	public Object[] landmarks;	
	// Use this for initialization
	void Start () {
		landmarks = Resources.LoadAll("landmarks", typeof(GameObject));
		Debug.Log("landmarks length");
		Debug.Log(landmarks.Length);
		// foreach(Object obj in landmark_objs)
		// {
		// 	landmarks.Add(Instantiate(GameObject(obj)));
		// }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
}

