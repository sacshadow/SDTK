using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace SDTK.ToolBox{
	[System.Serializable]
	public class TBData {
		public static TBData data;
		public static bool isChanged=false;
		
		public string niceName = "default.bin";
		public int selectTagIndex=0;
		public List<Tag> tag;
		
		public int tagIndex{
			get{
				return GetLastOpenedTag();
			}
			set{
				selectTagIndex = Mathf.Clamp(value,0,tag.Count-1);
			}
		}
		
		public TBData(){
			selectTagIndex=0;
			tag=new List<Tag>();
		}
		
		public int OffsetTag(int offset){
			return Mathf.Clamp(selectTagIndex+offset,0,tag.Count-1);
		}
		
		public int GetLastOpenedTag(){
			if(tag == null || tag.Count==0)
				throw new System.Exception("Data Error");
			
			return Mathf.Clamp(selectTagIndex,0,tag.Count-1);
		}
		
		public static void SaveData(){
			SaveAs(data.niceName);
		}
		public static void SaveAs(string name){
			data.niceName = name;
			TBConfig.SaveData<TBData>(data,name);
			EditorPrefs.SetString("ToolBoxData",data.niceName);
		}
		
		public static void LoadData(){
			CheckDefault();
			string lastSaveData = EditorPrefs.GetString("ToolBoxData","default.bin");
			data=TBConfig.LoadData<TBData>(lastSaveData);
		}
		
		public static void AddTag(string tagName){
			data.tag.Add(new Tag(tagName));
		}
		
		public static void RemveTag(int tagId){
			data.tag.RemoveAt(tagId);
		}
		
		public static void AcceptDrag(int tagId){
			DragAndDrop.AcceptDrag();
			
			UnityEngine.Object[] dragObjects= DragAndDrop.objectReferences;
			
			string path="";
			
			foreach(UnityEngine.Object uobj in dragObjects){
				
				path=AssetDatabase.GetAssetPath(uobj);
				
				if(path.Contains("/Editor/"))//编辑器脚本，查找menuItem
					AcceptMenuItem(TBTools.Analysis(path), tagId);
				else if(path.Contains(".js") || path.Contains(".cs"))
					data.tag[tagId].AddScript(uobj.name,uobj.name);
				else if(path.Contains(".prefab"))
					data.tag[tagId].AddPrefab(uobj);
				else
					data.tag[tagId].AddAsset(uobj);
			}
			
			SaveData();
			ToolBox.Instance.RefreshData();
			isChanged=true;
		
			Event.current.Use();
		}
		
		private static void AcceptMenuItem(List<string> menuItem, int tagId){
			string name;
			
			for(int i=0; i<menuItem.Count; i++){
				name=menuItem[i].Substring(menuItem[i].LastIndexOf("/")+1);
				if(name == "")
					continue;
				
				data.tag[tagId].AddMenuItem(TBTools.WarpText(name), menuItem[i]);
			}
		}
		
		private static void CheckDefault(){
			string lastSaveData = EditorPrefs.GetString("ToolBoxData","default.bin");
			if(!TBConfig.IsDataExists(lastSaveData)){
				data = new TBData();
				AddTag("New Tag");
				SaveAs("default.bin");
			}
		}
	}
}
