using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Xml.Serialization;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

public class YogoreData_importer : AssetPostprocessor {
	private static readonly string filePath = "Assets/ParamData/YogoreData.xls";
	private static readonly string exportPath = "Assets/ParamData/YogoreData.asset";
	private static readonly string[] sheetNames = { "YogoreData", };
	
	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		foreach (string asset in importedAssets) {
			if (!filePath.Equals (asset))
				continue;
				
			Entity_YogoreData data = (Entity_YogoreData)AssetDatabase.LoadAssetAtPath (exportPath, typeof(Entity_YogoreData));
			if (data == null) {
				data = ScriptableObject.CreateInstance<Entity_YogoreData> ();
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

					Entity_YogoreData.Sheet s = new Entity_YogoreData.Sheet ();
					s.name = sheetName;
				
					for (int i=1; i<= sheet.LastRowNum; i++) {
						IRow row = sheet.GetRow (i);
						ICell cell = null;
						
						Entity_YogoreData.Param p = new Entity_YogoreData.Param ();
						
					cell = row.GetCell(0); p.id = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(1); p.name = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(2); p.max_hp = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(3); p.recover_interval = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(4); p.recover_value = (int)(cell == null ? 0 : cell.NumericCellValue);
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
