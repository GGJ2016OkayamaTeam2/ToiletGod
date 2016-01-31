using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class ChangeEquipButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    [SerializeField] private GameObject zoukin;
    [SerializeField] private GameObject spray;
    [SerializeField] private Text sprayText;

    private HandController _handController;
    private HandController handController
    {
        get
        {
            if(!_handController)
            {
                _handController = FindObjectOfType<HandController>();
            }
            return _handController;
        }
    }

	public void OnClick()
    {
        Equip curEquip = handController.GetEquip();
        if(curEquip == Equip.Zoukin)
        {
            spray.SetActive(false);
            zoukin.SetActive(true);
            handController.SetEquip(Equip.Spray);
        } 
        else
        {
            sprayText.text = "x " + (GameManager.GetGameManager().getSprayRemain());
            spray.SetActive(true);
            zoukin.SetActive(false);
            handController.SetEquip(Equip.Zoukin);
        }
    }

    public void OnPointerEnter(PointerEventData data)
    {
        GameManager.GetGameManager().SetEnableSpray(false);
    }

    public void OnPointerExit(PointerEventData data)
    {
        GameManager.GetGameManager().SetEnableSpray(true);
    }
}
