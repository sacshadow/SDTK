using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace SDTK.ToolBox{
	public class TBEditor : EditorWindow {
		private string[] page=new string[]{"Tag","Icon"};
		private int pageSelect = 1;
		
		private string[] tagName;
		private string[] elementName;
		private System.Action[] pageGUI;
		
		private Vector2 scrollPosition;
		private int orgTag = -1;
		private int selectElement = -1;
		private Element element;
		private string iconId = "";
		private Texture2D iconPic=null;
		
		private TBData data{
			get{return TBData.data;}
		}
		
		public static void Open(){
			EditorWindow.GetWindow(typeof(TBEditor));
		}
		
		void OnEnable(){
			if(data == null){
				Close();
				return;
			}
		
			wantsMouseMove=true;
			pageGUI = new System.Action[]{ShowTag,ShowElement};
			GetTagName();
			GetElementName();
		}
		
		private void GetTagName(){
			tagName=new string[data.tag.Count];
			for(int tCount=0; tCount<tagName.Length; tCount++)
				tagName[tCount]=data.tag[tCount].niceName;
		}
		
		private void GetElementName(){
			elementName=new string[data.tag[data.tagIndex].element.Count];
			for(int iCount=0; iCount<elementName.Length; iCount++){
				elementName[iCount]=data.tag[data.tagIndex].element[iCount].niceName;
				elementName[iCount]=elementName[iCount].Replace("\n"," ");
			}
			
			orgTag = data.tagIndex;
		}
		
		void OnInspectorUpdate(){
			Repaint();
		}
		
		void OnGUI(){
			pageSelect=GUILayout.Toolbar(pageSelect, page,GUILayout.Width(200));
			pageGUI[pageSelect]();
			
			// GUILayout.FlexibleSpace();
			
			GUILayout.BeginHorizontal();
			if(GUILayout.Button("Load"))
				LoadToolBox();
			
			if(GUILayout.Button("Save As"))
				SaveAs();
			GUILayout.EndHorizontal();
			
			GUILayout.BeginHorizontal();
			if(GUILayout.Button("Apply"))
				TBData.SaveData();
			
			if(GUILayout.Button("Save and Close")){
				TBData.SaveData();
				Close();
			}
			GUILayout.EndHorizontal();
		}
		
		private void LoadToolBox(){
			
		}
		
		private void SaveAs(){
			
		}
		
		private void ShowTag(){
			scrollPosition=GUILayout.BeginScrollView(scrollPosition,"box");
			data.tagIndex=GUILayout.SelectionGrid(data.tagIndex,tagName,1);
			GUILayout.FlexibleSpace();
			GUILayout.EndScrollView();
			GUILayout.BeginHorizontal();
			if(GUILayout.Button("Move up"))
				TagMove(-1);
			if(GUILayout.Button("Move down"))
				TagMove(1);
			if(GUILayout.Button("New"))
				NewTag();
			if(GUILayout.Button("Delete"))
				DeleteTag();
			GUILayout.EndHorizontal();
			
			GUILayout.Space(20);
			GUILayout.Label("Tag Name:");
			data.tag[data.tagIndex].niceName=EditorGUILayout.TextField(data.tag[data.tagIndex].niceName);
			GUILayout.Space(60);
		
			if(UnityEngine.GUI.changed)
				OnGUIChange();
		}
		
		private void OnGUIChange(){
			GetTagName();
			GetElementName();
			
			if(ToolBox.Instance!=null)
				ToolBox.Instance.RefreshData();
		}
		
		private void ShowElement(){
			DetectChange();
			
			scrollPosition=GUILayout.BeginScrollView(scrollPosition,"box");
			
			if(data.tag[data.tagIndex].element.Count!=0)
				ElementUpdate();
			else
				element = null;
			
			GUILayout.FlexibleSpace();
			GUILayout.EndScrollView();
			
			GUILayout.BeginHorizontal();
			if(GUILayout.Button("Move up"))
				ElementMove(-1);
			if(GUILayout.Button("Move down"))
				ElementMove(1);
			if(GUILayout.Button("Add"))
				AddElement();
			if(GUILayout.Button("Delete"))
				DeleteElement();
			GUILayout.EndHorizontal();
			
			GUILayout.Space(20);
			
			ShowElementDetail();
		}
		
		private void ShowElementDetail(){
			if(Event.current.type == EventType.MouseDown)
				GUIUtility.keyboardControl=0;
				
			if(element != null){
				GUILayout.BeginHorizontal();
				GUILayout.Label("Btn Name:",GUILayout.Width(60));
				element.niceName=EditorGUILayout.TextArea(element.niceName,GUILayout.Height(50));
			
				GUILayout.EndHorizontal();
				
				GUILayout.BeginHorizontal();
				GUILayout.Label("Tooltip:",GUILayout.Width(60));
				element.tooltip=EditorGUILayout.TextField(element.tooltip);
				GUILayout.EndHorizontal();
				
				GUILayout.BeginHorizontal();
				GUILayout.Label("Icon Image:",GUILayout.Width(60));
				
				if(element.picID==null || element.picID.Length==0)
					iconPic=null;
				else if(iconId!=element.picID)
					ReloadPic();
				
				iconPic=EditorGUILayout.ObjectField(iconPic,typeof(Texture2D),false,GUILayout.Width(50),GUILayout.Height(50)) as Texture2D;
				
				if(iconPic != null)
					element.picID=AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(iconPic));
				else
					element.picID="";
				
				GUILayout.EndHorizontal();
				
				GUILayout.BeginHorizontal();
				switch(element.elementType){
					case ElementType.SCRIPT: ShowScript(); break;
					case ElementType.MENU_ITEM: ShowMenuItem(); break;
					case ElementType.PREFABE: ShowPrefab(); break;
				}
				GUILayout.EndHorizontal();
			}
			
			GUILayout.Space(5);
			
			if(UnityEngine.GUI.changed)
				OnGUIChange();
		}
		
		private void ShowScript(){
		
		}
		
		private void ShowMenuItem(){
		
		}
		
		private void ShowPrefab(){
		
		}
		
		private void ReloadPic(){
			iconId = element.picID;
			string path = AssetDatabase.GUIDToAssetPath(element.picID);
			
			if(path==null || path.Length==0){
				iconPic = null;
				return;
			}
			
			iconPic=AssetDatabase.LoadAssetAtPath(path,typeof(Texture2D)) as Texture2D;
		
		}
		
		private void ElementUpdate(){
			selectElement=Mathf.Clamp(selectElement,0,data.tag[data.tagIndex].element.Count-1);
			selectElement=GUILayout.SelectionGrid(selectElement,elementName,1);
			
			if(element!= data.tag[data.tagIndex].element[selectElement])
				RefreshElement();
		}
		
		private void DeleteElement(){
			if(data.tag[data.tagIndex].element.Count==0)
				return;
			
			data.tag[data.tagIndex].element.RemoveAt(selectElement);
			selectElement=Mathf.Clamp(selectElement-1,0,data.tag[data.tagIndex].element.Count);
			
			TBData.SaveData();
			
			if(ToolBox.Instance!=null)
				ToolBox.Instance.RefreshData();
			
			// GetElementName();
			// RefreshElement();
		}
		
		private void AddElement(){
			
		}
		
		private void ElementMove(int offset){
			int newIndex = data.tag[data.tagIndex].GetElementOffset(selectElement, offset);
			
			Element temp=data.tag[data.tagIndex].element[selectElement];
			
			data.tag[data.tagIndex].element.RemoveAt(selectElement);
			data.tag[data.tagIndex].element.Insert(newIndex, temp);
			
			selectElement = newIndex;
			TBData.SaveData();
			
			if(ToolBox.Instance!=null)
				ToolBox.Instance.RefreshData();
			
			GetElementName();
		}
		
		private void DetectChange(){
			if(orgTag != data.tagIndex){
				GetElementName();
				RefreshElement();
			}
		}
		
		private void RefreshElement(){
			selectElement = Mathf.Clamp(selectElement,0,data.tag[data.tagIndex].element.Count);
			
			if(data.tag[data.tagIndex].element.Count == 0)
				element = null;
			else
				element = data.tag[data.tagIndex].element[selectElement];
			
			ReloadIcon();
		}
		
		private void ReloadIcon(){
			if(element == null){
				iconPic = null;
				return;
			}
			
			iconId = element.picID;
			string path=AssetDatabase.GUIDToAssetPath(element.picID);
			
			if(path==null || path.Length==0){
				iconPic=null;
				return;
			}
			
			iconPic=AssetDatabase.LoadAssetAtPath(path,typeof(Texture2D)) as Texture2D;
		}
		
		private void TagMove(int side){
			int newIndex =	data.OffsetTag(side);
			if(newIndex == data.tagIndex)
				return;
			
			Tag temp = data.tag[data.tagIndex];
			
			data.tag.RemoveAt(data.tagIndex);
			data.tag.Insert(newIndex, temp);
			
			data.tagIndex = newIndex;
			
			TBData.SaveData();
			if(ToolBox.Instance!=null)
				ToolBox.Instance.RefreshData();
			GetTagName();
		}
		
		private void NewTag(){
			TBData.AddTag("New Tag "+(data.tag.Count+1));
			
			data.tagIndex=data.tag.Count-1;
			OnGUIChange();
		}
		
		private void DeleteTag(){
			if(data.tag.Count == 0)
				TBData.AddTag("New Tag 1");
			
			data.tag.RemoveAt(data.tagIndex);
			
			if(data.tag.Count == 0)
				TBData.AddTag("New Tag 1");
			
			OnGUIChange();
		}
		
	}
}
