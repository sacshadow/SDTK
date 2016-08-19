using UnityEngine;
//using UnityEditor;
using System;
//using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class GT  {
	
	public static T FindObjectOfType<T>() where T : UnityEngine.Object {
		return UnityEngine.Object.FindObjectOfType(typeof(T)) as T;
	}
	
	public static T[] FindObjectsOfType<T>() where T: UnityEngine.Object {
		return UnityEngine.Object.FindObjectsOfType(typeof(T)).Select(x=>x as T).ToArray();
	}
	
	public static T Instantiate<T>(T t) where T : UnityEngine.Object {
		return UnityEngine.Object.Instantiate(t) as T;
	}
	
	public static T Instantiate<T>(T t, Vector3 pos, Quaternion rot) where T : UnityEngine.Object {
		return UnityEngine.Object.Instantiate(t, pos, rot) as T;
	}

    public static void DestroyIfExist(UnityEngine.Object obj)
    {
        if (obj != null)
            UnityEngine.Object.Destroy(obj);
    }

	public static void CallBack(List<Action> callBack) {
		callBack.ForEach(call => call());
	}
}
