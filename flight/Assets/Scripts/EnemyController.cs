using UnityEngine;

namespace Flight {

	public class EnemyController : Controller {

		private float DAMP_FACT = 0.5f;
		
		[SerializeField] EnemySenses senses;


		public override Vector2 GetInputVelocity ()
		{
			return senses.GetHeading();
		}

	}

}