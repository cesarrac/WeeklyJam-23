using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction { Left, Right}
public class CharacterMovement : MonoBehaviour {

	float minX, maxX;
	float minY, maxY;
	Vector2 moveVector, inputVector;
	Animator animator;
	public SpriteRenderer[] spriteRenderers;
	bool isMoving = false;
	float normalSpeed = 4;
	public float curSpeed {get; protected set;}
	float runSpeed;
	float crawlSpeed;
	bool canDash, isDashing;
	float dashTime = 0.4f;
	float timeToDash = 1f, timer = 0;
	public Direction facingDirection {get; protected set;}
	public delegate void OnMove();
	public event OnMove onStartMove;
	public event OnMove onStopMove;
	bool isLocked = false;
	void OnEnable(){
		animator = GetComponentInChildren<Animator>();
		//spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
		minX = minY = -8;
		maxX = 7;
		maxY = 3;
		facingDirection = Direction.Left;
		curSpeed = normalSpeed;
		runSpeed = curSpeed * 3;
		crawlSpeed = curSpeed * 0.5f;
	}

	public void Initialize(Vector2 minWalkPos, Vector2 maxWalkPos){
		minX = minWalkPos.x;
		minY = minWalkPos.y;
		maxX = maxWalkPos.x;
		maxY = maxWalkPos.y;
	}

	void Update(){
		if (isLocked == true)
			return;
		
		inputVector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
		if (Input.GetKeyDown(KeyCode.Space)){
			if (isMoving == false)
				return;
			curSpeed = runSpeed;
			animator.SetTrigger("dash");
			isDashing = true;
		}
		Dash();
		Move();
	}
	
	void Dash(){
		if (isDashing == false)
			return;
		if (timer >= dashTime){
			curSpeed = normalSpeed;
			isDashing = false;
			timer = 0;
		}else{
			timer += Time.deltaTime;
		}
	}
	void StartMove(){
		isMoving = true;
		animator.SetBool("isWalking", isMoving);
		if (onStartMove != null)
			onStartMove();
	}
	void Move(){
		if (inputVector == Vector2.zero){
			if (isMoving == true){
				StopMove();
			}
			
			return;
		}else {
			
			if (isMoving == false){
				StartMove();
			}
		}
	 	
		moveVector = transform.position;
		moveVector += inputVector * curSpeed * Time.deltaTime;
		ClampMovement();
		transform.position = moveVector;
		FlipSprites();
	}
	void ClampMovement(){
		if (moveVector.x > maxX){
			moveVector.x = maxX;
		}else if (moveVector.x < minX){
			moveVector.x = minX;
		}
		if (moveVector.y > maxY){
			moveVector.y = maxY;
		}else if (moveVector.y < minY){
			moveVector.y = minY;
		}
	}
	void StopMove(){
		isMoving = false;
		animator.SetBool("isWalking", isMoving);
		if (onStopMove != null)
			onStopMove();
	}
	void FlipSprites(){
		bool flip = false;
		facingDirection = Direction.Left;
		if (inputVector.x > 0){
			flip = true;
			facingDirection = Direction.Right;
		}
		// OFFSET the main sprite & Tool since it is pivoting from bottom left
		if (flip == true){
			spriteRenderers[0].transform.localPosition = Vector2.right;
			//spriteRenderers[4].transform.localPosition = Vector2.right;
		}else{
			spriteRenderers[0].transform.localPosition = Vector2.zero;
			//spriteRenderers[4].transform.localPosition = Vector2.zero;
		}
		foreach(SpriteRenderer sr in spriteRenderers){
			sr.flipX = flip;
		}
	}
	public void LockMovement(bool locked){
		isLocked = locked;
		if (locked == true)
			StopMove();
	}
}
