using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Entity_LevelData : ScriptableObject
{	
	public List<Sheet> sheets = new List<Sheet> ();

	[System.SerializableAttribute]
	public class Sheet
	{
		public string name = string.Empty;
		public List<Param> list = new List<Param>();
	}

	[System.SerializableAttribute]
	public class Param
	{
		
		public int stage;
		public int yogore_count;
		public int time_limit;
	}
}

