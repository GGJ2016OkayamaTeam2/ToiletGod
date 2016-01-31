using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CutinAnimationImage : MonoBehaviour {
	public Image cutinImage;
	public Sprite cutin1;
	public Sprite cutin2;
	public Sprite cutin3;
	public Sprite cutin4;
	public GameObject target, target2;
	const float EASING = 0.05f;
	bool m_startAnimation = true;
	bool isStartSecondAnimation = false;


	// Use this for initialization
	void Start () {
		int round = GameManager.GetGameManager ().getCurRoundId ();
		switch (round) {
		case 0:
			this.cutinImage.sprite = this.cutin1;
			break;
		case 1:
			this.cutinImage.sprite = this.cutin2;
			break;
		case 2:
			this.cutinImage.sprite = this.cutin3;
			break;
		default:
			this.cutinImage.sprite = this.cutin4;
			break;
		}
	}

	private void setImage(Image img){
		this.cutinImage = img;
	}

	public void execCutin(){
		this.m_startAnimation = true;
	}

	// Update is called once per frame
	void Update () 
	{
		// ボタンが押されたらアニメーションスタート
		if( !m_startAnimation ) return;

		// 2点間の距離を速度に反映する
		Vector3 diff = target.transform.position - transform.position;
		Vector3 v = diff * EASING;
		transform.position += v;

		// 十分近づいたらアニメーション終了
		if( diff.magnitude < 2.01f && !this.isStartSecondAnimation) 
		{
			Debug.Log("END1");
			//m_startAnimation = false;
			this.isStartSecondAnimation = true;
		}
		if (this.isStartSecondAnimation) {
			Vector3 diff2 = target2.transform.position - transform.position;
			Vector3 v2 = diff2 * EASING;
			transform.position += v2;
			if( diff2.magnitude < 1.01f && this.isStartSecondAnimation) 
			{
				Debug.Log("END2");
				m_startAnimation = false;
				this.isStartSecondAnimation = false;
			}
		}
	}
}
