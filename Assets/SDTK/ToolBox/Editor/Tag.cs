using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace SDTK.ToolBox{

	[System.Serializable]
	public class Tag {
		public string niceName;
		public List<Element> element;
		
		public Tag(){
			niceName="New Tag";
			element=new List<Element>();
		}
		public Tag(string nName){
			niceName=nName;
			element=new List<Element>();
		}
		
		public int GetElementOffset(int org, int offset){
			return Mathf.Clamp(org+offset,0,element.Count-1);
		}
		
		public void AddScript(string name, string data){
			element.Add(new Element(name,data,ElementType.SCRIPT));
		}
		
		public void AddMenuItem(string name, string data){
			element.Add(new Element(name,data,ElementType.MENU_ITEM));
		}
		
		public void AddFolder(string name, string path){
			string data="";//TODO: path to data;
			element.Add(new Element(name,data,ElementType.FOLDER));
		}
		
		public void AddAsset(UnityEngine.Object uobj){
			string path=AssetDatabase.GetAssetPath(uobj);
			string guid=AssetDatabase.AssetPathToGUID(path);
			
			if(uobj is Material)
				element.Add(new Element(uobj.name,guid,ElementType.MATERIAL));
			else
				element.Add(new Element(uobj.name,guid,ElementType.ASSET));
		}
		
		public void AddPrefab(UnityEngine.Object prefab){
			if(PrefabUtility.GetPrefabType(prefab)!=PrefabType.Prefab &&
				PrefabUtility.GetPrefabType(prefab)!=PrefabType.ModelPrefab 
			)
				throw new System.Exception("Only Prefab can save to shelf; object type is "+PrefabUtility.GetPrefabType(prefab));
			
			
			string path=AssetDatabase.GetAssetPath(prefab);
			string guid=AssetDatabase.AssetPathToGUID(path);
			
			element.Add(new Element(prefab.name,guid,ElementType.PREFABE));
		}
	}
}
