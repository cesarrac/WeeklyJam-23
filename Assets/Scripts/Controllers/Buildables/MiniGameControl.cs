using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
public class MiniGameControl : MonoBehaviour {
	public SpriteRenderer line;
	public GameObject goal, indicator;
	BoxCollider2D goalCollider;
	Sequence sequence;
	float minX, maxX;
	float movementDuration = 0.5f;
	bool isSuccess = false;
	public delegate void OnGame();
	public event OnGame onGameSuccess;
	public event OnGame onGameFail;
	public Text miniGameText;
	void Awake(){
		goalCollider = goal.GetComponent<BoxCollider2D>();
	}
	public void Initialize(MachineCondition curMachineCondition, MiniGameDifficulty difficulty){
		movementDuration = 1;
		if ((int) difficulty > 0){
			float diff = (int) difficulty;
			diff = diff / 10;
			movementDuration -= diff;
		}
			// The difficulty offset based on current condition of machine
			// if it's condition is less than OK (3)
		if ((int) curMachineCondition <= 2){
			float diff = (int) curMachineCondition;
			diff = diff / 10;
			if (diff > 0)
				movementDuration -= diff;
		}
		Initialize();
	}
	public void Initialize(MiniGameDifficulty difficulty){
		movementDuration = 1;
		if ((int) difficulty > 0){
			float diff = (int) difficulty;
			diff = diff / 10;
			movementDuration -= diff;
		}
		Initialize();
	}
	

	void Initialize(){
		sequence = DOTween.Sequence();
		// Calculate minX and maxX
		minX = -((line.size.x / 2) - 0.5f);
		maxX = line.size.x - 1f;
		// Place the goal in a random location from min X to max X
		goal.transform.localPosition = new Vector2(Random.Range(-2.5f, 2.5f), 0);
		indicator.transform.localPosition = new Vector2(minX, 0);
		// Set movement duration according to the difficulty
		sequence.Append(indicator.transform.DOMoveX(maxX, movementDuration).SetRelative());
		sequence.SetLoops(-1, LoopType.Yoyo);
	
	}
	
	void Update(){
		if (Input.GetMouseButtonDown(0)){
			CheckGoal();
		}
	}
	
	void CheckGoal(){
		miniGameText.gameObject.SetActive(true);
		if (goalCollider.OverlapPoint(indicator.transform.position) == true){
			Debug.Log("You did it!");

			isSuccess = true;
			sequence.Join(goal.GetComponent<SpriteRenderer>().DOColor(Color.green, 0.1f));
			miniGameText.text = "Hit!";
			miniGameText.transform.position = Camera.main.WorldToScreenPoint(transform.position);
			sequence.Join(miniGameText.DOFade(1, 0.25f));
			sequence.Join(miniGameText.DOColor(Color.green, 0.25f));
		}else{
			Debug.Log("You missed!");
			miniGameText.transform.position = Camera.main.WorldToScreenPoint(transform.position);
			miniGameText.text = "Miss!";
			sequence.Join(miniGameText.DOFade(1, 0.25f));
			sequence.Join(miniGameText.DOColor(Color.red, 0.25f));
			isSuccess = false;
			sequence.Join(goal.GetComponent<SpriteRenderer>().DOColor(Color.red, 0.1f));
		}
		sequence.Kill();
		sequence.Append(goal.transform.DOShakeRotation(1f, new Vector3(0, 0, 10)).OnComplete(() => FinishGame()));
		sequence.Join(goal.transform.DOScale(new Vector3(1.5f, 1.5f, 0), 0.1f));
	}
	void FinishGame(){
		sequence.Join(miniGameText.DOFade(0, 0.25f));
		if (isSuccess){
			if (onGameSuccess != null)
				onGameSuccess();
		}else{
			if(onGameFail != null)
				onGameFail();
		}
		//this.gameObject.SetActive(false);
	}
	void OnDisable(){
		if (sequence != null)
			sequence.Kill();
		goal.transform.localScale = Vector3.one;
		goal.GetComponent<SpriteRenderer>().color = Color.white;
		miniGameText.gameObject.SetActive(false);
	/* 	onGameFail = null;
		onGameSuccess = null; */
	}
}
