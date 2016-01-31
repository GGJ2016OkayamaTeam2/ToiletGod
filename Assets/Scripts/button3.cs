using UnityEngine;
using System.Collections;

public class button3 : MonoBehaviour
{

    public void NextScene()
    {
        GameManager.GetGameManager().GoNextRound();
		GameManager.GetGameManager ().execSceneChange (GameManager.SceneState.Toile);
    }
}