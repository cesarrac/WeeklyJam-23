using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatType{Hitpoints, Slipperiness, Grip}
[System.Serializable]
public class Stat  {
	public StatType statType;
	int value;
	public int minValue, maxValue;

	public Stat(StatType _statType, int min, int max){
		value = min;
		statType = _statType;
		minValue = min;
		maxValue = max;
	}

	public int GetValue(int modifier = 0){
		if (maxValue > minValue){
			return Random.Range(minValue, maxValue + 1) + modifier;
		}
		return value + modifier;
	}
}
