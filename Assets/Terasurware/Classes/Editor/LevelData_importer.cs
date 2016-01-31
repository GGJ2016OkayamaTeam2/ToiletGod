using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Xml.Serialization;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

public class LevelData_importer : AssetPostprocessor {
	private static readonly string filePath = "Assets/ParamData/LevelData.xls";
	private static readonly string exportPath = "Assets/ParamData/LevelData.asset";
	private static readonly string[] sheetNames = { "LevelData", };
	
	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		foreach (string asset in importedAssets) {
			if (!filePath.Equals (asset))
				continue;
				
			Entity_LevelData data = (Entity_LevelData)AssetDatabase.LoadAssetAtPath (exportPath, typeof(Entity_LevelData));
			if (data == null) {
				data = ScriptableObject.CreateInstance<Entity_LevelData> ();
				AssetDatabase.CreateAsset ((ScriptableObject)data, exportPath);
				data.hideFlags = HideFlags.NotEditable;
			}
			
			data.sheets.Clear ();
			using (FileStream stream = File.Open (filePath, FileMode.Open, FileAccess.Read)) {
				IWorkbook book = new HSSFWorkbook (stream);
				
				foreach(string sheetName in sheetNames) {
					ISheet sheet = book.GetSheet(sheetName);
					if( sheet == null ) {
						Debug.LogError("[QuestData] sheet not found:" + sheetName);
						continue;
					}

					Entity_LevelData.Sheet s = new Entity_LevelData.Sheet ();
					s.name = sheetName;
				
					for (int i=1; i<= sheet.LastRowNum; i++) {
						IRow row = sheet.GetRow (i);
						ICell cell = null;
						
						Entity_LevelData.Param p = new Entity_LevelData.Param ();
						
					cell = row.GetCell(0); p.stage = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(1); p.yogore_count = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(2); p.time_limit = (int)(cell == null ? 0 : cell.NumericCellValue);
						s.list.Add (p);
					}
					data.sheets.Add(s);
				}
			}

			ScriptableObject obj = AssetDatabase.LoadAssetAtPath (exportPath, typeof(ScriptableObject)) as ScriptableObject;
			EditorUtility.SetDirty (obj);
		}
	}
}
