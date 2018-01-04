using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
public class SlipMGameControl : MonoBehaviour {

	// Move a net back and forth and catch the falling objects that appear
	// randomly at each end of the line

	public float startDelay = 3f;
	float pStartDelay = 0;
	float pFallDuration = 1f, fallDuration, pDuration = 3, duration;
	int pBallsRequired, ballsReq, ballsCaught;
	public SpriteRenderer line;
	public GameObject net, gamePanel;
	float minX, maxX;
	bool isSuccess = false, curBallHit = false;

	public delegate void OnGame();
	public event OnGame onGameSuccess;
	public event OnGame onGameFail;
	public GameObject ballReqPanel;
	public Text miniGameText, ballReqText, ballCaughtText;
	Sequence sequence;
	ObjectPool pool;
	List<GameObject> activeBalls;
	float ballHeight = 2;
	CountdownHelper durationCountdown;
	GameObject curBall;
	float movePercent, speed = 8;
	float destX = 0;

	public void Initialize(MiniGameDifficulty difficulty){
		if (pool == null)
			pool = ObjectPool.instance;

		ballsCaught = 0;
		isSuccess = false;
		pStartDelay = startDelay;
		ballsReq = 1;
		fallDuration = pFallDuration;
		duration = pDuration;

		if (difficulty > 0){
			float offset = (int)difficulty;
			offset = offset / 10;
			fallDuration -= offset;
			fallDuration = Mathf.Clamp(fallDuration, 0.1f, 1);
			duration += (int)difficulty / 10;
			ballsReq += (int)difficulty;
			Debug.Log("Slip game Fall duration set to: " + fallDuration + " offset was " + offset);
			
		}
		ballReqPanel.transform.position = Camera.main.WorldToScreenPoint(new Vector3(gamePanel.transform.position.x, gamePanel.transform.position.y - 1));
		ballReqPanel.SetActive(true);
		ballReqText.text = ballsReq.ToString();
		ballCaughtText.text = "0";

		//durationCountdown = new CountdownHelper(duration);
		gamePanel.transform.DOScale(1.5f, 0.25f).SetLoops(2, LoopType.Yoyo);
		sequence = DOTween.Sequence();
		minX = -1.5f;
		maxX = 1.5f;
		destX = 0;
		net.transform.localPosition = Vector2.zero;


		SpawnBall();
			
	}
	void SpawnBall(){
		int sideSelection = Random.Range(0, 2);
		float x = Random.Range(minX, maxX);//sideSelection > 0 ? maxX : minX;
		
		Vector2 spawnPosition = new Vector2(x, ballHeight);

		curBall = pool.GetObjectForType("SlipBall", true, transform.position);
		curBall.transform.SetParent(this.transform);
		curBall.transform.localPosition = spawnPosition;

		curBallHit = false;
		ExpandBall(curBall.transform, pStartDelay > 0 ? pStartDelay : 0.25f);
	}
	void ExpandBall(Transform ball, float delay){
		//sequence.Append(ball.transform.DOScale(2,delay).OnComplete(() => DropBall(ball)));
		sequence.Join(ball.GetComponent<SpriteRenderer>().DOFade(1, delay).OnComplete(() => DropBall(ball)));
		// The first time this runs it sets start delay back to 0 because it has already started
		pStartDelay = 0;
	}
	void DropBall(Transform ball){
		sequence.Append(ball.transform.DOMoveY(transform.position.y, fallDuration).OnUpdate(() => CheckForNet(ball.gameObject)).OnComplete(() => EndDrop(ball.gameObject)));
	}
	void Update(){

		if (Input.GetAxisRaw("Horizontal") != 0){
			destX = Input.GetAxisRaw("Horizontal") < 0 ? minX : maxX;
		}
		
		if (destX != 0){
			Move();
		}
		
	/* 	durationCountdown.UpdateCountdown();
		if (durationCountdown.elapsedPercent >= 1){
			OnGameComplete();
		} */
	}
	void Move(){
		float distanceLeft = Mathf.Abs(destX - net.transform.localPosition.x);
		movePercent = speed * Time.deltaTime / distanceLeft;
		net.transform.localPosition = Vector2.Lerp(net.transform.localPosition, new Vector2(destX, net.transform.localPosition.y), movePercent);
	}
	void CheckForNet(GameObject ball){
		if (ball.GetComponentInChildren<BoxCollider2D>().OverlapPoint(net.transform.position) == true && curBallHit == false){
			DoText (true);
			sequence.Append(ball.GetComponent<SpriteRenderer>().DOFade(0, 0.5f).OnComplete(() => NextBall(ball)));
			sequence.Join(ball.transform.DOScale(4, 0.25f));
			curBallHit = true;
			ballsCaught += 1;
			ballCaughtText.text = ballsCaught.ToString();
		}
	}
	void EndDrop(GameObject ball){
		if (curBallHit == true)
			return;
		
		sequence.Append(ball.GetComponent<SpriteRenderer>().DOFade(0, 0.5f).OnComplete(() => FailGame(ball)));
		sequence.Join(ball.transform.DOScale(1f, 0.25f));
		sequence.Join(ball.transform.DOShakeRotation(0.25f, new Vector3(0,0,60)));
		sequence.Join(ball.transform.DOMoveY(ball.transform.position.y - 4f, 0.75f));
	}
	void DoText(bool success){
		if (success){
			miniGameText.text = "NICE!";
			miniGameText.color = Color.blue;
		}
		else{
			miniGameText.text = "Miss";
			miniGameText.color = Color.red;
		}

		if (miniGameText.gameObject.activeSelf == false){
			Vector3 screenPos = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y, 0));
			miniGameText.transform.position = screenPos;
			Vector3 targetPos = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x,  transform.position.y + 1, 0));
			miniGameText.gameObject.SetActive(true);
			miniGameText.transform.DOMoveY(targetPos.y, 1).OnComplete(() => DisableText());
		}
	
	}
	void OnGameComplete(){
		isSuccess = true;
	/* 	if (curBall != null){
			PoolBall(curBall);
		} */
		// GAME SUCCESS!
		if(onGameSuccess != null){
			onGameSuccess();
		}
	}
	void NextBall(GameObject ball){
		PoolBall(ball);
		if (ballsCaught < ballsReq){
			SpawnBall();
		}
		else{
			OnGameComplete();
		}
	}
	void PoolBall(GameObject ball){

		ball.GetComponent<SpriteRenderer>().color = Color.white;
		ball.transform.localScale = Vector3.one;
		ball.transform.eulerAngles = Vector3.zero;
		ball.transform.SetParent(null);
		pool.PoolObject(ball);
	}
	void FailGame(GameObject oldBall){
		PoolBall(oldBall);
		sequence.Kill();
	
		if (isSuccess == false){
			DoText(false);
			if (onGameFail != null){
				onGameFail();
			}
		}
	}
	void DisableText(){
		if (miniGameText != null){
			miniGameText.gameObject.SetActive(false);
		}
	}

	void OnDisable(){
		ballReqPanel.SetActive(false);
	}
}
