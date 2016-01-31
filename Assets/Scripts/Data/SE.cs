using UnityEngine;

public static class SE
{
    static AudioData _audioData;
    static AudioData audioData { get { return _audioData = _audioData ?? (_audioData = Resources.Load<AudioData>("AudioData")); } }

}
