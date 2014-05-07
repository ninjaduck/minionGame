using UnityEngine;
using System.Collections;


[RequireComponent(typeof(PlayerPhysics))]
public class PlayerController : Entity {
	
	// Player Handling
	public float gravity = 20;
	public float walkSpeed = 8;
	public float runSpeed = 12;
	public float acceleration = 30;
	public float slideDeceleration = 10;
	public float jumpHeight = 12;
	private float prevmoveDir;

	public const int RIGHT = 1;  
	public const int LEFT = 0; 
	public int movingTowards = RIGHT;

	private float animationSpeed;

	private float currentSpeed;
	private float targetSpeed;
	private Vector2 amountToMove;
	
	private PlayerPhysics playerPhysics;

	private Vector3 eulerAngles;
	private Animator animator;

	private bool jumping;
	private bool sliding;
	private bool WallHolding;



	void Start () {
		playerPhysics = GetComponent<PlayerPhysics>();
		animator = GetComponent<Animator>();
		prevmoveDir = 0;
		transform.Rotate (Vector3.up, 90, Space.World);
	}
	
	void Update () {
		// Reset acceleration upon collision
		if (playerPhysics.movementStopped) {
			targetSpeed = 0;
			currentSpeed = 0;
		}

		// If player is touching the ground
		if (playerPhysics.grounded) {
						amountToMove.y = 0;
						
						if(WallHolding){
							WallHolding = false;
							animator.SetBool("Wall_Hold",false);
						}
						
						if (jumping) {
								jumping = false;
								animator.SetBool ("Jumping", false);
						}

						if (sliding) {
								if (Mathf.Abs (currentSpeed) < 0.25f) {
										sliding = false;
										animator.SetBool ("Sliding", false);
										playerPhysics.ResetCollider ();
								}

						}

						

						if (Input.GetButtonDown ("Slide")) {

								sliding = true;
								animator.SetBool ("Sliding", true);
								playerPhysics.SetCollider (new Vector3 (0.5f, 0.5f, 1), new Vector3 (0, 0.25f, 0.15f));
								targetSpeed = 0;
						}
				} 

		else {
			if(!WallHolding){
				if(playerPhysics.canWallHold){
					WallHolding = true;
					animator.SetBool("Wall_Hold",true);
				}
			}
		}

		// Jump
		if (Input.GetButtonDown ("Jump")) {



			if(playerPhysics.grounded || WallHolding){
				amountToMove.y = jumpHeight;
				jumping = true;
				animator.SetBool ("Jumping", true);


			}

			if(WallHolding){
				WallHolding = false;
				animator.SetBool("Wall_Hold",false);
			}
		}
		animationSpeed = IncrementTowards (animationSpeed, Mathf.Abs (targetSpeed), acceleration);

		animator.SetFloat ("Speed", Mathf.Abs(animationSpeed));
		// Input
		if (!sliding) {
						float speed = Input.GetButton ("Run") ? runSpeed : walkSpeed;
						targetSpeed = Input.GetAxisRaw ("Horizontal") * speed;
						currentSpeed = IncrementTowards (currentSpeed, targetSpeed, acceleration);


						//Face Direction
						float moveDir = Input.GetAxisRaw ("Horizontal");
						//Debug.Log (moveDir);
//						if (moveDir != 0) {
//								if ((prevmoveDir == 0 && moveDir == 1) && movingTowards == LEFT) {
//										transform.Rotate (Vector3.up, 180, Space.World);
//					
//										movingTowards = RIGHT;
//								} else if ((prevmoveDir == 0 && moveDir == -1) && movingTowards == RIGHT) {
//										transform.Rotate (Vector3.up, 180, Space.World);
//					
//										movingTowards = LEFT;
//								}
//				
//								//transform.eulerAngles = (moveDir>0)?Vector3.zero : Vector3.up;
//								//transform.Rotate(eulerAngles,Space.World);
//				
//						}	
//						prevmoveDir = moveDir;

						if (moveDir != 0 && !WallHolding) {
												if ((moveDir == 1) && movingTowards == LEFT) {
														transform.Rotate (Vector3.up, 180, Space.World);
									
														movingTowards = RIGHT;
												} else if ((moveDir == -1) && movingTowards == RIGHT) {
														transform.Rotate (Vector3.up, 180, Space.World);
									
														movingTowards = LEFT;
												}
								
												//transform.eulerAngles = (moveDir>0)?Vector3.zero : Vector3.up;
												//transform.Rotate(eulerAngles,Space.World);
								
										}	
										//prevmoveDir = moveDir;

				} 

		else {
			currentSpeed = IncrementTowards (currentSpeed, targetSpeed, slideDeceleration);
		}

		// Set amount to move
		amountToMove.x = currentSpeed;

		if (WallHolding) {
			amountToMove.x = 0;
			if(Input.GetAxisRaw("Vertical")!= -1){
				amountToMove.y = 0;
			}
		}
		amountToMove.y -= gravity * Time.deltaTime;
		playerPhysics.Move(amountToMove * Time.deltaTime);

		

	}
	
	// Increase n towards target by speed
	private float IncrementTowards(float n, float target, float a) {
		if (n == target) {
			return n;	
		}
		else {
			float dir = Mathf.Sign(target - n); // must n be increased or decreased to get closer to target
			n += a * Time.deltaTime * dir;
			return (dir == Mathf.Sign(target-n))? n: target; // if n has now passed target then return target, otherwise return n
		}
	}
}
