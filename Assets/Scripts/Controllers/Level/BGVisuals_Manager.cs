using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BGVisuals_Manager : MonoBehaviour {
	public static BGVisuals_Manager instance {get; protected set;}
	ObjectPool pool;
	public int totalPixelStarAnimations = 3;
	public int totalPixelStarsToSpawn = 50;

	public int totalAsteroidAnimations = 2;
	public int asteroidCount = 10;
	List<GameObject> pixelStars_active, asteroids_active;
	GameObject bgHolder, asteroidsHolder, pixelStarHolder;
	public Transform vCamTransform;
	public Cinemachine.CinemachineVirtualCamera bgVCam;
	float maxY, maxX, minX, minY;
	private void Awake() {
		instance = this;
		pixelStars_active = new List<GameObject>();
		asteroids_active = new List<GameObject>();
		maxY = (Camera.main.orthographicSize * 2) + 2;
		minY = -(maxY) - 2;
		maxX = (Camera.main.orthographicSize * 2) + 2f;
		minX = -(maxX) - 2;
		bgHolder = new GameObject();
		bgHolder.name = "BACKGROUND EFFECTS";
	}
	private void Start() {
		pool = ObjectPool.instance;
		
	}
	public void GenerateSpace(){
		GeneratePixelStars();
		StartCoroutine("WaitToSpawnAsteroids");
	}
	void GeneratePixelStars(){
	
		if (pixelStarHolder == null){
			pixelStarHolder = new GameObject();
			pixelStarHolder.name = "PIXEL STARS";
			pixelStarHolder.transform.SetParent(bgHolder.transform);
			//pixelStarHolder.transform.position = new Vector3(0,0,2);
			//bgVCam.Follow = pixelStarHolder.transform;
			/* Parallax_Controller parallax = pixelStarHolder.AddComponent<Parallax_Controller>();
			parallax.SetAnchor(virtualCamera);
			parallax.parallaxSpeed = 0.25f;
			parallax.distanceFromAnchor = 1; */
		}
		for (int i = 0; i < totalPixelStarsToSpawn; i++)
		{
			int animSelection = Random.Range(1, totalPixelStarAnimations + 1);
			GameObject starGobj = SpawnEffect("Pixel Star",Random.Range(minX, maxX), Random.Range(minY, maxY));
			if (starGobj == null)
				return;
			starGobj.transform.SetParent(pixelStarHolder.transform);
			starGobj.GetComponentInChildren<Animator>().Play("Pixel_stars" + animSelection.ToString());
			Color starColor = new Color32(255, 255, 255, (byte)Random.Range(50, 255));
			starGobj.GetComponentInChildren<SpriteRenderer>().color = starColor;
			pixelStars_active.Add(starGobj);
		}

		float shipMaxX = TileManager.instance.GetMaxX();
		float shipMaxY = TileManager.instance.GetMaxY();
		foreach (GameObject star in pixelStars_active)
		{
			if (star.transform.position.x >= 0 && star.transform.position.x <= shipMaxX &&
				star.transform.position.y >= 0 && star.transform.position.y <= shipMaxY){
					float newX = star.transform.position.x <= shipMaxX * 0.5f ? -star.transform.position.x - 4 : star.transform.position.x  + (shipMaxX - star.transform.position.x) + 4;
					float newY = star.transform.position.y <= shipMaxY * 0.5f ? -star.transform.position.y - 4 : star.transform.position.y  + (shipMaxY - star.transform.position.y) + 4;
					star.transform.position = new Vector2(newX, newY);
				}
		}
	}
	GameObject SpawnEffect(string effectName, float x, float y){
			Vector2 randomPos = new Vector2(x,y);
			GameObject effectGObj = pool.GetObjectForType(effectName, true, randomPos);
			
			return effectGObj;
	}
	void StartGenerateAsteroids(){
		StartCoroutine("SpawnAsteroids");
	}
	IEnumerator WaitToSpawnAsteroids(){
		while(true){
			StopCoroutine("SpawnAsteroids");
			yield return new WaitForSeconds(2.75f);
			StartGenerateAsteroids();
			yield break;
		}
	}
	IEnumerator SpawnAsteroids(){
		while(true){
			for (int i = 0; i < asteroidCount; i++)
			{
				GameObject asteroid = SpawnEffect("Asteroid", Random.Range(minX, maxX), -20);

				yield return new WaitForSeconds(2);

				if (asteroid == null)
					continue;
				int animSelection = Random.Range(1, totalAsteroidAnimations + 1);
				asteroid.GetComponentInChildren<Animator>().Play("Asteroid" + animSelection.ToString());
				asteroid.transform.DOMoveY(maxY + 10, Random.Range(40, 100)).OnComplete(() => PoolAsteroid(asteroid));
				yield return null;
			}
			StartCoroutine("WaitToSpawnAsteroids");
			yield break;
		}
	}

	void PoolAsteroid(GameObject asteroid){
		pool.PoolObject(asteroid);
	}
}
