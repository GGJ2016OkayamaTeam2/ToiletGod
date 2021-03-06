﻿using UnityEngine;
using System.Collections;
using System;

public class Yogore : MonoBehaviour, IErasable {
    
    // resource data
    [SerializeField] private Entity_YogoreData yogoreSheetData;
    private Entity_YogoreData.Param yogoreData;

    // yogore type
    private enum YogoreType { Zakkin, }
    [SerializeField] private YogoreType yogoreType;

    // hp
    private int _curHp;
    private int curHp
    {
        get { return _curHp; }
        set
        {
            _curHp = value >= yogoreData.max_hp ? yogoreData.max_hp : value;
            if(_curHp <= 0)
            {
                CleanUp();
            }
        }
    }

    // effect after clean up
    [SerializeField] private ParticleSystem cleanupEffect;
    [SerializeField] private int score = 100;

    void Awake()
    {
        var curIndex = (int)yogoreType;
        yogoreData = yogoreSheetData.sheets[0].list[curIndex];

        curHp = yogoreData.max_hp;
    }

    // Use this for initialization
    IEnumerator Start () {
        while(true)
        {
            if (curHp >= yogoreData.max_hp)
            {
                yield return new WaitForEndOfFrame();
                continue;
            }
            yield return new WaitForSeconds(yogoreData.recover_interval);
            curHp += yogoreData.recover_value;
        }
    }

    public void Erase(int force)
    {
        curHp -= force;
    }

    void CleanUp()
    {
        if (cleanupEffect)
        {
            var kira = Instantiate(cleanupEffect, transform.position, Quaternion.identity) as ParticleSystem;
            Destroy(kira.gameObject, kira.duration);
        }

        GameManager.GetGameManager().AddScore(score);
        YogoreManager.GetManager().DecYogoreCount();

        Destroy(gameObject);        
    }
}
