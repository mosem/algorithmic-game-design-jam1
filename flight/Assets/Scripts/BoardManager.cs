using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Flight {
	public class BoardManager : MonoBehaviour {

		public enum Region{Center, TopLeft, Top, TopRight, Right, BottomRight, Bottom, BottomLeft, Left, OutOfBounds};

		private enum OutOfBoundsRegion{TopLeft, Top, TopRight, Right, BottomRight, Bottom, BottomLeft, Left, InBounds};

		public Object[] landmarks;
		public List<GameObject> players;
		public List<GameObject> enemies;
		[SerializeField] public float boardWidth = 80.0f;
		[SerializeField] public float boardHeight = 80.0f;
		[SerializeField] public float boardPadding = 5.0f;
		[SerializeField] private int nLandmarks = 16;
//		[SerializeField] private float footprintsDelay = 10.0f;
		[SerializeField] private float initialDistanceThreshold = 7.5f;
		[SerializeField] private int nTriesToPlacePlayer = 10;
		[SerializeField] private string LANDMARK_LAYER = "Game";

		private List<List<GameObject>> playersClones;
		public List<GameObject> footprints;
		private List<List<GameObject>> enemiesClones;

		private List<Region> playersRegion;
		private List<Region> enemiesRegion;

		private Vector3 shiftHorizontalVec;
		private Vector3 shiftVerticalVec;

		public GameObject enemy;                // The enemy prefab to be spawned.
		public float spawnTime = 10f;            // How long between each spawn.


		// Use this for initialization
		void Start () {
			InitLandmarks();
			shiftHorizontalVec = new Vector3(boardWidth,0);
			shiftVerticalVec = new Vector3(0,boardHeight);
			players = new List<GameObject>();
			players.AddRange(GameObject.FindGameObjectsWithTag("Player"));
			playersRegion = new List<Region>(players.Count);
			playersClones = new List<List<GameObject>>(players.Count);
			for (int playerIdx = 0; playerIdx < players.Count; playerIdx++)
			{
				players[playerIdx].transform.position = GetRandomPositionOnBoard(playerIdx);
				Region playerRegion = CheckRegion(players[playerIdx]);
				playersRegion.Add(playerRegion);
				playersClones.Add(new List<GameObject>());
				if (playerRegion != Region.Center) // if not in center create new clones according to new region
				{
					playersClones[playerIdx].AddRange(CloneGameObject(players[playerIdx], playerRegion));
				}
			}
			enemies = new List<GameObject>();
			enemiesRegion = new List<Region>();
			enemiesClones = new List<List<GameObject>>();
			InvokeRepeating ("SpawnEnemy", spawnTime, spawnTime);
			footprints = new List<GameObject>();
		}

		// Update is called once per frame
		void Update () {
				updateAgents();
		}

		public List<Vector3> GetPlayersPositionsInRadius(Vector3 center, float radius)
		{
			List<Vector3> positions = new List<Vector3>();
			foreach (GameObject player in players) 
			{
				if ((player.transform.position - center).sqrMagnitude < Mathf.Pow(radius,2.0f))
				{
					positions.Add(player.transform.position);
				}

			}
			foreach (List<GameObject> clones in playersClones)
			{
				foreach (GameObject clone in clones)
				{
					if ((clone.transform.position - center).sqrMagnitude < Mathf.Pow(radius,2.0f))
					{
						positions.Add(clone.transform.position);
					}

				}

			}
			return positions;
		}

		public List<Vector3> GetFootprintsPositionsInRadius(Vector3 center, float radius)
		{
			List<Vector3> positions = new List<Vector3>();

			foreach (GameObject footprint in footprints)
			{
				if (footprint != null && (footprint.transform.position - center).sqrMagnitude < radius)
				{
					positions.Add(footprint.transform.position);
				}
			}

			return positions;
		}

		public void AddFootprint(GameObject footprint, Vector3 position, float lifetime)
		{
			GameObject footprintObj = Instantiate(footprint, position, Quaternion.identity);
			footprints.Add(footprintObj);
//			Destroy(footprintObj, lifetime);
			StartCoroutine(DestroyFootprint(footprintObj,lifetime));
		}

		private IEnumerator DestroyFootprint(GameObject footprint,float delay)
		{
			yield return new WaitForSeconds(delay);
			DestroyFootprint(footprint);
		}

		public void DestroyFootprint(GameObject footprint)
		{
			for (int i = footprints.Count-1; i > -1; i--)
			{
				

				if (footprint.GetInstanceID() == footprints[i].GetInstanceID())
				{
					footprints.RemoveAt(i);
					break; // only one instance of footprint should be in list
				}
			}
			Destroy(footprint);
		}

		void SpawnEnemy()
		{
			Vector2 position = new Vector2(Random.Range(-boardWidth/2.0f, boardWidth/2.0f), Random.Range(-boardHeight/2.0f, boardHeight/2.0f));

			// Create an instance of the enemy prefab at the randomly selected spawn point's position and rotation.
			GameObject enemyObject = Instantiate(enemy, position, Quaternion.identity);
			enemies.Add(enemyObject);
			enemiesRegion.Add(CheckRegion(enemyObject));
			enemiesClones.Add(new List<GameObject>());
		}

		private Vector3 GetRandomPositionOnBoard(int thisPlayerIdx)
		{	
			Vector3 potentialPosition = new Vector3(Random.Range(-boardWidth/2, boardWidth/2), Random.Range(-boardHeight/2, boardHeight/2));
			if (thisPlayerIdx != 0)
			{
				for (int i = 0; i < nTriesToPlacePlayer; i++)
				{
					if (checkPlayerPositionVsOtherPlayersPositions(potentialPosition, thisPlayerIdx)) 
					{
						break;
					} 
					else {
						potentialPosition = new Vector3(Random.Range(-boardWidth/2, boardWidth/2), Random.Range(-boardHeight/2, boardHeight/2));
					}
				}
			}
			return potentialPosition;
		}

		// returns true if it is far away from other players, otherwise returns false
		private bool checkPlayerPositionVsOtherPlayersPositions(Vector3 pos, int thisPlayerIdx)
		{ 
			for (int otherPlayerIdx = 0; otherPlayerIdx < thisPlayerIdx; otherPlayerIdx++)
			{
				if (Vector3.Distance(players[otherPlayerIdx].transform.position, pos) < initialDistanceThreshold){
					return false;
				}
				foreach (GameObject clone in playersClones[otherPlayerIdx])
				{
					if (Vector3.Distance(clone.transform.position, pos) < initialDistanceThreshold )
					{
						return false;
					}
				}
			}
			return true;
		}

		private void InitLandmarks() {
			landmarks = Resources.LoadAll("landmarks", typeof(GameObject));
			float localGridWidth = boardWidth/Mathf.Sqrt((float)nLandmarks);
			float localGridHeight = boardHeight/Mathf.Sqrt((float)nLandmarks);
//			float localRadius = Mathf.Min(localGridWidth, localGridHeight)/2.0f;
			for(int i = 0; i < Mathf.Sqrt((float)nLandmarks); i++) {
				float localLeftBorder = i*localGridWidth;
				for (int j = 0; j < Mathf.Sqrt((float)nLandmarks); j++) {
					float localBottomBorder = j*localGridHeight;
					Vector3 rand_pos = new Vector3(Random.Range(localLeftBorder-boardWidth/2,localLeftBorder+localGridWidth-boardWidth/2), Random.Range(localBottomBorder-boardHeight/2,localBottomBorder + localGridHeight - boardHeight/2));
					GameObject obj = Instantiate(landmarks[Random.Range(0,landmarks.Length)], rand_pos,  Quaternion.identity) as GameObject;
					CloneGameObject(obj, CheckRegion(obj));
					obj.transform.SetParent(transform);
					obj.GetComponent<SpriteRenderer>().sortingLayerName = LANDMARK_LAYER;
				}
				
			}
		}

		private void updateAgents() 
		{
			for (int playerIdx = 0; playerIdx < players.Count; playerIdx++) 
			{
				UpdateAgentRegion(playerIdx, ref players, ref playersRegion, ref playersClones);
			}
			for (int enemyIdx = 0; enemyIdx < enemies.Count; enemyIdx++)
			{
				UpdateAgentRegion(enemyIdx, ref enemies, ref enemiesRegion, ref enemiesClones);
			}
		}


		private void UpdateAgentRegion(int agentIdx, ref List<GameObject> agents, ref List<Region> agentsRegion, ref List<List<GameObject>> agentClones)
		{
			Region playerNewRegion = CheckRegion(agents[agentIdx]);
			if (playerNewRegion != agentsRegion[agentIdx]) // if region changed - update player's region and clones (and position if needed)
			{
				agentsRegion[agentIdx] = playerNewRegion;
				DestroyAndRemoveClones(agentIdx, ref agentClones); // destroy and clear all current clones
				if (playerNewRegion == Region.OutOfBounds) // if out of bounds telleport player
				{
					TeleportAgent(agents[agentIdx]);
				}
				else if (playerNewRegion != Region.Center) // if not in center create new clones according to new region
				{
					agentClones[agentIdx].AddRange(CloneGameObject(agents[agentIdx], playerNewRegion));
				}
			}
		}

		// Destroy clones' GameObjects and remove clones from list
		private void DestroyAndRemoveClones(int agentIdx, ref List<List<GameObject>> agentClones)
		{
			foreach (GameObject clone in agentClones[agentIdx])
			{
				Destroy(clone);
			}
			agentClones[agentIdx].Clear();
		}

		private void TeleportAgent(GameObject agent) {
			OutOfBoundsRegion playerNewRegion = CheckOutOfBoundsRegion(agent);
			switch (playerNewRegion) 
			{
			case OutOfBoundsRegion.TopLeft:
				agent.transform.position += shiftHorizontalVec - shiftVerticalVec;
				break;
			case OutOfBoundsRegion.Top:
				agent.transform.position -= shiftVerticalVec;
				break;
			case OutOfBoundsRegion.TopRight:
				agent.transform.position += -shiftHorizontalVec - shiftVerticalVec;
				break;
			case OutOfBoundsRegion.Right:
				agent.transform.position -= shiftHorizontalVec;
				break;
			case OutOfBoundsRegion.BottomRight:
				agent.transform.position += -shiftHorizontalVec + shiftVerticalVec;
				break;
			case OutOfBoundsRegion.Bottom:
				agent.transform.position += shiftVerticalVec;
				break;
			case OutOfBoundsRegion.BottomLeft:
				agent.transform.position += shiftHorizontalVec + shiftVerticalVec;
				break;
			case OutOfBoundsRegion.Left:
				agent.transform.position += shiftHorizontalVec;
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

