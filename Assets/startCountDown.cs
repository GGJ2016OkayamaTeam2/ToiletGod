using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class startCountDown : MonoBehaviour {
	public Text timelimit;
    public Text scoreText;

	// Use this for initialization
	void Start () {
		this.timelimit.text = "TimeLimit: 120";
	}
	
	// Update is called once per frame
	void Update () {
        float currentTime = GameManager.GetGameManager().GetCurrentTime();
        if (currentTime > 0)
        {
            timelimit.text = "TimeLimit : " + currentTime.ToString("00.0");
        }
        else
        {
            timelimit.text = "TimeLimit : 0.0";
        }

        scoreText.text = "Score : " + GameManager.GetGameManager().GetScore();

        //this.timelimit.text = "TimeLimit: " + GameManager.GetGameManager ().getCurrentTimeLimitStr ();
    }
}
