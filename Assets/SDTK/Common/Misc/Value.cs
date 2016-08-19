using UnityEngine;
// using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public class Value<T> {
	public Func<T> value;
	
	private Func<T> process;
	
	public Value(Func<T> process) {
		this.process = process;
		
		value = ()=> {
			T rt = process();
			value = () => rt;
			return value();
		};
	}
	
	public static implicit operator T(Value<T> t) {
		return t.value();
	}
	
	public Value<T> ValueExpiredTime(float seconds) {
		if(seconds <= 0)
			throw new System.Exception("Expired time can not be negtive - " + seconds);
		value = () => {
			T rt = process();
			float lastTime = Time.time;
			value = ()=> {
				if(Time.time > lastTime + seconds) {
					rt = process();
					lastTime = Time.time;
				}
				return rt;
			};
			return value();
		};
		
		return this;
	}
}
