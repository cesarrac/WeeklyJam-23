using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat  {

	int value;
	public string name;

	public int minValue, maxValue;

	public Stat(string _name, int min, int max){
		value = min;
		name = _name;
		minValue = min;
		maxValue = max;
	}

	public int GetValue(int modifier = 0){
		if (maxValue > minValue){
			return Random.Range(minValue, maxValue + 1) + modifier;
		}
		return value + modifier;
	}
	public void Modify(int minModifier, int maxModifier){
		minValue += minModifier;
		maxValue += maxModifier;
		value = minValue;
	}
}
