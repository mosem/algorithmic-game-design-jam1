using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	[SerializeField] public Camera mainCam;
	[SerializeField] public Canvas winCanvas;
	[SerializeField] public Canvas loseCanvas;
	// Use this for initialization
	void Start () {
		mainCam.gameObject.SetActive(false);
		winCanvas.gameObject.SetActive(false);
		loseCanvas.gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Win()
	{
		mainCam.gameObject.SetActive(true);
		winCanvas.gameObject.SetActive(true);
	}

	public void Lose()
	{
		mainCam.gameObject.SetActive(true);
		loseCanvas.gameObject.SetActive(true);
	}
		
}
