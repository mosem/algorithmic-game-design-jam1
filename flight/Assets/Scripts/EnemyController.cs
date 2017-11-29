using UnityEngine;

namespace Flight {
[CreateAssetMenu(menuName = "Controller/EnemyController")]
public class EnemyController : Controller {

	public override Vector2 GetInputVelocity ()
	{
		return new Vector2(Random.Range(-10, 10), Random.Range(-10, 10));
	}

}

}