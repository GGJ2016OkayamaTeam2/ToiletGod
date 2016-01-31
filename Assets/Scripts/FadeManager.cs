using UnityEngine;
using System.Collections;

/// <summary>
/// Author Kondo.
/// bugfix ravencoding.
/// </summary>
public class FadeManager : SingletonMonoBehaviour<FadeManager> {
	/// <summary>暗転用黒テクスチャ</summary>
	private Texture2D blackTexture;
	/// <summary>フェード中の透明度</summary>
	private float fadeAlpha = 0;
	/// <summary>フェードのコルーチン実行中かどうか</summary>
	private bool isFading = false;

	public void Awake () {
		if (this != Instance) {
			Destroy (this);
			return;
		}

		DontDestroyOnLoad (this.gameObject);

		//ここで黒テクスチャを作る
		this.blackTexture = new Texture2D (32, 32, TextureFormat.RGB24, false);
		this.blackTexture.ReadPixels (new Rect (0, 0, 32, 32), 0, 0, false);
		this.blackTexture.SetPixel (0, 0, Color.white);
		this.blackTexture.Apply ();
	}

	public void OnGUI () {
		if (!this.isFading)
			return;

		//透明度を更新して黒テクスチャを描画
		GUI.color = new Color (0, 0, 0, this.fadeAlpha);
		GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), this.blackTexture);
	}

	/// <summary>
	/// 画面遷移
	/// </summary>
	/// <param name="scene">シーン名</param>
	/// <param name="interval">暗転にかかる時間(秒)</param>
	public void LoadLevel (string scene, float interval) {
		if (this.isFading) {
			//do nothing.
		} else {
			this.isFading = true;
			StartCoroutine (TransScene (scene, interval));
		}
	}

	///<summary>
	/// シーン遷移コルーチン
	/// </summary>
	/// <param name="scene">シーン名</param>
	/// <param name="interval">暗転にかかる時間(秒)</param>
	private IEnumerator TransScene (string scene, float interval) {
		yield return new WaitForEndOfFrame ();
		//だんだん暗く
		float time = 0;
		while (time <= interval) {
			this.fadeAlpha = Mathf.Lerp (0f, 1f, time / interval);
			time += Time.deltaTime;
			yield return 0;
		}

		//シーン切り替え
		Application.LoadLevel (scene);


		time = 0;
		while (time <= interval) {
			this.fadeAlpha = Mathf.Lerp (1f, 0f, time / interval);
			time += Time.deltaTime;
			yield return 0;
		}
		onFadeFinished ();
		this.isFading = false;
	}

	public void onFadeFinished(){
		GameObject go = GameObject.Find ("CutinText");
		if (go != null) {
            CutinAnimation cutin = go.transform.GetComponentInChildren<CutinAnimation>();
            cutin.execCutin ();
			Debug.Log("Find CutinText");
			//ここでCutinTextにあるゲームコンポーネントのCutinAnimationスクリプトの
			// execCutin()を呼び出せば、カットインが始まる.
		}
	}
}