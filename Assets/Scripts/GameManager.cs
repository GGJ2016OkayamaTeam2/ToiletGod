using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class GameManager : MonoBehaviour {
	public StopWatch stopWatch;
	public static Int32 scoreInt = 0;

	// Use this for initialization
	void Start () {
		stopWatch = new StopWatch ();//Default State is Zero.
		stopWatch.changeState ();//State: Zero state to Play state.
	}

	// Update is called once per frame
	void Update () {
		stopWatch.update ();
		UpdateTime ();
		UpdateScore ();
	} 

	void UpdateTime(){
		//timeText.text = "Time: " + stopWatch.getCurrentTimeString ();
		Debug.Log(stopWatch.getCurrentTimeString());
	}

	void UpdateScore(){
		//this.scoreText.text = "Score: " + HUDInGame.getCurrentTotalScore();
	}
}


public class StopWatch{
	public TextMesh timeText;
	public String timeString;
	StopwatchState state = StopwatchState.Zero;
	TimeSpan lastStopTimeSpan;
	DateTime startDateTime;
	Int32 timeBonus;

	enum StopwatchState {
		Zero,
		Play,
		Pause
	}

	public void update () {
		UpdateTime();
	}

	/*
	 * StopwatchState.Zero状態のときに本関数を呼び出すと、
	 * StopwatchState.Play状態に遷移する.
	 * StopwatchState.Play状態の時に本関数を呼び出すと、
	 * StopwatchState.Pause状態に遷移する.
	 * StopwatchState.Pause状態の時に本関数を呼び出すと、
	 * StopwatchState.Zero状態に遷移する.
	 * 
	 * TODO: ポーズから復帰した時にタイムが変にならないかどうかはまだ検証していません.
	 */
	public void changeState() {
		if (state == StopwatchState.Pause) {
			lastStopTimeSpan = new TimeSpan(0);
			startDateTime = DateTime.UtcNow;
			state = StopwatchState.Zero;
		} else if (state == StopwatchState.Play) {
			TimeSpan ts = DateTime.UtcNow - startDateTime;
			lastStopTimeSpan = ts + lastStopTimeSpan;
			state = StopwatchState.Pause;
		} else {
			startDateTime = DateTime.UtcNow;
			state = StopwatchState.Play;
		}
	}
	void UpdateTime() {
		TimeSpan currentTs;
		if (state == StopwatchState.Play) {
			TimeSpan ts = DateTime.UtcNow - startDateTime;
			currentTs = ts + lastStopTimeSpan;
		} else {
			currentTs = lastStopTimeSpan;
		}
		this.timeString = ConvertTimeSpanToString(currentTs);
		this.timeBonus = (Int32)(500000/currentTs.TotalSeconds);
	}
	static public string ConvertTimeSpanToString(TimeSpan ts) {
		if (ts.Hours > 0 || ts.Days > 0) {
			return string.Format("{0}:{1:D2}:{2:D2}.{3}", ts.Days * 24 + ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds.ToString("000").Substring(0, 2));
		} else {
			return string.Format("{0}:{1:D2}.{2}", ts.Minutes, ts.Seconds, ts.Milliseconds.ToString("000").Substring(0, 2));
		}

	}
	public string getCurrentTimeString(){
		return this.timeString;
	}
	public Int32 getTimeBonus(){
		return this.timeBonus;
	}
}