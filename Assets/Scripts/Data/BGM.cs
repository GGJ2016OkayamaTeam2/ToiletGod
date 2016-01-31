using UnityEngine;

public static class BGM
{
    static AudioData _audioData;
    static AudioData audioData { get { return _audioData = _audioData ?? (_audioData = Resources.Load<AudioData>("AudioData")); } }

    public static AudioClip BGM1 { get { return audioData.GetBGM("BGM1"); } }
    public static AudioClip clap_excellent { get { return audioData.GetBGM("clap_excellent"); } }
    public static AudioClip clap { get { return audioData.GetBGM("clap"); } }
    public static AudioClip miss { get { return audioData.GetBGM("miss"); } }
}
