using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sprite_Manager : MonoBehaviour {
	public static Sprite_Manager instance {get; protected set;}
	Dictionary<string, Sprite> spritesMap;
	Dictionary<string, RuntimeAnimatorController> animatorsMap;
	private void Awake() {
		instance = this;
		spritesMap = new Dictionary<string, Sprite>();
	
		Sprite[] items = Resources.LoadAll<Sprite>("Sprites/Items/");
		foreach(Sprite item in items){
			AddSpriteToMap(item);
		}
		Sprite[] machines = Resources.LoadAll<Sprite>("Sprites/Machines/");
		foreach(Sprite machine in machines){
			AddSpriteToMap(machine);
		}
		animatorsMap = new Dictionary<string, RuntimeAnimatorController>();
		RuntimeAnimatorController[] animators = Resources.LoadAll<RuntimeAnimatorController>("Animators/Machines/");
		foreach (RuntimeAnimatorController animator in animators)
		{
			AddAnimatorToMap(animator);
		}
	}
	void AddSpriteToMap(Sprite sprite){
		if (spritesMap.ContainsKey(sprite.name) == true)
			return;
		spritesMap.Add(sprite.name, sprite);
	}
	void AddAnimatorToMap(RuntimeAnimatorController anim){
		if (animatorsMap.ContainsKey(anim.name) == true)
			return;
		animatorsMap.Add(anim.name, anim);
	}
	public Sprite GetSprite(string name){
		if (spritesMap.ContainsKey(name) == false)
			return null;
		
		return spritesMap[name];
	}
	public RuntimeAnimatorController GetAnimator(string name){
		if (animatorsMap.ContainsKey(name) == false)
			return null;
		
		return animatorsMap[name];
	}
}
