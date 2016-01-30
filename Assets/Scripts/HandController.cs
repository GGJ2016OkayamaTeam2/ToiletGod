using UnityEngine;
using System.Collections;

public class HandController : MonoBehaviour {
	public GameObject handItem;
	private Vector3 screenPos;
	private Vector3 worldPos;
	public Renderer rend;

	// Use this for initialization
	void Start () {
		this.handItem = GameObject.Find ("HandItem");
		//renderer.
		rend = GetComponent<Renderer>();
	
	}
	
	// Update is called once per frame
	void Update () {
		this.screenPos = Input.mousePosition;
		this.screenPos.z = 10f;
		this.worldPos = Camera.main.ScreenToWorldPoint (this.screenPos);
		this.handItem.transform.position = this.worldPos;
		Debug.Log (this.worldPos);

		// render material.
		if (Input.GetMouseButton(0)) {
			this.rend.sharedMaterial.color = Color.gray;
		} else {
			this.rend.sharedMaterial.color = Color.blue;
		}
	}
}
