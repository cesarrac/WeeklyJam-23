using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MiniGameDifficulty {Easy, Slow, Average, Fast, Ace, Fire, Mindblown, Sauvant, Intergalactic, Master}
public class MiniGame : MonoBehaviour {

	public GameObject moving_obj, goal_obj;
	public float speed = 2;
	float move_x;
	bool isRight, isLeft;
	GameObject successPanel, failPanel;
	float startingGoalOffset = 0.1f;
	float difficultyOffset = 0;
	float target_position;
	bool isHorizontal = true;
	public delegate void OnTryClick();
	public event OnTryClick onSuccess;
	public event OnTryClick onFail;

	public void Init(MiniGameDifficulty difficulty){
		if ((int) difficulty > 0)
			difficultyOffset = (int)difficulty / 10;
		difficultyOffset = 0;
	}
	public void StartMiniGame(MachineCondition curMachineCondition){
		int minusCondition = 2 - (int)curMachineCondition;
		if (minusCondition > 0){
			for(int i = 0; i < minusCondition; i++){
				IncreaseDifficulty();
			}
		}
		Sprite mainSprite = GetComponent<SpriteRenderer>().sprite;
		if (isHorizontal){
			float goalX = Random.Range(1, mainSprite.bounds.max.x - 1);
			goal_obj.transform.localPosition = new Vector2(goalX, goal_obj.transform.localPosition.y);
		}else{
			float goalY = Random.Range(1, mainSprite.bounds.max.y - 1);
			goal_obj.transform.localPosition = new Vector2(goal_obj.transform.localPosition.y,goalY);
		}
		move_x = moving_obj.transform.localPosition.x;
		isLeft = true;
		target_position = goal_obj.transform.localPosition.x;
	}
	void Update(){
		if (Input.GetMouseButtonDown(0)){
			CheckForGoal();
		}
		if (isLeft){
			MoveRight();
			
		}else if (isRight){
			MoveLeft();
		}
	}
	void CheckForGoal(){
		float offset = startingGoalOffset - difficultyOffset;
		Debug.Log("Offset at: " + offset);

		if (move_x >= target_position - offset && move_x <= target_position + offset){
			//Debug.Log("You got it!");
			IncreaseDifficulty();
			failPanel.SetActive(false);
			successPanel.SetActive(true);
			if (onSuccess != null){
				onSuccess();
			}
		}else{
			//Debug.Log("You missed, it's at " + move_x);
			if (onFail != null){
				onFail();
			}
			successPanel.SetActive(false);
			failPanel.SetActive(true);
		}
	}
	void IncreaseDifficulty(){
		difficultyOffset += 0.01f;
		difficultyOffset = Mathf.Clamp(difficultyOffset, 0, 0.1f);
		Debug.Log("Difficulty increased to " + difficultyOffset);
	}
	void MoveRight(){
		if (moving_obj.transform.localPosition.x >= 1){
				isLeft = false;
				isRight = true;
				return;
		}
		move_x += speed * Time.deltaTime;
		moving_obj.transform.localPosition = new Vector2(move_x, moving_obj.transform.localPosition.y);
	}
	void MoveLeft(){
		if (moving_obj.transform.localPosition.x <= -1f){
				isLeft = true;
				isRight = false;
				return;
		}
		move_x -= speed * Time.deltaTime;
		moving_obj.transform.localPosition = new Vector2(move_x, moving_obj.transform.localPosition.y);
	}

	public void Deactivate(){
			successPanel.SetActive(false);
			failPanel.SetActive(false);
			isLeft = true;
			isRight = false;
			moving_obj.transform.localPosition = new Vector2(-1, 0);
			difficultyOffset = 0;
			this.gameObject.SetActive(false);
	}
}
