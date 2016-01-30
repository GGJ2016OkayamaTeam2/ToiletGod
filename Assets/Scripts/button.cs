using UnityEngine;
using System.Collections;

public class button : MonoBehaviour
{

    public void NextScene()
    {
		GameManager.GetGameManager ().execSceneChange (GameManager.SceneState.Credit);
    }
}
