using UnityEngine;
using System.Collections;

public class CutinAnimation : MonoBehaviour {

	public GameObject target, target2;
	const float EASING = 0.05f;
	bool m_startAnimation = false;
	bool isStartSecondAnimation = false;

	// Use this for initialization
	void Start () {

	}

	void OnGUI()
	{
		if( GUI.Button( new Rect(20, 20, 100, 50), "Start" ) )
		{
			m_startAnimation = true;
		}
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
