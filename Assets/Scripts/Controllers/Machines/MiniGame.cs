using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MiniGameDifficulty {Easy, Slow, Average, Fast, Ace, Fire, Mindblown, Sauvant, Intergalactic, Master}
public class MiniGame : MonoBehaviour {
	public RectTransform lineTransform;
	public GameObject moving_obj, goal_obj;
	public float speed = 2;
	float move_x;
	public bool isRight, isLeft;
	float startingGoalOffset = 9f;
	float difficultyOffset = 0;
	float target_position;
	bool isHorizontal = true;
	bool canMove = false;
	public delegate void OnGame();
	public event OnGame onGameSuccess;
	public event OnGame onGameFail;
	float left, right;
	public void StartMiniGame(MachineCondition curMachineCondition, MiniGameDifficulty difficulty){
		// The starting difficulty based on machine's efficiency
		difficultyOffset = 0;
		if ((int) difficulty > 0)
			difficultyOffset = (int)difficulty;
		// The difficulty offset based on current condition of machine
		int minusCondition = 2 - (int)curMachineCondition;
		if (minusCondition > 0){
			for(int i = 0; i < minusCondition; i++){
				IncreaseDifficulty();
			}
		}

		// Set speed of moving obj according to difficulty
		speed = (int)difficulty > 0 ? (int)difficulty * 100 : 100;
		
		if (isHorizontal){
			float lineWidth = lineTransform.sizeDelta.x * 0.5f;
			left = -lineWidth + 2;
			right = lineWidth - 2;
			Debug.Log("MINIGAME: left = " + left + " right = " + right);
			float goalX = Random.Range(left, right);
			goal_obj.transform.localPosition = new Vector2(goalX, goal_obj.transform.localPosition.y);
		}else{
			float lineHeight = lineTransform.sizeDelta.y * 0.5f;
			float goalY = Random.Range(-lineHeight + 2, lineHeight - 2);
			goal_obj.transform.localPosition = new Vector2(goal_obj.transform.localPosition.y,goalY);
		}
		move_x = moving_obj.transform.localPosition.x;
		isLeft = true;
		target_position = goal_obj.transform.localPosition.x;

		// scale goal and moving obj to match difficulty
		float xScale = difficultyOffset > 0 ? difficultyOffset / 10 : startingGoalOffset / 10;
		moving_obj.transform.localScale = new Vector3(xScale, 1, 1);
		goal_obj.transform.localScale = new Vector3(xScale, 1, 1);

		canMove = true;
	}
	void Update(){
		if (canMove == false)
			return;
			
		if (isLeft){
			MoveRight();
			
		}else if (isRight){
			MoveLeft();
		}
	}
	public void CheckForGoal(){
		float offset = startingGoalOffset - difficultyOffset;
		Debug.Log("Offset at: " + offset);

		if (move_x >= target_position - offset && move_x <= target_position + offset){
			Debug.Log("You got it!");
			IncreaseDifficulty();
			if (onGameSuccess != null)
				onGameSuccess();
		}else{
			Debug.Log("You missed, it's at " + move_x + " and goal is at " + target_position);
			if (onGameFail != null)
				onGameFail();
		}
	}
	void IncreaseDifficulty(){
		difficultyOffset += 1f;
		difficultyOffset = Mathf.Clamp(difficultyOffset, 0, 9f);
		Debug.Log("Difficulty increased to " + difficultyOffset);
	}
	void MoveRight(){
		if (moving_obj.transform.localPosition.x >= right){
				isLeft = false;
				isRight = true;
				return;
		}
		move_x += speed * Time.deltaTime;
		moving_obj.transform.localPosition = new Vector2(move_x, moving_obj.transform.localPosition.y);
	}
	void MoveLeft(){
		if (moving_obj.transform.localPosition.x <= left){
				isLeft = true;
				isRight = false;
				return;
		}
		move_x -= speed * Time.deltaTime;
		moving_obj.transform.localPosition = new Vector2(move_x, moving_obj.transform.localPosition.y);
	}

	public void Deactivate(){
			canMove = false;
			isLeft = true;
			isRight = false;
			moving_obj.transform.localPosition = new Vector2(-1, 0);
			difficultyOffset = 0;
			this.gameObject.SetActive(false);
	}
}
