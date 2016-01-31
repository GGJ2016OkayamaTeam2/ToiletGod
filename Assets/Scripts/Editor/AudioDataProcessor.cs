using UnityEngine;
using UnityEngine.Audio;
using UnityEditor;
using System.IO;
using System.Text;
using System.Collections.Generic;

/// <summary>
/// フォルダを指定し、そのフォルダ内にオーディオクリップ、またはオーディオミキサーが入ったら再生準備を行う.
/// </summary>
public class AudioDataProcessor : AssetPostprocessor
{
    private enum AudioType { BGM, SE }

    // --- BGM Path ---
    private const string BgmFilePath  = "Assets/Scripts/Data/BGM.cs";
    private static readonly string BgmFilenameNoExtension = Path.GetFileNameWithoutExtension(BgmFilePath);

    // --- SE Path ---
    private const string SeFilePath = "Assets/Scripts/Data/SE.cs";
    private static readonly string SeFilenameNoExtension = Path.GetFileNameWithoutExtension(SeFilePath);

    // --- AudioMixerGroup Path ---
    private const string MixerGroupFilePath = "Assets/Scripts/Data/MixerGroup.cs";
    private static List<AudioMixerGroup> groupFileList = new List<AudioMixerGroup>();

    // --- Audio Data ---
    static AudioData _audioData;
    static AudioData audioData { get { return _audioData = _audioData ?? (_audioData = Resources.Load<AudioData>("AudioData")); } }


    // ------ フォルダ移動等をトリガーに呼び出されるコールバック ------
    static void OnPostprocessAllAssets
        (string[] importedAssets, string[] deletedAssets,
         string[] movedAssets,    string[] movedFromPath)
    {
        if (audioData == null)
        {
            Debug.LogError("Resources フォルダに AudioData.asset を配置してください.");
            return;
        }

        foreach (var assetPath in importedAssets)
        {
            RegisterAudio(assetPath);
            RegisterGroup(assetPath);
        }
        foreach (var assetPath in deletedAssets)
        {
            RemoveAudio(assetPath);
            RemoveGroup(assetPath);
        }
        foreach (var assetPath in movedAssets)
        {
            RegisterAudio(assetPath);
            RegisterGroup(assetPath);
        }
        foreach (var assetPath in movedFromPath)
        {
            RemoveAudio(assetPath);
            RemoveGroup(assetPath);
        }
    }

    // --- 音声ファイルの登録処理 ---
    static void RegisterAudio(string audioPath)
    {
        AudioClip audio;
        string path = Path.GetDirectoryName(audioPath);
        if (path == audioData.bgmPath)
        {
            audio = AssetDatabase.LoadAssetAtPath(audioPath, typeof(AudioClip)) as AudioClip;
            if (audio == null) return;

            string replacedName = ReplaceString(audio.name);
            audioData.RegisterBGM(replacedName, audio);
            if (!audioData.bgmFilePathList.Contains(audioPath))
            {
                audioData.bgmFilePathList.Add(audioPath);
                CreateOrUpdateAudioClass(AudioType.BGM);
            }
        } else if (path == audioData.sePath) {

            audio = AssetDatabase.LoadAssetAtPath(audioPath, typeof(AudioClip)) as AudioClip;
            if (audio == null) return;

            string replacedName = ReplaceString(audio.name);
            audioData.RegisterSE(replacedName, audio);
            if (!audioData.seFilePathList.Contains(audioPath))
            {
                audioData.seFilePathList.Add(audioPath);
                CreateOrUpdateAudioClass(AudioType.SE);
            }
        }
    }

    // --- 音声ファイルの登録解除処理 ---
    static void RemoveAudio(string audioPath)
    {
        string path = Path.GetDirectoryName(audioPath);
        if (path == audioData.bgmPath)
        {
            audioData.RemoveBGM(audioPath);
            audioData.bgmFilePathList.Remove(audioPath);
            CreateOrUpdateAudioClass(AudioType.BGM);
        } else if (path == audioData.sePath) {
            audioData.RemoveSE(audioPath);
            audioData.seFilePathList.Remove(audioPath);
            CreateOrUpdateAudioClass(AudioType.SE);
        }
    }

    // --- 音声ファイルの登録数に応じて再生データの作成/更新を行う ---
    static void CreateOrUpdateAudioClass(AudioType audioType)
    {
        string type,audioFilePath,audioFileNameNoExtension;
        List<string> audioFilePathList;

        if (audioType == AudioType.BGM)
        {
            type                     = "BGM";
            audioFilePath            = BgmFilePath;
            audioFileNameNoExtension = BgmFilenameNoExtension;
            audioFilePathList        = audioData.bgmFilePathList;
        }
        else
        {
            type                     = "SE";
            audioFilePath            = SeFilePath;
            audioFileNameNoExtension = SeFilenameNoExtension;
            audioFilePathList        = audioData.seFilePathList;
        }

        var builder = new StringBuilder();

        builder.AppendLine("using UnityEngine;");
        builder.AppendLine();

        builder.AppendFormat("public static class {0}", audioFileNameNoExtension).AppendLine();
        builder.AppendLine("{");

        builder.AppendLine(@"    static AudioData _audioData;");
        builder.AppendLine(@"    static AudioData audioData { get { return _audioData = _audioData ?? (_audioData = Resources.Load<AudioData>(""AudioData"")); } }");
        builder.AppendLine();

        foreach (var path in audioFilePathList)
        {
            var audio = AssetDatabase.LoadAssetAtPath(path, typeof(AudioClip)) as AudioClip;
            if (audio == null) continue;
            var replacedAudioName = ReplaceString(audio.name);
            builder.AppendFormat(@"    public static AudioClip {0} {{ get {{ return audioData.Get{1}(""{2}""); }} }}", replacedAudioName, type, replacedAudioName).AppendLine();
        }
        
        builder.AppendLine("}");

        var directoryName = Path.GetDirectoryName(audioFilePath);

        if (!Directory.Exists(directoryName))
        {
            Directory.CreateDirectory(directoryName);
        }

        File.WriteAllText(audioFilePath, builder.ToString(), Encoding.UTF8);
        AssetDatabase.Refresh(ImportAssetOptions.Default);
    }

    // --- オーディオミキサーの登録処理 ---
    static void RegisterGroup(string groupPath)
    {
        AudioMixer mixer;
        string path = Path.GetDirectoryName(groupPath);
        if (path == audioData.mixerGroupPath)
        {
            mixer = AssetDatabase.LoadAssetAtPath(groupPath, typeof(AudioMixer)) as AudioMixer;
            if (mixer == null) return;

            groupFileList.Clear();
            CreateMixerGroup(mixer);
            
            CreateOrUpdateGroupClass();
        }
    }
    
    // --- オーディオミキサーの削除処理 ---
    static void RemoveGroup(string groupPath)
    {
        AudioMixer mixer;
        string path = Path.GetDirectoryName(groupPath);
        if (path == audioData.mixerGroupPath)
        {
            mixer = AssetDatabase.LoadAssetAtPath(groupPath, typeof(AudioMixer)) as AudioMixer;

            //指定のフォルダが空ならグループ全消去.
            if (mixer == null)
            {
                audioData.ClearGroup();
                groupFileList.Clear();
            }
            else
            {
                //中身がある場合も一旦消去して再構築.
                audioData.groupDictionary.Clear();
                CreateMixerGroup(mixer);
            }

            CreateOrUpdateGroupClass();
        }
    }

    // --- ミキサーからミキサーグループの抽出 ---
    static void CreateMixerGroup(AudioMixer mixer)
    {
        foreach (var group in mixer.FindMatchingGroups(""))
        {
            string replacedName = ReplaceString(group.name);
            audioData.RegisterGroup(replacedName, group);
            if (!groupFileList.Contains(group))
                groupFileList.Add(group);
        }
    }

    // --- ミキサーグループ使用準備 ---
    static void CreateOrUpdateGroupClass()
    {
        var builder = new StringBuilder();

        builder.AppendLine("using UnityEngine;");
        builder.AppendLine("using UnityEngine.Audio;");
        builder.AppendLine();

        builder.AppendLine("public static class MixerGroup");
        builder.AppendLine("{");

        builder.AppendLine(@"    static AudioData _audioData;");
        builder.AppendLine(@"    static AudioData audioData { get { return _audioData = _audioData ?? (_audioData = Resources.Load<AudioData>(""AudioData"")); } }");
        builder.AppendLine();

        foreach(var groupFile in groupFileList) {
            var replacedGroupName = ReplaceString(groupFile.name);
            builder.AppendFormat(@"    public static AudioMixerGroup {0} {{ get {{ return audioData.GetGroup(""{1}""); }} }}", replacedGroupName, replacedGroupName).AppendLine();
        }

        builder.AppendLine("}");

        var directoryName = Path.GetDirectoryName(audioData.mixerGroupPath);

        if (!Directory.Exists(directoryName))
        {
            Directory.CreateDirectory(directoryName);
        }

        File.WriteAllText(MixerGroupFilePath, builder.ToString(), Encoding.UTF8);
        AssetDatabase.Refresh(ImportAssetOptions.Default);
    }

    // --- 文字列の置換処理.パフォーマンスェ… ---
    static string ReplaceString(string origin)
    {
        //先頭末尾のスペースを削除.
        origin = origin.Trim();

        int output;
        string pre = int.TryParse(origin.Substring(0,1), out output) ? "_" : "";
        
        return pre + origin.Replace(" ", "_").Replace("(", "_").Replace(")", "_").Replace("-", "_").Replace("__", "_");
    }
}
