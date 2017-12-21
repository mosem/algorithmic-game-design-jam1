using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Flight{
public class EnemySenses : MonoBehaviour {

		private BoardManager boardManager;
		[SerializeField] private float VISION_THRESH = 15.0f;
		[SerializeField] private float SMELL_THRESH = 20.0f;
	//	public GameObject[] players;
	//	public Vector2[] headings;

	//	private Rigidbody2D body;

	

		// Use this for initialization
		void Start () 
		{
	//		players = GameObject.FindGameObjectsWithTag("Player");
	//		body = GetComponent<Rigidbody2D>();
				boardManager = GameObject.FindGameObjectWithTag("Board").GetComponent<BoardManager>();
		}

		void Update ()
		{
	//		GameObject[] tmp = GameObject.FindGameObjectsWithTag("Player"); // include new clones and remove destroyed clones
	//			if (tmp.Length > 0)
	//			{
	//				players = tmp;
	//			}
		}

		public Vector3 GetHeading()
		{
			List<Vector3> playerPositions = (boardManager != null) ? boardManager.GetPlayersPositionsInRadius(transform.position, SMELL_THRESH) : new List<Vector3>();
			if (playerPositions.Count != 0)
			{
//				Debug.Log("detected player");
				return GetMinHeading(playerPositions);
			}
			List<Vector3> footprintPositions = (boardManager != null) ? boardManager.GetFootprintsPositionsInRadius(transform.position, VISION_THRESH) : new List<Vector3>();
			if (footprintPositions.Count != 0)
			{
//				Debug.Log("detected footprint");
				return GetMinHeading(footprintPositions);
			}
			return new Vector3(Random.Range(-1, 1), Random.Range(-1, 1)).normalized;
		}

//		public Vector3 GetClosestPlayerHeading ()
//		{
//			List<Vector3> playerPositions = (boardManager != null) ? boardManager.GetPlayersPositionsInRadius(transform.position, VISION_THRESH) : new List<Vector3>();
//			return GetMinHeading(playerPositions);
//		}
//
//		public Vector3 GetClosestFootprintHeading()
//		{
//			List<Vector3> footprintPositions = (boardManager != null) ? boardManager.GetFootprintsPositionsInRadius(transform.position, SMELL_THRESH) : new List<Vector3>();
//			return GetMinHeading(footprintPositions);
//		}

		private Vector3 GetMinHeading(List<Vector3> positions)
		{
			float minDist = Mathf.Infinity;
			Vector3 minHeading = new Vector3(0,0,0);
			foreach (Vector3 targetPosition in positions)
			{
				Vector3 targetHeading = targetPosition - transform.position;
				float dist = targetHeading.sqrMagnitude;
				if (dist < minDist)
				{
					minDist = dist;
					minHeading = targetHeading;
				}
			}
			return minHeading.normalized;
		}
}
}
