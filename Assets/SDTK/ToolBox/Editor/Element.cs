using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace SDTK.ToolBox{
	[System.Serializable]
	public class Element {
		public string niceName;
		public string tooltip;
		public string data;
		public string picID;
		public ElementType elementType;
		
		public Element(){
			niceName="New Element";
			tooltip="Assign value in ToolBox Editor";
			data="";
			picID="";
			elementType=ElementType.NULL;
		}
		public Element(string nName, string nData, ElementType eType){
			niceName=nName;
			data=nData;
			elementType=eType;
			picID = "";
			
			switch(elementType){
				case ElementType.NULL: throw new System.Exception("[Toolbox]Type null");
				case ElementType.ASSET: break;
				case ElementType.FOLDER: break;
				case ElementType.MATERIAL: break;
				case ElementType.MENU_ITEM: picID=AssetDatabase.AssetPathToGUID("Assets/SDTK/ToolBox/Icon/EditorScript.png");break;
				case ElementType.PREFABE: picID=AssetDatabase.AssetPathToGUID("Assets/SDTK/ToolBox/Icon/prefab.png");break;
				case ElementType.SCRIPT: picID=AssetDatabase.AssetPathToGUID("Assets/SDTK/ToolBox/Icon/script.png");break;
				default:throw new System.Exception("Unknow type "+elementType);
			}
			
			tooltip=niceName.Replace("\n"," ");
		}
		
		public void Apply(){
			switch(elementType){
				case ElementType.NULL : Debug.LogWarning("Empty Button or asset is removed");break;
				case ElementType.ASSET : SelectObject();break;
				case ElementType.FOLDER : SelectObject();break;
				case ElementType.MATERIAL : SelectObject();break;
				case ElementType.MENU_ITEM : EditorApplication.ExecuteMenuItem(data);break;
				case ElementType.PREFABE : InstancePrefab();break;
				case ElementType.SCRIPT : AddComponents();break;
				default : throw new System.Exception("Unknow type "+elementType);
			}
		}
		
		public int GetWidth(){
			int width=0;
			string[] line = niceName.Split('\n');
			
			for(int i=0; i<line.Length; i++){
				width = Mathf.Max(width,line[i].Length*5);
			}
			
			return width + 40 + (picID == ""?0:20);
		}
		
		private void AddComponents(){
			GameObject[] gos = Selection.gameObjects;
			if(gos == null || gos.Length == 0){
				EditorUtility.DisplayDialog("Nothing Selected","Target GameObject needed for add script "+data,"ok");
				return;
			}
			
			Undo.RecordObjects(gos,"AddComponent "+data);
			
			//*********************************//
			//TODO: Change To 5.x API
			// foreach(GameObject go in gos)
				// UnityEngineInternal.APIUpdaterRuntimeServices.AddComponent(go, "Assets/SDTK/ToolBox/Editor/Element.cs (76,5)", data);
		}
		
		private void InstancePrefab(){
			if(data==null || data.Length==0)
				throw new System.Exception("[ToolBox]Data lost; you may want remove this btn; hold ctrl to remove;");
			
			string path=AssetDatabase.GUIDToAssetPath(data);
			GameObject prefab=AssetDatabase.LoadAssetAtPath(path,typeof(GameObject)) as GameObject;
			
			if(prefab==null)
				throw new System.Exception("[ToolBox]Prefab lost; you may want remove this btn; hold ctrl to remove;");
			
			UnityEngine.Object obj=PrefabUtility.InstantiatePrefab(prefab);
			Selection.activeObject=obj;
			EditorApplication.ExecuteMenuItem("GameObject/Move To View");
		}
		
		private void SelectObject(){
			if(data==null || data.Length==0)
				throw new System.Exception("[ToolBox]Data lost; you may want remove this btn; hold ctrl to remove;");
			
			string assetPath=AssetDatabase.GUIDToAssetPath(data);
			UnityEngine.Object o=AssetDatabase.LoadAssetAtPath(assetPath,typeof(UnityEngine.Object));
			if(o==null)
				throw new System.Exception("[ToolBox]Asset lost; you may want remove this btn; hold ctrl to remove;");
			
			Selection.activeObject=o;
		}
		
		private void OnDrag(){
			//TODO: Drag Object to scenes;
		}
		
		
		
		public static implicit operator GUIContent(Element element){
			GUIContent rt=new GUIContent(element.niceName, element.tooltip);
			if(element.picID==null || element.picID.Length<0)
				return rt;
			
			string path=AssetDatabase.GUIDToAssetPath(element.picID);
			Texture2D pic=AssetDatabase.LoadAssetAtPath(path,typeof(Texture2D)) as Texture2D;
			
			rt.image=pic;
			return rt;
		}
	}
}
