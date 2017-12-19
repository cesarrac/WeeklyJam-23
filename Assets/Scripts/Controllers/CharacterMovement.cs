using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour {

	int minX, maxX;
	int minY, maxY;
	Vector2 moveVector, inputVector;
	Animator animator;
	SpriteRenderer[] spriteRenderers;
	bool isMoving = false;
	float speed = 3;
	void Awake(){
		animator = GetComponentInChildren<Animator>();
		spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
		minX = minY = -8;
		maxX = 7;
		maxY = 3;
	}

	void Update(){
		inputVector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
		Move();
	}
	void StartMove(){
		isMoving = true;
		animator.SetBool("isWalking", isMoving);
	}
	void Move(){
		if (isMoving == false){
			StartMove();
		}
	 	if (inputVector == Vector2.zero){
			StopMove();
			return;
		} 
		moveVector = transform.position;
		moveVector += inputVector * speed * Time.deltaTime;
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
	}
	void FlipSprites(){
		bool flip = false;
		if (inputVector.x > 0){
			flip = true;
		}
		// OFFSET the main sprite since it is pivoting from bottom left
		if (flip == true){
			spriteRenderers[0].transform.localPosition = Vector2.right;
		}else{
			spriteRenderers[0].transform.localPosition = Vector2.zero;
		}
		foreach(SpriteRenderer sr in spriteRenderers){
			sr.flipX = flip;
		}
	}
}
