using UnityEngine;
using System.Collections;
using System.Linq;

public class HandController : MonoBehaviour {
	[SerializeField] private Transform handItem;
	private Vector3 screenPos;
	private Vector3 worldPos;
    //public Renderer rend;

    private Vector3 lastRayPos;
    private float rayMovedMag;
    [SerializeField] private float threshold = 2f;

    [SerializeField] private int force = 3;

	// Use this for initialization
	void Start () {
		//this.handItem = GameObject.Find ("HandItem");
		
        //renderer.
		//rend = GetComponent<Renderer>();
	
	}
	
	// Update is called once per frame
	void Update () {
        
        // Hand Item positon move to cursor position
		this.screenPos = Input.mousePosition;
		this.screenPos.z = 10f;
		this.worldPos = Camera.main.ScreenToWorldPoint (this.screenPos);
		this.handItem.position = this.worldPos;

        // raycast for erase YOGORE
        if(Input.GetMouseButton(0))
        {
            if(lastRayPos == Vector3.zero)
            {
                lastRayPos = worldPos;
            }

            rayMovedMag += (lastRayPos - worldPos).magnitude;
            lastRayPos = worldPos;

            if (rayMovedMag >= threshold)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                IErasable[] erasables;

                erasables = Physics.RaycastAll(ray, Mathf.Infinity)
                    .Select(t => t.transform.GetComponent<IErasable>())
                    .ToArray();                
                foreach (var erasable in erasables)
                {
                    if(erasable != null)
                    {
                        erasable.Erase(force);
                    }
                }
                rayMovedMag = 0;
            }
        }

        if (Input.GetMouseButtonUp(0)) lastRayPos = Vector3.zero;
    }
}
