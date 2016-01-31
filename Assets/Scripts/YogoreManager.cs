using UnityEngine;
using System.Collections.Generic;

public class YogoreManager : MonoBehaviour {
    
    [SerializeField] private Entity_LevelData levelSheetData;
    private Entity_LevelData.Param levelData;

    [SerializeField] private Vector3 center;
    [SerializeField] private float radius;

    // clamp settings -1 -> no clamp
    [SerializeField] private float maxZForBenza = 100;
    [SerializeField] private float minZForBenza = -1f;
    [SerializeField] private float maxXForBenza = -1;
    [SerializeField] private float minXForBenza = -1;

    [SerializeField] private float angleRandomMin = -5f;
    [SerializeField] private float angleRandomMax = 5f;

    // cache manager.
    private static YogoreManager _manager;
    public static YogoreManager GetManager()
    {
        if (!_manager)
        {
            _manager = FindObjectOfType<YogoreManager>() as YogoreManager;
            if (!_manager)
            {
                _manager = new GameObject("YogoreManager").AddComponent<YogoreManager>();
            }
        }
        return _manager;
    }

    [SerializeField] private Transform yogore;


    void Awake()
    {
        if (_manager && _manager != this)
        {
            Destroy(gameObject);
        }
    }

    // ===== DEBUG ======
    void Start()
    {
        InitStage(0);
    }


    public void InitStage(int index)
    {
        // select data
        levelData = levelSheetData.sheets[0].list[index];

        // instantiate YOGOREs
        var yogores = new List<Transform>();
        for (int i = 0; i < levelData.yogore_count; i++)
        {
            yogores.Add(Instantiate(yogore) as Transform);
        }

        // layout circlely.
        float angleDiff = 360f / yogores.Count;

        for (int i = 0; i < yogores.Count; i++)
        {
            var pos = center;

            float angle = (90 - angleDiff * i) * Mathf.Deg2Rad;
            angle += Random.Range(angleRandomMin, angleRandomMax);

            pos.x += radius * Mathf.Cos(angle);
            if (minXForBenza != -1) pos.x = Mathf.Max(pos.x, minXForBenza);
            if (maxXForBenza != -1) pos.x = Mathf.Min(pos.x, maxXForBenza);
            pos.x += Random.Range(-10, 10f);

            pos.z += radius * Mathf.Sin(angle);
            if (minZForBenza != -1) pos.z = Mathf.Max(pos.z, minZForBenza);
            if (maxZForBenza != -1) pos.z = Mathf.Min(pos.z, maxZForBenza);
            pos.z += Random.Range(-10, 10f);


            yogores[i].transform.position = pos;
        }
    }
}
