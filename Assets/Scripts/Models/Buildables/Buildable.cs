using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum BuildableType { Machine, Producer, Decoration}
public class Buildable {

    public string name;
    public Sprite sprite;
    public RuntimeAnimatorController animatorController;
    public BuildableType buildableType;
    public Stat[] stats;

    protected Buildable(BuildablePrototype b){
        this.name = b.name;
        this.sprite = Sprite_Manager.instance.GetSprite(b.sprite);
        this.animatorController = Sprite_Manager.instance.GetAnimator(b.animatorController);
        this.buildableType = b.buildableType;
        this.stats = new Stat[b.stats.Length];
		for(int i = 0; i < this.stats.Length; i++){
			this.stats[i] = new Stat(b.stats[i].statType, b.stats[i].minValue, b.stats[i].maxValue);
		}
    }
    protected Buildable(string name, string _sprite, string animatorName, BuildableType buildType, Stat[] bStats){
        this.name = name;
        if (Sprite_Manager.instance != null){
            this.sprite = Sprite_Manager.instance.GetSprite(_sprite);
            this.animatorController = Sprite_Manager.instance.GetAnimator(animatorName);
        }
        this.buildableType = buildType;
        this.stats = new Stat[bStats.Length];
		for(int i = 0; i < this.stats.Length; i++){
			this.stats[i] = new Stat(bStats[i].statType, bStats[i].minValue, bStats[i].maxValue);
		}
    }
    public static Buildable CreateInstance(BuildablePrototype prototype){
        return new Buildable(prototype);
    }

    public Stat GetStat(StatType statType){
        foreach (Stat stat in this.stats)
        {
            if (stat.statType == statType)
                return stat;
        }
        return null;
    }
}