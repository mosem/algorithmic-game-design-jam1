﻿using System.Collections;
using UnityEngine;

namespace Flight {
class GridMove : MonoBehaviour {

	enum Direction {Idle, North, East, South, West};

	private Rigidbody2D body;
	
	private Animator animator;

//    private Vector2 initialPosition;

    private Vector2 inputVelocity;

	public Controller controller;

	[SerializeField] private bool facingRight;

	[SerializeField] public bool isAlive;
	
	[SerializeField] private float speed = 1.0f;

//	[SerializeField] private float horizontalLimit = 20;

//	[SerializeField] private float verticalLimit = 20;


	public int dir;

	protected void Awake() {
        body = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
//        initialPosition = body.position;
		facingRight = true;
		isAlive = true;
		dir = 0;
    }

	protected void Update ()
	{

        // Handle keyboard input.
//		inputVelocity = new Vector2(Input.GetAxis(horizontalCtrl), Input.GetAxis(verticalCtrl));
		inputVelocity = isAlive ? controller.GetInputVelocity() : new Vector2(0,0);

		// Set direction for Animator
		dir = 0;
		if (inputVelocity.x != 0) {
			dir = inputVelocity.x > 0 ? (int)Direction.East : (int)Direction.West;
		}
		else if (inputVelocity.y != 0) {
			dir = inputVelocity.y > 0 ? (int)Direction.North : (int)Direction.South;
		}
		animator.SetInteger("dir", dir);
		if (dir == (int)Direction.West)
		{
			facingRight = false;
			transform.localRotation = Quaternion.Euler(0, 180, 0);
		}
			else if (dir == (int)Direction.East)
		{
			facingRight = true;
			transform.localRotation = Quaternion.Euler(0, 0, 0);
		}

		

//		float horizontal = inputVelocity.x * speed * Time.deltaTime;
//		float vertical = inputVelocity.y * speed * Time.deltaTime;
//		Vector3 pos = transform.position;
//		pos.x = Mathf.Clamp(pos.x + horizontal, -horizontalLimit, horizontalLimit);
//		pos.y = Mathf.Clamp(pos.y + vertical, -verticalLimit, verticalLimit);
//		transform.position = pos;
    }


	protected void FixedUpdate() {

		body.velocity = inputVelocity*speed;
		
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