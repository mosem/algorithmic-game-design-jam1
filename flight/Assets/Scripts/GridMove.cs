using System.Collections;
using UnityEngine;

namespace Flight {
class GridMove : MonoBehaviour {


	private Rigidbody2D body;

    private Vector2 initialPosition;

    private Vector2 inputVelocity;

	public Controller controller;

	protected void Awake() {
        body = GetComponent<Rigidbody2D>();
        initialPosition = body.position;
    }

	protected void Update ()
	{

        // Handle keyboard input.
//		inputVelocity = new Vector2(Input.GetAxis(horizontalCtrl), Input.GetAxis(verticalCtrl));
		inputVelocity = controller.GetInputVelocity();
		Debug.Log(inputVelocity.ToString());

    }


	protected void FixedUpdate() {

		body.velocity = inputVelocity;
        
    }

}
}