using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class GameManager : MonoBehaviour {
	//public StopWatch stopWatch;
	public int sprayUsageRemain;
	private int curSprayRemain;

    private int round = 0;
    private int score = 0;

    [SerializeField] private Entity_LevelData levelDataSheet;
    private Entity_LevelData.Param levelData;

    private float timeLimit;


	public enum SceneState{
		Main,
		Credit,
		Collection,
		Toile,
		Toile1,
		Result,
		Result2
	}

    private SceneState currentState;

	//singleton.
	private static GameManager _gm;
	public static GameManager GetGameManager(){
		if (!_gm) {
			_gm = FindObjectOfType<GameManager> () as GameManager;
			if (!_gm) {
				_gm = new GameObject ("GameManager").AddComponent<GameManager> ();
			}
		}
		return _gm;
	}

    void Awake()
    {
        if(_gm && _gm != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    void InitLevelData()
    {
        levelData = levelDataSheet.sheets[0].list[round];
        timeLimit = levelData.time_limit;
        //stopWatch = new StopWatch(levelData.time_limit);
    }

	// Use this for initialization
	void Start () {
        InitLevelData();

		//stopWatch = new StopWatch ();//Default State is Zero.

		//stopWatch.changeState ();//State: Zero state to Play state.
		this.curSprayRemain = this.sprayUsageRemain;
	}

	// Update is called once per frame
	void Update () {
		//stopWatch.update ();
		UpdateTime ();
		UpdateScore ();
	} 

	private void UpdateTime(){
        if(currentState == SceneState.Toile)
        {
            timeLimit -= Time.deltaTime;
            if (timeLimit <= 0)
            {
                execSceneChange(SceneState.Result);
            }
        }

		//Debug.Log(stopWatch.getCurrentTimeString());
	}

    public float GetCurrentTime()
    {
        return timeLimit;
    }

	private void UpdateScore(){

	}

    public void AddScore(int add)
    {
        score += add;
    }

    public int GetScore()
    {
        return score;
    }

    public void GoNextRound()
    {
        round++;
        InitLevelData();
    }

    public void StaySameRound()
    {
        InitLevelData();
    }

	//public String getCurrentTimeLimitStr(){
	//	return stopWatch.getCurrentTimeString ();
	//}
		
	public void execSceneChange(SceneState state){
        currentState = state;

        switch (state){
		case SceneState.Main:
			FadeManager.Instance.LoadLevel ("main", 1);
			break;
		case SceneState.Toile:
			FadeManager.Instance.LoadLevel ("Toire", 1);
			break;
		case SceneState.Toile1:
			FadeManager.Instance.LoadLevel ("Toire 1", 1);
			break;
		case SceneState.Result:
			FadeManager.Instance.LoadLevel ("result", 1);
			break;
		case SceneState.Collection:
			FadeManager.Instance.LoadLevel ("Collection", 1);
			break;
		default:
			break;
		}
	}

	public void decSprayCount(){
		this.curSprayRemain--;
	}

	public int getSprayRemain(){
		return this.curSprayRemain;
	}
}

/*
public class StopWatch{
	public TextMesh timeText;
	public String timeString;
	StopwatchState state = StopwatchState.Zero;
	TimeSpan lastStopTimeSpan;
	DateTime startDateTime;
	Int32 timeBonus;

	public int secTimeLimit = 120;

	private TimeSpan timeLimit;

	enum StopwatchState {
		Zero,
		Play,
		Pause
	}

	public StopWatch(){
		this.timeLimit = new TimeSpan(0, 0, this.secTimeLimit);//0時間5分0秒.
	}

    public StopWatch(int limitSec)
    {
        this.timeLimit = new TimeSpan(0, 0, limitSec);
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
     /*
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
		TimeSpan timeRemaining = this.timeLimit.Subtract(currentTs);
		this.timeString = ConvertTimeSpanToString(timeRemaining);
		this.timeBonus = (Int32)(500000/currentTs.TotalSeconds);
	}

	//表示の見た目.
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
}*/