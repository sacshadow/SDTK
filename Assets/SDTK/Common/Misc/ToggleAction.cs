using UnityEngine;
// using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public class ToggleAction {
	
	public Action Execute = ()=>{};
	
	public void Set(Action toggleAction) {
		Execute = ()=> {
			toggleAction();
			Execute = ()=>{};
		};
	}
}
