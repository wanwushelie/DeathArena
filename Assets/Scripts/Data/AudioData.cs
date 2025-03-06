using UnityEngine;

[CreateAssetMenu(fileName = "AudioData", menuName = "Audio System/Audio Data")]
public class AudioData : ScriptableObject
{
    [System.Serializable]
    public class AudioEntry
    {
        public string key; // 音频标识（如：PlayerWalk）
        public SoundType type; // 音效类型（BGM/EFFECT）
        public AudioClip[] candidateClips; // 候选音频剪辑
        public AudioClip selectedClip; // 最终选定的音频剪辑
        [Range(0, 1)] public float volume = 1f; // 音量
        [Range(0.1f, 3)] public float pitch = 1f; // 音调
    }

    public AudioEntry[] entries; // 所有音频条目
}