using UnityEngine;
using System.Collections;

public class button4 : MonoBehaviour
{

    public void NextScene()
    {
        GameManager.GetGameManager().StaySameRound();
		GameManager.GetGameManager ().execSceneChange (GameManager.SceneState.Toile);
    }
}
