using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using SDTK;

namespace SDTK.ToolBox{
	public class ToolBox : EditorWindow {
		public static ToolBox Instance{
			get{return singleton;}
		}
		private static ToolBox singleton;
		
		private static int lastTagId=-1;
		private static string[] tagName;
		private static GUIContent[] icon;
		private static int[] elementWidth;
		private static List<int> lineCount;
		private static TBData data{
			get{
				return TBData.data;
			}
		}
		
		private int orgWidth=-1;
		private bool isOpen = false;
		private Vector2 scrollPosition;
		private bool isBtnDelete = false;
		
		[MenuItem("SDTK/ToolBox &q")]
		public static void OpenToolBox(){
			ToolBox toolbox = EditorWindow.GetWindow(typeof(ToolBox)) as ToolBox;
			if(toolbox.isOpen)
				toolbox.Close();
			else
				toolbox.isOpen = true;
		}
		
		public void RefreshData(){
			GetTags();
			RefreshIcon();
		}
		
		public void RefreshIcon(){
			icon=new GUIContent[data.tag[data.tagIndex].element.Count];
			elementWidth = new int[icon.Length];
			lineCount = new List<int>();
			
			for(int i=0; i<icon.Length; i++){
				icon[i] = data.tag[data.tagIndex].element[i];
				elementWidth[i] = data.tag[data.tagIndex].element[i].GetWidth();
			}
			
			RefreshLine();
			lastTagId=data.tagIndex;
			TBData.SaveData();
		}
		
		private void RefreshLine(){
			int lineWidth = 0;
			orgWidth = Screen.width;
			lineCount = new List<int>();
			
			for(int i=0; i<elementWidth.Length; i++){
				lineWidth += elementWidth[i];
				if(lineWidth>orgWidth){
					lineCount.Add(i);
					lineWidth = elementWidth[i];
				}
			}
			lineCount.Add(elementWidth.Length);
		}
		
		private void GetTags(){
			tagName=new string[data.tag.Count];
			
			for(int tCount=0; tCount<tagName.Length; tCount++)
				tagName[tCount]=data.tag[tCount].niceName;
		}
		
		void OnEnable(){
			titleContent = new GUIContent("SDTK ToolBox");
			singleton = this;
			
			TBData.LoadData();
			RefreshData();
		}
		
		void OnInspectorUpdate(){
			Repaint();
		}
		
		void OnGUI(){
			if(data==null || data.tag==null || data.tag.Count==0){
				GUILayout.Label("Error occur\nPlease delete all the \"*.bin\" files inside \"SDTK/ToolBox/Data/\" folder, then close this window and open again");
				return;
			}
			
			if(orgWidth != Screen.width)
				RefreshLine();
			
			scrollPosition=GUILayout.BeginScrollView(scrollPosition);
			DrawTag();
			DrawBtn();
			GUILayout.EndScrollView();
			
			DragUpdate();
		}
		
		private void DrawTag(){
			if(Screen.width>120){
				GUILayout.BeginHorizontal();
					AutoFitTag();
					GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
			}
			else
				AutoFitTag();
			
			if(data.tagIndex!=lastTagId)
				RefreshIcon();
		}
		
		private void AutoFitTag(){
			if(GUILayout.Button("Setup",GUILayout.Width(50)))
				ShowSetup();
				
			if(Screen.width>400)
				data.tagIndex = GUILayout.Toolbar(data.tagIndex,tagName);
			else
				data.tagIndex = EditorGUILayout.Popup(data.tagIndex,tagName);
		}
		
		private void ShowSetup(){
			TBEditor.Open();
		}
		
		
		private void DrawBtn(){
			int start = 0;
			for(int i =0; i<lineCount.Count; i++){
				DrawEachLine(start, lineCount[i]);
				if(isBtnDelete)
					break;
				start = lineCount[i];
			}
			isBtnDelete = false;
		}
		
		private void DrawEachLine(int start, int end){
			GUILayout.BeginHorizontal();
			
			for(int i=start; i<end; i++){
				
				DrawEach(i);
			}
			
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}
		
		private void DrawEach(int index){
			if(index>=icon.Length)
				return;
		
			if(Event.current.control){
				GUILayout.BeginVertical();
				if(GUILayout.Button(icon[index],GUILayout.Height(50)))
					data.tag[data.tagIndex].element[index].Apply();
				if( GUILayout.Button("Delete")){
					data.tag[data.tagIndex].element.RemoveAt(index);
					isBtnDelete=TBData.isChanged=true;
					RefreshIcon();
				}
				GUILayout.EndVertical();
			}
			else if(GUILayout.Button(icon[index],GUILayout.Height(50)))
				data.tag[data.tagIndex].element[index].Apply();
		}
		
		//监听拖拽
		private void DragUpdate(){
			EventType eventType = Event.current.type;
			if (eventType == EventType.DragUpdated)
				CheckLegal();
			
			if (eventType == EventType.DragPerform)
				TBData.AcceptDrag(data.tagIndex);
		}
		
		private void CheckLegal(){
			DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
			Event.current.Use();
		}
	}
}
