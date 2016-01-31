using UnityEngine;
using UnityEditor;
using UnityEngine.Audio;
using System.Collections.Generic;

[System.Serializable]
public class DictionaryOfStringAudioClip : SerializableDicionary<string, AudioClip> { }
[System.Serializable]
public class DictionaryOfStringMixer : SerializableDicionary<string, AudioMixer> { }
[System.Serializable]
public class DictionaryOfStringGroup : SerializableDicionary<string, AudioMixerGroup> { }

[CreateAssetMenu( fileName = "AudioData", menuName = "Create AudioData" )]
public class AudioData : ScriptableObject {

    public string bgmPath = "Assets/Audio/BGM";
    public string sePath = "Assets/Audio/SE";
    public string mixerGroupPath = "Assets/Audio/Mixer";
    
    [HideInInspector]
    public DictionaryOfStringAudioClip bgmDictionary = new DictionaryOfStringAudioClip();
    [HideInInspector]
    public DictionaryOfStringAudioClip seDictionary = new DictionaryOfStringAudioClip();
    [HideInInspector]
    public DictionaryOfStringMixer mixerDictionary = new DictionaryOfStringMixer();
    [HideInInspector]
    public DictionaryOfStringGroup groupDictionary = new DictionaryOfStringGroup();

#if UNITY_EDITOR

    [HideInInspector]
    public List<string> bgmFilePathList = new List<string>();

    [HideInInspector]
    public List<string> seFilePathList = new List<string>();

    public void RegisterBGM(string bgmName, AudioClip bgmClip)
    {
        if (bgmDictionary.ContainsKey(bgmName)) return;

        bgmDictionary.Add(bgmName, bgmClip);
        EditorUtility.SetDirty(this);
    }

    public void RemoveBGM(string bgmName)
    {
        if (!bgmDictionary.ContainsKey(bgmName)) return;

        bgmDictionary.Remove(bgmName);
        EditorUtility.SetDirty(this);
    }

    public void RegisterSE(string seName, AudioClip seClip)
    {
        if (seDictionary.ContainsKey(seName)) return;

        seDictionary.Add(seName, seClip);
        EditorUtility.SetDirty(this);
    }

    public void RemoveSE(string seName)
    {
        if (!seDictionary.ContainsKey(seName)) return;

        seDictionary.Remove(seName);
        EditorUtility.SetDirty(this);
    }

    public void RegisterMixer(string mixerName, AudioMixer mixer)
    {
        if (mixerDictionary.ContainsKey(mixerName)) return;
        mixerDictionary.Add(mixerName, mixer);
        EditorUtility.SetDirty(this);
    }

    public void RemoveMixer(string mixerName)
    {
        if (!mixerDictionary.ContainsKey(mixerName)) return;

        mixerDictionary.Remove(mixerName);
        EditorUtility.SetDirty(this);
    }

    public void RegisterGroup(string groupName, AudioMixerGroup group)
    {
        if (groupDictionary.ContainsKey(groupName)) return;
        groupDictionary.Add(groupName, group);
        EditorUtility.SetDirty(this);
    }

    public void ClearGroup()
    {
        groupDictionary.Clear();
        EditorUtility.SetDirty(this);
    }

    public void RemoveGroup(string groupName)
    {
        if (!groupDictionary.ContainsKey(groupName)) return;

        groupDictionary.Remove(groupName);
        EditorUtility.SetDirty(this);
    }

#endif
    
    public AudioClip GetBGM(string bgmName)
    {
        return bgmDictionary[bgmName];
    }

    public AudioClip GetSE(string seName)
    {
        return seDictionary[seName];
    }

    public AudioMixer GetMixer(string mixerName)
    {
        return mixerDictionary[mixerName];
    }

    public AudioMixerGroup GetGroup(string groupName)
    {
        return groupDictionary[groupName];
    }
}
