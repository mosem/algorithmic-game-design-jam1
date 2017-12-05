﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Flight {
	public class BoardManager : MonoBehaviour {

	public enum Region{Center, TopLeft, Top, TopRight, Right, BottomRight, Bottom, BottomLeft, Left, OutOfBounds};

	private enum OutOfBoundsRegion{TopLeft, Top, TopRight, Right, BottomRight, Bottom, BottomLeft, Left, InBounds};

	public Object[] landmarks;
	public GameObject[] players;
	public GameObject footprint;
	public int nLandmarks = 16;
	[SerializeField] private static float boardWidth = 40.0f;
	[SerializeField] private static float boardHeight = 40.0f;
	[SerializeField] private static float boardPadding = 5.0f;
	[SerializeField] private float footprintsDelay = 10.0f;

	private List<GameObject>[] playersClones;
	private List<GameObject>[] playersFootprints;
	public Region[] playersRegion;

		private Vector3 shiftHorizontalVec = new Vector3(boardWidth,0);
		private Vector3 shiftVerticalVec = new Vector3(0,boardHeight);
	
	// Use this for initialization
	void Start () {
		InitLandmarks();
		players = GameObject.FindGameObjectsWithTag("Player");
		playersRegion = new Region[players.Length];
		playersClones = new List<GameObject>[players.Length];
		for (int i = 0; i < players.Length; i++)
		{
			playersRegion[i] = CheckRegion(players[i]);
			playersClones[i] = new List<GameObject>();
		}
		footprint = Resources.Load("footprint", typeof(GameObject)) as GameObject;
	}
	
	// Update is called once per frame
	void Update () {
			updatePlayers();
	}

	public void AddFootprint(GameObject footprint, Vector3 position, float lifetime)
	{
		GameObject playerFootprint = Instantiate(footprint, position, Quaternion.identity);
		Destroy(playerFootprint, lifetime);
	}
			
	private void InitLandmarks() {
		landmarks = Resources.LoadAll("landmarks", typeof(GameObject));
		float localGridWidth = boardWidth/Mathf.Sqrt((float)nLandmarks);
		float localGridHeight = boardHeight/Mathf.Sqrt((float)nLandmarks);
		float localRadius = Mathf.Min(localGridWidth, localGridHeight)/2.0f;
		for(int i = 0; i < Mathf.Sqrt((float)nLandmarks); i++) {
			float localLeftBorder = i*localGridWidth;
			for (int j = 0; j < Mathf.Sqrt((float)nLandmarks); j++) {
					float localBottomBorder = j*localGridHeight;
					Vector3 rand_pos = new Vector3(Random.RandomRange(localLeftBorder-boardWidth/2,localLeftBorder+localGridWidth-boardWidth/2), Random.RandomRange(localBottomBorder-boardWidth/2,localBottomBorder + localGridHeight - boardWidth/2));
					GameObject obj = Instantiate(landmarks[Random.Range(0,landmarks.Length)], rand_pos,  Quaternion.identity) as GameObject;
					CloneGameObject(obj, CheckRegion(obj));
					obj.transform.SetParent(transform);
			}
			
		}
	}

	private void updatePlayers() 
	{
			for (int playerIdx =0; playerIdx < players.Length; playerIdx++) 
			{
				UpdatePlayerRegion(playerIdx);
			}
	}
	
	
	private void UpdatePlayerRegion(int playerIdx)
	{
		Region playerNewRegion = CheckRegion(players[playerIdx]);
		if (playerNewRegion != playersRegion[playerIdx]) // if region changed - update player's region and clones (and position if needed)
		{
				playersRegion[playerIdx] = playerNewRegion;
				DestroyAndRemoveClones(playerIdx); // destroy and clear all current clones
			if (playerNewRegion == Region.OutOfBounds) // if out of bounds telleport player
			{
					TelleportPlayer(players[playerIdx]);
			}
			else if (playerNewRegion != Region.Center) // if not in center create new clones according to new region
			{
					playersClones[playerIdx].AddRange(CloneGameObject(players[playerIdx], playerNewRegion));
			}
		}
	}

	// Destroy clones' GameObjects and remove clones from list
	private void DestroyAndRemoveClones(int playerIdx)
	{
		foreach (GameObject clone in playersClones[playerIdx])
		{
			Destroy(clone);
		}
		playersClones[playerIdx].Clear();
	}

	private void TelleportPlayer(GameObject player) {
		OutOfBoundsRegion playerNewRegion = CheckOutOfBoundsRegion(player);
		switch (playerNewRegion) 
		{
		case OutOfBoundsRegion.TopLeft:
			player.transform.position += shiftHorizontalVec - shiftVerticalVec;
			break;
		case OutOfBoundsRegion.Top:
			player.transform.position -= shiftVerticalVec;
			break;
		case OutOfBoundsRegion.TopRight:
			player.transform.position += -shiftHorizontalVec - shiftVerticalVec;
			break;
		case OutOfBoundsRegion.Right:
			player.transform.position -= shiftHorizontalVec;
			break;
		case OutOfBoundsRegion.BottomRight:
			player.transform.position += -shiftHorizontalVec + shiftVerticalVec;
			break;
		case OutOfBoundsRegion.Bottom:
			player.transform.position += shiftVerticalVec;
			break;
		case OutOfBoundsRegion.BottomLeft:
			player.transform.position += shiftHorizontalVec + shiftVerticalVec;
			break;
		case OutOfBoundsRegion.Left:
			player.transform.position += shiftHorizontalVec;
			break;
		default:
			break;
		}
	}
			
	private Region CheckRegion(GameObject obj)
	{
			if (obj.transform.position.x < -boardWidth/2.0f || obj.transform.position.x > boardWidth/2.0f 
				|| obj.transform.position.y < -boardHeight/2.0f || obj.transform.position.y > boardHeight/2.0f) // object is out of bounds (maybe it's a clone)
			{
				return Region.OutOfBounds;
			}
			if (obj.transform.position.x <= boardPadding - boardWidth/2.0f) // object is in left margin
			{
				if (obj.transform.position.y <= boardPadding - boardHeight/2.0f) // object is in bottom left corner
				{
					return Region.BottomLeft;
				}
				if (obj.transform.position.y >= boardHeight/2.0f - boardPadding) // object is in top left corner
				{
					return Region.TopLeft;
				}
				return Region.Left;
			}
			if (obj.transform.position.x >= boardWidth/2.0f - boardPadding) // object is in right margin
			{
				if (obj.transform.position.y <= boardPadding - boardHeight/2.0f) // object is in bottom right corner
				{
					return Region.BottomRight;
				}
				if (obj.transform.position.y >= boardHeight/2.0f - boardPadding) // object is in top right corner 
				{
					return Region.TopRight;
				}
				return Region.Right;
			}
			if (obj.transform.position.y <= boardPadding - boardHeight/2.0f) // object is in bottom margin but not in corners
			{
				return Region.Bottom;
			}
			if (obj.transform.position.y >= boardHeight/2.0f - boardPadding) // object is in top margin but not in corners
			{
				return Region.Top;
			}
			return Region.Center; //object is in center: not inside padding region
	}

	private OutOfBoundsRegion CheckOutOfBoundsRegion(GameObject obj) {
		if (obj.transform.position.x < -boardWidth/2.0f) // object is in left out of bounds
		{
			if (obj.transform.position.y < -boardHeight/2.0f) // object is in bottom-left out of bounds
			{
				return OutOfBoundsRegion.BottomLeft;
			}
			if (obj.transform.position.y > boardHeight/2.0f) // object is in top-left out of bounds
			{
				return OutOfBoundsRegion.TopLeft;
			}
			return OutOfBoundsRegion.Left;
		}
		if (obj.transform.position.x > boardWidth/2.0f) // object is in right out of bounds
		{
			if (obj.transform.position.y < -boardHeight/2.0f) // object is in bottom-right out of bounds
			{
				return OutOfBoundsRegion.BottomRight;
			}
			if (obj.transform.position.y > boardHeight/2.0f) // object is in top-right out of bounds
			{
				return OutOfBoundsRegion.TopRight;
			}
			return OutOfBoundsRegion.Right;
		}
		if (obj.transform.position.y < -boardHeight/2.0f) // object is in bottom out of bounds but not in corners
		{
			return OutOfBoundsRegion.Bottom;
		}
		if (obj.transform.position.y > boardHeight/2.0f) // object is in top out of bounds but not in corners
		{
			return OutOfBoundsRegion.Top;
		}
		return OutOfBoundsRegion.InBounds;
	}
	
		private List<GameObject> CloneGameObject(GameObject obj, Region regionOfObj) {
			List<GameObject> clones = new List<GameObject>();
			if (regionOfObj == Region.Left || regionOfObj == Region.TopLeft || regionOfObj == Region.BottomLeft) // object is in left margin
			{
				//clone object and shift right
				Vector3 shiftedRightPos = obj.transform.position + shiftHorizontalVec;
				GameObject shiftedRightObj = Instantiate(obj, shiftedRightPos,  Quaternion.identity) as GameObject;;
				shiftedRightObj.transform.SetParent(transform);
				clones.Add(shiftedRightObj);
				if (regionOfObj == Region.BottomLeft) // object is in bottom left corner
				{
					// clone object and shift up and right
					Vector3 shiftedUpAndRightPos = obj.transform.position + shiftVerticalVec + shiftVerticalVec;
					GameObject shiftedUpAndRightObj = Instantiate(obj,shiftedUpAndRightPos,  Quaternion.identity) as GameObject;;
					shiftedUpAndRightObj.transform.SetParent(transform);
					clones.Add(shiftedUpAndRightObj);
				}
				else if (regionOfObj == Region.TopLeft) // object is in top left corner
				{
					// clone object and shift down and right
					Vector3 shiftedDownAndRightPos = obj.transform.position + shiftHorizontalVec - shiftVerticalVec;
					GameObject shiftedDownAndRightObj = Instantiate(obj,shiftedDownAndRightPos,  Quaternion.identity) as GameObject;;
					shiftedDownAndRightObj.transform.SetParent(transform);
					clones.Add(shiftedDownAndRightObj);
				}
			}
			if (regionOfObj == Region.Right || regionOfObj == Region.TopRight || regionOfObj == Region.BottomRight) // object is in right margin
			{
				// clone object and shift left
				Vector3 shiftedLeftPos = obj.transform.position - shiftHorizontalVec;
				GameObject shiftedLeftObj = Instantiate(obj, shiftedLeftPos, Quaternion.identity) as GameObject;;
				shiftedLeftObj.transform.SetParent(transform);
				clones.Add(shiftedLeftObj);
				if (regionOfObj == Region.BottomRight) // object is in bottom right corner
				{
					// clone object and shift up and left
					Vector3 shiftedUpAndLeftPos = obj.transform.position - shiftHorizontalVec + shiftVerticalVec;
					GameObject shiftedUpAndLeftObj = Instantiate(obj,shiftedUpAndLeftPos, Quaternion.identity) as GameObject;;
					shiftedUpAndLeftObj.transform.SetParent(transform);
					clones.Add(shiftedUpAndLeftObj);
				}
				else if (regionOfObj == Region.TopRight) // object is in top right corner 
				{
					// clone object and shift down and left
					Vector3 shiftedDownAndLeftPos = obj.transform.position - shiftVerticalVec - shiftVerticalVec;
					GameObject shiftedDownAndLeftObj = Instantiate(obj,shiftedDownAndLeftPos, Quaternion.identity) as GameObject;;
					shiftedDownAndLeftObj.transform.SetParent(transform);
					clones.Add(shiftedDownAndLeftObj);
				}
			}
			if (regionOfObj == Region.Bottom || regionOfObj == Region.BottomLeft || regionOfObj == Region.BottomRight) // object is in bottom margin (can be cloned previously horizontally and diagnoaly)
			{
				// clone object and shift up
				Vector3 shiftedUpPos = obj.transform.position + shiftVerticalVec;
				GameObject shiftedUpObj = Instantiate(obj,shiftedUpPos, Quaternion.identity) as GameObject;;
				shiftedUpObj.transform.SetParent(transform);
				clones.Add(shiftedUpObj);
			}
			if (regionOfObj == Region.Top || regionOfObj == Region.TopLeft || regionOfObj == Region.TopRight) // object is in top margin (can be cloned previously horizontally and diagnoaly)
			{
				// clone object and shift down
				Vector3 shiftedDownPos = obj.transform.position - shiftVerticalVec;
				GameObject shiftedDownObj = Instantiate(obj,shiftedDownPos, Quaternion.identity) as GameObject;;
				shiftedDownObj.transform.SetParent(transform);
				clones.Add(shiftedDownObj);
			}
			return clones;
	}
}
}

