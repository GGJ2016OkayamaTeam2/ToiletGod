using UnityEngine;
using System.Collections;

public class button7 : MonoBehaviour
{

    public void NextScene()
    {
		GameManager.GetGameManager ().execSceneChange (GameManager.SceneState.Result);
    }
}
