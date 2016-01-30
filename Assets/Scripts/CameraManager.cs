using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Manage main camera.
/// </summary>
public class CameraManager : MonoBehaviour {

    // ----- FOR DEBUG -----
    [Header(" ===== DEBUG =====")]
    [SerializeField] private bool canMove = true;
    [SerializeField] private bool canRotate = true;
    [SerializeField] private bool canExpansion = true;

    [Serializable]
    private class CameraMoveData
    {
        [Header(" ----- MoveSettings ----")]
        public float moveSpeed = 3f;
        public Vector2 maxPos = Vector2.zero;
        public Vector2 minPos = Vector2.zero;

        [Header(" ----- RotationSetings ----- ")]
        public float rotSpeed = 5f;
        public Vector3 maxAngle = Vector3.zero;
        public Vector3 minAngle = Vector3.zero;
    }

    [Header(" ----- CameraMoveSettings ----- ")]
    [SerializeField] private CameraMoveData farCameraMoveSettings;
    [SerializeField] private CameraMoveData nearCameraMoveSettings;

    private bool isNear = false;

    // ------ expansion settings ------
    private enum PosDivision
    {
        Top, Center, Bottom
    }

    [Header(" ----- DestinationSettings ----- ")]
    [SerializeField] private Vector3 nearTopCameraPos = Vector3.zero;
    [SerializeField] private Vector3 nearCenterCameraPos = Vector3.zero;
    [SerializeField] private Vector3 nearBottomCameraPos = Vector3.zero;

    [Serializable]
    private class DestinationSettings {

    }
    
    [Header(" ------ Tween Settings ----- ")]
    [SerializeField] private AnimationCurve easeCurve = new AnimationCurve();
    [SerializeField] private float easeTime = 0.5f;
    private bool isTweening = false;

    private Vector3 lastMouseCenterPosition;
    private Vector3 lastMouseRightPosition;

    private Vector3 lastFarPosition;

    // Target cam.
    private Transform mainCamTransform;

    // cache manager.
    private static CameraManager _manager;
    public static CameraManager GetManager()
    {
        if(!_manager)
        {
            _manager = FindObjectOfType<CameraManager>() as CameraManager;
            if(!_manager)
            {
                _manager = new GameObject("CameraManager").AddComponent<CameraManager>();
            }
        }
        return _manager;
    }

    // Check manager.
    void Awake()
    {
        if (_manager && _manager != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // cache main camera.
        mainCamTransform = Camera.main.transform;
    }

	// Update is called once per frame
	void LateUpdate () {

        // movement
        if(canMove)
        {
            MoveCamera();
            ClampCamera();
        }

        // rotation
        if (canRotate)
        {
            RotateCamera();
        }

        // expansion and reduction
        if (canExpansion)
        {
            if (!isNear) ExpansionCamera();
            else ReductionCamera();
        }
    }

    // move camera horizontal and vertical.
    void MoveCamera()
    {
        if(Input.GetMouseButtonDown(2))
        {
            lastMouseCenterPosition = Input.mousePosition;
        }

        if(Input.GetMouseButton(2))
        {
            var delta = Input.mousePosition - lastMouseCenterPosition;
            if(isNear) mainCamTransform.Translate(-delta * nearCameraMoveSettings.moveSpeed * Time.deltaTime);
            else mainCamTransform.Translate(-delta * farCameraMoveSettings.moveSpeed * Time.deltaTime);
            lastMouseCenterPosition = Input.mousePosition;
        }
    }

    // clamp camera in level.
    void ClampCamera()
    {
        if (isTweening) return;
        var pos = mainCamTransform.position;
        if(isNear)
        {
            pos.x = Mathf.Clamp(pos.x, nearCameraMoveSettings.minPos.x, nearCameraMoveSettings.maxPos.x);
            pos.y = Mathf.Clamp(pos.y, nearCameraMoveSettings.minPos.y, nearCameraMoveSettings.maxPos.y);
        }
        else
        {
            pos.x = Mathf.Clamp(pos.x, farCameraMoveSettings.minPos.x, farCameraMoveSettings.maxPos.x);
            pos.y = Mathf.Clamp(pos.y, farCameraMoveSettings.minPos.y, farCameraMoveSettings.maxPos.y);
        }
        mainCamTransform.position = pos;
    }

    void RotateCamera()
    {
        if(Input.GetMouseButtonDown(1))
        {
            lastMouseRightPosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(1))
        {
            var delta = Input.mousePosition - lastMouseRightPosition;
            var curAngle = mainCamTransform.eulerAngles;

            var rotSpeed = isNear ? nearCameraMoveSettings.rotSpeed : farCameraMoveSettings.rotSpeed;
            curAngle.x -= delta.y * rotSpeed * Time.deltaTime;
            curAngle.y += delta.x * rotSpeed * Time.deltaTime;

            mainCamTransform.eulerAngles = curAngle;
            lastMouseRightPosition = Input.mousePosition;
        }
    }

    void ExpansionCamera()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        if (isTweening) return;

        // save last camera position at far.
        lastFarPosition = mainCamTransform.position;

        var div = CheckMouseDiv(Input.mousePosition);
        StartCoroutine(ExpansionCor(div));
    }

    IEnumerator ExpansionCor(PosDivision div)
    {
        var startTime = Time.time;
        var initPos = mainCamTransform.position;
        var destPos = Vector3.zero;

        isTweening = true;
        
        switch (CheckMouseDiv(Input.mousePosition))
        {
            case PosDivision.Top: destPos = nearTopCameraPos; break;
            case PosDivision.Center: destPos = nearCenterCameraPos; break;
            case PosDivision.Bottom: destPos = nearBottomCameraPos; break;
        }

        float sec = 0.5f;

        while (Time.time - startTime < sec)
        {
            float rate = (Time.time - startTime) / sec;
            var pos = easeCurve.Evaluate(rate);
            mainCamTransform.position = Vector3.Lerp(initPos, destPos, pos);
            yield return new WaitForEndOfFrame();
        }

        mainCamTransform.position = destPos;

        isTweening = false;
        isNear = true;
    }

    void ReductionCamera()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        if (isTweening) return;

        StartCoroutine(ReductionCor());
    }

    IEnumerator ReductionCor()
    {
        var startTime = Time.time;
        var initPos = mainCamTransform.position;
        var destPos = lastFarPosition;
        
        isTweening = true;

        while (Time.time - startTime < easeTime)
        {
            float rate = (Time.time - startTime) / easeTime;
            var pos = easeCurve.Evaluate(rate);
            mainCamTransform.position = Vector3.Lerp(initPos, destPos, pos);
            yield return new WaitForEndOfFrame();
        }

        mainCamTransform.position = destPos;

        isTweening = false;
        isNear = false;
    }
    
    PosDivision CheckMouseDiv(Vector3 mousePos)
    {
        var screenMousePos = Camera.main.ScreenToViewportPoint(mousePos);
        if(screenMousePos.y >= 0.66f)
        {
            return PosDivision.Top;
        }
        else if(screenMousePos.y >= 0.33f)
        {
            return PosDivision.Center;
        }
        else
        {
            return PosDivision.Bottom;
        }
    }
}
