using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Entity_YogoreData : ScriptableObject
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
		
		public int id;
		public string name;
		public int max_hp;
		public int recover_interval;
		public int recover_value;
	}
}
