using UnityEngine;
using System.Collections;

public class button5 : MonoBehaviour
{

    public void NextScene()
    {
		GameManager.GetGameManager ().execSceneChange (GameManager.SceneState.Collection);
    }
}

