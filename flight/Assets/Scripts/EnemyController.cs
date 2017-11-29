using UnityEngine;

namespace Flight {
[CreateAssetMenu(menuName = "Controller/EnemyController")]

public class EnemyController : Controller {

	private float VISION_THRESH = 3.0f;
	private float DAMP_FACT = 0.5f;
	
	[SerializeField] EnemySenses senses;


	public override Vector2 GetInputVelocity ()
	{
		Vector2 closestEnemyHeading = senses.GetClosestPlayerHeading();
		Vector2 inputVelocity =  (closestEnemyHeading.magnitude <= VISION_THRESH) ? closestEnemyHeading 
					: new Vector2(Random.Range(-10, 10), Random.Range(-10, 10));
			return DAMP_FACT*inputVelocity;
	}

}

}