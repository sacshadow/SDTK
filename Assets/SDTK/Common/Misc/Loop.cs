using UnityEngine;
// using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;


public class Loop {
	public int begin {get; private set;}
	public int end {get; private set;}
	
	public static Loop Begin(int bg){
		return new Loop(bg,0);
	}
	
	public static Loop Count(int ed){
		return new Loop(0,ed);
	}
	public static Loop Count<T>(T[] ary) {
		return new Loop(0,ary.Length);
	}
	public static Loop Count<T>(List<T> list) {
		return new Loop(0,list.Count);
	}
	
	public static Loop Between(int bg, int ed) {
		return new Loop(bg,ed);
	}
	
	public static void ForEach<T>(IEnumerable<T> array, Action<T> process) {
		foreach(var element in array) process(element);
	}
	
	public Loop(int bg, int ed) {
		this.begin = bg;
		this.end = ed;
	}
	
	public Loop To(int ed){
		end = ed;
		return this;
	}
	
	public Loop Do(Action<int> action){
		for(int i=begin; i<end; i++){
			action(i);
		}
		return this;
	}
	
	public List<T> Select<T>(Func<int,T> func){
		List<T> rt = new List<T>();
		for(int i=begin; i<end; i++){
			rt.Add(func(i));
		}
		return rt;
	}
	
}
