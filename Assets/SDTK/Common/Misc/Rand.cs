using UnityEngine;
//using UnityEditor;
// using System;
using System.Collections;
using System.Collections.Generic;
// using System.Linq;

public static class Rand  {
	public static float value {get {return Random.value; }}
	
	public static int Range(int min, int max) {
		return Random.Range(min,  max);
	}
	
	public static float Range(float min, float max) {
		return Random.Range(min, max);
	}
	
	public static List<T> Shuffle<T>(List<T> input) {
		List<T> rt = new List<T>(input);
		T temp;
		int max = input.Count;
		
		for(int i =0; i<max; i++) {
			int swap = Range(i, max) % max;
			
			if(swap != i) {
				temp = rt[i];
				rt[i] = rt[swap];
				rt[swap] = temp;
			}
		}
		
		return rt;
	}
	
}
