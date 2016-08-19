using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace SDTK.ToolBox{
	public static class TBTools {
		public static string WarpText(string name){
			string rt=name;
			rt.Replace("\\n","\n");
			string[] seg=rt.Split(' ');
			
			if(seg.Length==1)
				return name;
			
			for(int sCount=0, sum=0; sCount<seg.Length; sCount++){
				sum = CountLineLength(seg, sCount, sum);
			}	
			rt=String.Join(" ",seg);
			
			return rt;
		}
		
		private static int CountLineLength(string[] seg, int sCount, int sum){
			if(seg[sCount].Contains("\n"))
				return 0;
				
			int rt=sum+seg[sCount].Length;
				
			if(sum>10 && sCount!=0){
				seg[sCount-1]+="\n";
				return 0;
			}
			return rt;
		}
		
		public static List<string> Analysis(string path){
			List<string> menuItem=new List<string>();
			string keyword="";
			string keyword2="";
			
			if(path.Contains(".js")){
				keyword="@MenuItem(\"";
			}
			else if(path.Contains(".cs")){
				keyword="[MenuItem(\"";
				keyword2="[UnityEditor.MenuItem(\"";
			}
			else 
				return null;
			
			FileInfo file=new FileInfo(Application.dataPath+"/../"+path);
			StreamReader reader=file.OpenText ();
			
			string txt=reader.ReadLine();
			string itemPath="";
			
			while(txt!=null){
				bool isMatch=false;
				string check =  txt.Replace(" ","");
				
				if(check.Contains(keyword)){
					isMatch=true;
				}
				else if(keyword2!="" && check.Contains(keyword2)){
					isMatch=true;
				}
				
				if(check.Contains("CONTEXT"))
					isMatch=false;
				
				if(isMatch){
					itemPath=txt.Split('"')[1];
					string[] sg=itemPath.Split(' ');
					
					if(sg[sg.Length-1].Length>1){
						char c=sg[sg.Length-1][0];
						if(c=='&' || c=='%' || c=='_' || c=='#')
							sg[sg.Length-1]="";
					}
					
					itemPath=String.Join(" ",sg);
					itemPath=itemPath.Trim("\t  ".ToCharArray());
					
					menuItem.Add(itemPath);
				}
				txt=reader.ReadLine();
			}
			
			reader.Close();
			
			return menuItem;
		}	
		
	}
}
