using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class startCountDown : MonoBehaviour {
	public Text timelimit;

	// Use this for initialization
	void Start () {
		this.timelimit.text = "TimeLimit: 120";
	}
	
	// Update is called once per frame
	void Update () {
		this.timelimit.text = "TimeLimit: " + GameManager.GetGameManager ().getCurrentTimeLimitStr ();
	}
}
