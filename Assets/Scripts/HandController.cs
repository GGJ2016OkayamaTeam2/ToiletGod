using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using UnityEngine.EventSystems;

public enum Equip
{
    Zoukin,
    Spray
}

public class HandController : MonoBehaviour {
	[SerializeField] private RectTransform handItem;
	private Vector3 screenPos;
	private Vector3 worldPos;
    //public Renderer rend;

    private Vector3 lastRayPos;
    private float rayMovedMag;
    [SerializeField] private float threshold = 2f;
    [SerializeField] private float zoukinRadius = 1f;
    [SerializeField] private int zoukinForce = 3;

    [SerializeField] private float sprayRadius = 5;
    [SerializeField] private int sprayForce = 10;

    [SerializeField] private ParticleSystem sprayParticle = null;
    [SerializeField] private Vector3 sprayOffs = Vector3.zero;

    //[SerializeField] private RectTransform sprayImage;

    [SerializeField] private GameObject spray;
    [SerializeField] private GameObject zoukin;

    [SerializeField] private Image sprayQuanImage;

    private Canvas rootCanvas;

    private Equip equip;


    public Equip GetEquip()
    {
        return equip;
    }

    public void SetEquip(Equip equip)
    {
        this.equip = equip;
        changeCursor();
    }

    void changeCursor()
    {
        if(equip == Equip.Zoukin)
        {
            spray.SetActive(false);
            zoukin.SetActive(true);
            lastRayPos = Vector3.zero;
        }
        else
        {
            spray.SetActive(true);
            zoukin.SetActive(false);
        }
    }

	// Use this for initialization
	void Start () {
        SetEquip(Equip.Spray);
        //this.handItem = GameObject.Find ("HandItem");

        //renderer.
        //rend = GetComponent<Renderer>();
        rootCanvas = handItem.root.GetComponent<Canvas>();

        ChangeQuan(6);
    }


	// Update is called once per frame
	void Update () {
        
        // Hand Item positon move to cursor position
		this.screenPos = Input.mousePosition;
		this.screenPos.z = 10f;
        this.worldPos = Camera.main.ScreenToWorldPoint (this.screenPos);
        //this.handItem.position = this.worldPos;
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rootCanvas.transform as RectTransform, Input.mousePosition, rootCanvas.worldCamera, out pos);
        transform.position = rootCanvas.transform.TransformPoint(pos);

        if(!GameManager.GetGameManager().CanUseSpray())
        {
            return;
        }

        // ----- ZOUKIN -----
        // raycast for erase YOGORE

        if(equip == Equip.Zoukin)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                IErasable[] erasables;

                erasables = Physics.SphereCastAll(ray, zoukinRadius, Mathf.Infinity)
                    .Select(t => t.transform.GetComponent<IErasable>())
                    .ToArray();

                foreach (var erasable in erasables)
                {
                    if (erasable != null)
                    {
                        erasable.Erase(zoukinForce);
                    }
                }
            }

            if (Input.GetMouseButton(0))
            {
                if (lastRayPos == Vector3.zero)
                {
                    lastRayPos = worldPos;
                }

                rayMovedMag += (lastRayPos - worldPos).magnitude;
                lastRayPos = worldPos;

                if (rayMovedMag >= threshold)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    IErasable[] erasables;

                    erasables = Physics.SphereCastAll(ray, zoukinRadius, Mathf.Infinity)
                        .Select(t => t.transform.GetComponent<IErasable>())
                        .ToArray();

                    foreach (var erasable in erasables)
                    {
                        if (erasable != null)
                        {
                            erasable.Erase(zoukinForce);
                        }
                    }
                    rayMovedMag = 0;
                }
            }

            if (Input.GetMouseButtonUp(0)) lastRayPos = Vector3.zero;
        }
        else
        {
            // ----- SPRAY -----
            if (Input.GetMouseButtonDown(0))
            {
                if (GameManager.GetGameManager().getSprayRemain() > 0)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    IErasable[] erasables;

                    var targets = Physics.SphereCastAll(ray, sprayRadius, Mathf.Infinity);

                    erasables = targets
                        .Select(t => t.transform.GetComponent<IErasable>())
                        .ToArray();

                    //var duration = 0f;

                    /*
                    if (erasables != null && erasables.Length >= 1)
                    {
                        var sprayPos = transform.position + sprayOffs;
                        sprayPos.y = sprayParticle.transform.position.y;

                        var spray = Instantiate(sprayParticle, sprayPos / 10, sprayParticle.transform.rotation) as ParticleSystem;
                        duration = spray.duration;
                        Destroy(spray, duration);
                    }*/

                    //var simage = Instantiate(sprayImage, transform.position + new Vector3(60, 30, 0), sprayImage.rotation) as RectTransform;
                    //simage.parent = transform.parent;
                    //simage.SetParent(transform.parent);
                    //Destroy(simage.gameObject, duration);

                    foreach (var erasable in erasables)
                    {
                        if (erasable != null)
                        {
                            erasable.Erase(sprayForce);
                        }
                    }

                    GameManager.GetGameManager().decSprayCount();
                    ChangeQuan( GameManager.GetGameManager().getSprayRemain());

                    if(GameManager.GetGameManager().getSprayRemain() <= 0)
                    {
                        GameObject.Find("ChangeButton").GetComponent<ChangeEquipButton>().OnClick();
                    }
                }
            }
        }
    }

    bool cantUse;

    public void ChangeQuan(int newquan)
    {
        if (cantUse) return;

        if (newquan <= 0) cantUse = true;
        float rate = (float)newquan / 6f;

        sprayQuanImage.fillAmount = rate;
    }
}
