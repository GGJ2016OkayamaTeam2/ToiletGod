using UnityEngine;
using System.Collections;

public class button2 : MonoBehaviour
{

    public void NextScene()
    {
		GameManager.GetGameManager ().execSceneChange (GameManager.SceneState.Main);
    }
}
