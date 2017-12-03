using System.Collections;
using UnityEngine;

namespace Flight {
class GridMove : MonoBehaviour {

	enum Direction {Idle, North, East, South, West};

	private Rigidbody2D body;
	
	private Animator animator;

    private Vector2 initialPosition;

    private Vector2 inputVelocity;

	public Controller controller;

	public bool facingRight;

	public int dir;

	protected void Awake() {
        body = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
        initialPosition = body.position;
		facingRight = true;
    }

	protected void Update ()
	{

        // Handle keyboard input.
//		inputVelocity = new Vector2(Input.GetAxis(horizontalCtrl), Input.GetAxis(verticalCtrl));
		inputVelocity = controller.GetInputVelocity();

		dir = 0;
		if (inputVelocity.x != 0) {

			dir = inputVelocity.x > 0 ? (int)Direction.East : (int)Direction.West;

		}
		else if (inputVelocity.y != 0) {
			dir = inputVelocity.y > 0 ? (int)Direction.North : (int)Direction.South;
		}
		animator.SetInteger("dir", dir);
		
    }


	protected void FixedUpdate() {

		body.velocity = inputVelocity;
		
    }

	void Flip()
	{
		// Switch the way the player is labelled as facing
		facingRight = !facingRight;

		// Multiply the player's x local scale by -1
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

}
}