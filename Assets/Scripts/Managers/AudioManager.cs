using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioListener))]
public class AudioManager : MonoSingleton<AudioManager> {

    private const int BgmSourceSize = 2;

    [SerializeField]
    private int seSourceSize = 3;
    private int sourceSize;

    [SerializeField]
    private int playableSECount = 5;

    private Dictionary<AudioSource, int> seDict;

    [SerializeField, Range(0, 1f)]
    private float volume;

    private AudioSource[] sources;
    private AudioSource currentBGMSource;

    void Awake()
    {
        Init();

        sourceSize = BgmSourceSize + seSourceSize;
        sources = new AudioSource[sourceSize];
        
        //Initialize BGM sources
        for (int i = 0; i < BgmSourceSize; i++)
        {
            sources[i] = gameObject.AddComponent<AudioSource>() as AudioSource;
            sources[i].volume = 0.0f;
            sources[i].loop = true;
        }

        currentBGMSource = sources[0];

        //Initialize SE sources
        seDict = new Dictionary<AudioSource, int>();

        for (int i = BgmSourceSize; i < sourceSize; i++)
        {
            sources[i] = gameObject.AddComponent<AudioSource>() as AudioSource;
            sources[i].volume = 0.0f;
            seDict.Add(sources[i], 0);
        }
    }

    // --- Play ---
    public void PlayBGM(AudioClip clip, AudioMixerGroup group = null)
    {
        if (clip == currentBGMSource.clip) return;

        StopBGM();
        currentBGMSource.clip = clip;
        currentBGMSource.volume = volume;
        currentBGMSource.outputAudioMixerGroup = group;
        currentBGMSource.Play();
    }
    
    public void PlaySE(AudioClip clip, AudioMixerGroup group = null)
    {
        for (int i = BgmSourceSize; i < sourceSize; i++)
        {
            //同じクリップが指定された場合.
            if (sources[i].clip == clip)
            {
                //現在の再生数が許容範囲の場合.
                if (seDict[sources[i]] < playableSECount)
                {
                    sources[i].PlayOneShot(clip);

                    //再生数更新.
                    seDict[sources[i]]++;

                    //クリップの長さだけ待って数を減らす.
                    float length = clip.length;
                    StartCoroutine(DecSECountCor(i, length));
                }
                break;
            }

            //違うクリップが指定された場合.
            else
            {
                //ソースが別のクリップに使用されていないかチェック.
                if (seDict[sources[i]] == 0)
                {
                    sources[i].clip = clip;
                    sources[i].volume = volume;
                    sources[i].outputAudioMixerGroup = group;
                    sources[i].PlayOneShot(clip);
                    
                    //再生数更新.
                    seDict[sources[i]]++;

                    //クリップの長さだけ待って数を減らす.
                    float length = clip.length;
                    StartCoroutine(DecSECountCor(i, length));
                    
                    break;
                }
            }
        }
    }

    IEnumerator DecSECountCor(int index, float length)
    {
        yield return new WaitForSeconds(length);

        seDict[sources[index]]--;
    }

    // --- Pause ---
    public void PauseAll()
    {
        PauseBGM();
        PauseSE();
    }

    public void PauseBGM()
    {
        for (int i = 0; i < BgmSourceSize; i++)
        {
            if (sources[i].time != 0)
                sources[i].Pause();
        }
    }

    public void PauseSE()
    {
        for (int i = BgmSourceSize; i < sourceSize; i++)
        {
            if(sources[i].time != 0)
                sources[i].Pause();
        }
    }

    // --- Resume ---
    public void ResumeAll()
    {
        ResumeBGM();
        ResumeSE();
    }

    public void ResumeBGM()
    {
        for (int i = 0; i < BgmSourceSize; i++)
        {
            if (sources[i].time != 0)
                sources[i].Play();
        }
    }

    public void ResumeSE()
    {
        for (int i = BgmSourceSize; i < sourceSize; i++)
        {
            if (sources[i].time != 0)
                sources[i].Play();
        }
    }

    // --- Stop ---
    public void StopAll()
    {
        StopBGM();
        StopSE();
    }

    public void StopBGM()
    {
        for (int i = 0; i < BgmSourceSize; i++)
        {
            sources[i].Stop();
        }
    }

    public void StopSE()
    {
        for (int i = BgmSourceSize; i < sourceSize; i++)
        {
            sources[i].Stop();
        }
    }

    // --- Fade In ---
    public void FadeinBGM(AudioClip clip, float fadeSec = 1.0f, AudioMixerGroup group = null)
    {
        if (currentBGMSource.clip != clip)
        {
            StartCoroutine(FadeInCor(clip, 0, fadeSec, group));
        }
    }

    public void FadeInBGMDelay(AudioClip clip, float delaySec = 1.0f, float fadeSec = 1.0f, AudioMixerGroup group = null)
    {
        if (currentBGMSource.clip != clip)
        {
            StartCoroutine(FadeInCor(clip, delaySec, fadeSec, group));
        }
    }

    private IEnumerator FadeInCor(AudioClip clip, float delaySec, float fadeSec, AudioMixerGroup group)
    {
        yield return new WaitForSeconds(delaySec);

        if (currentBGMSource == null)
        {
            currentBGMSource = sources[0];
        }
        AudioSource src = currentBGMSource;

        src.clip = clip;
        src.volume = 0.0f;
        src.outputAudioMixerGroup = group;
        src.Play();

        float tStart = Time.time;

        while (Time.time - tStart < fadeSec)
        {
            float rate = (Time.time - tStart) / fadeSec;
            src.volume = volume * rate;
            yield return new WaitForEndOfFrame();
        }
        currentBGMSource.volume = volume;
    }

    // --- Fade Out ---
    public void FadeoutBGM(float fadeSec = 1.0f)
    {
        StartCoroutine(FadeOutCor(0, fadeSec));
    }

    public void FadeOutBGMDelay(float delaySec = 1.0f, float fadeSec = 1.0f)
    {
        StartCoroutine(FadeOutCor(delaySec, fadeSec));
    }

    private IEnumerator FadeOutCor(float delaySec, float fadeSec)
    {
        yield return new WaitForSeconds(delaySec);

        float tStart = Time.time;

        AudioSource src = currentBGMSource;

        float initialVolume = src.volume;

        while (Time.time - tStart < fadeSec)
        {
            float rate = (Time.time - tStart) / fadeSec;
            src.volume = initialVolume * (1.0f - rate);
            yield return new WaitForEndOfFrame();
        }
        src.volume = 0.0f;
        src.Pause();
    }

    // --- Cross Fade ---
    public void CrossFade(AudioClip clip, float fadeSec = 1.0f, AudioMixerGroup group = null)
    {
        StartCoroutine(CrossFadeCor(clip, 0, fadeSec, group, false));
    }

    public void CrossFadeDelay(AudioClip clip, float delaySec = 1.0f, float fadeSec = 1.0f, AudioMixerGroup group = null)
    {
        StartCoroutine(CrossFadeCor(clip, delaySec, fadeSec, group, false));
    }

    public void CrossFadeSync(AudioClip clip, float delaySec = 0f, float fadeSec = 1.0f, AudioMixerGroup group = null)
    {
        StartCoroutine(CrossFadeCor(clip, delaySec, fadeSec, group, true));
    }

    private IEnumerator CrossFadeCor(AudioClip clip, float delaySec,float fadeSec, AudioMixerGroup group, bool sync)
    {
        if (currentBGMSource.clip == clip) yield break;

        yield return new WaitForSeconds(delaySec);

        if (currentBGMSource == null)
        {
            currentBGMSource = sources[0];
        }

        AudioSource fadeout = currentBGMSource;
        AudioSource fadein = (currentBGMSource == sources[0]) ? sources[1] : sources[0];

        fadein.clip = clip;
        fadein.volume = 0.0f;
        fadein.outputAudioMixerGroup = group;
        if (sync) fadein.time = fadeout.time;
        fadein.Play();

        float initialVolume = fadeout.volume;
        float tStart = Time.time;

        while (Time.time - tStart < fadeSec)
        {
            float rate = (Time.time - tStart) / fadeSec;
            fadein.volume = volume * rate;
            fadeout.volume = initialVolume * (1.0f - rate);
            yield return new WaitForEndOfFrame();
        }
        currentBGMSource = fadein;
        currentBGMSource.volume = volume;

        fadeout.Pause();
    }
}
