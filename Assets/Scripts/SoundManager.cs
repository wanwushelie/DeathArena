using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundType
{
    BGM,
    EFFECT,
    MAXCOUNT,
}

public class SoundManager
{
    public static SoundManager Instance { get; private set; }

    public AudioSource[] audioSources = new AudioSource[(int)SoundType.MAXCOUNT];
    private Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();
    private AudioData audioData;

    private SoundManager() { }

    static SoundManager()
    {
        Instance = new SoundManager();
    }

    public void Init(AudioData data = null)
    {
        audioData = data;
        
        GameObject root = GameObject.Find("@Sound");
        if (root == null)
        {
            root = new GameObject { name = "@Sound" };
            Object.DontDestroyOnLoad(root);

            string[] soundNames = System.Enum.GetNames(typeof(SoundType));
            for (int i = 0; i < soundNames.Length - 1; i++)
            {
                GameObject go = new GameObject { name = soundNames[i] };
                audioSources[i] = go.AddComponent<AudioSource>();
                go.transform.parent = root.transform;
            }

            audioSources[(int)SoundType.BGM].loop = true;
            audioSources[(int)SoundType.BGM].volume = 1f;
            audioSources[(int)SoundType.EFFECT].volume = 1f;
        }
    }

    public void Play(string key)
    {
        if (audioData != null)
        {
            var entry = System.Array.Find(audioData.entries, e => e.key == key);
            if (entry != null)
            {
                Play(entry.selectedClip, entry.type, entry.pitch, entry.volume);
                return;
            }
        }
        Debug.LogWarning($"[SoundManager] AudioData not found or key '{key}' not exist");
    }

    // ... 保留原有 GetOrAddAudioClip 方法 ...

    public void Clear()
    {
        foreach (AudioSource audioSource in audioSources)
        {
            audioSource.clip = null;
            audioSource.Stop();
        }
        audioClips.Clear();
    }

    public void Play(AudioClip audioClip, SoundType type = SoundType.EFFECT, float pitch = 1.0f, float volume = 1.0f)
    {
        if (audioClip == null)
            return;

        if (type == SoundType.BGM)
        {
            AudioSource audioSource = audioSources[(int)type];
            if (audioSource.isPlaying)
                audioSource.Stop();

            audioSource.pitch = pitch;
            audioSource.clip = audioClip;
            audioSource.volume = volume;
            audioSource.Play();
        }
        else
        {
            AudioSource audioSource = audioSources[(int)type];
            audioSource.pitch = pitch;
            audioSource.PlayOneShot(audioClip);
        }
    }

    public void Play(string path, SoundType type = SoundType.EFFECT, float pitch = 1.0f, float volume = 1.0f)
    {
        AudioClip audioClip = GetOrAddAudioClip(path, type);
        Play(audioClip, type, pitch, volume);
    }

    private AudioClip GetOrAddAudioClip(string path, SoundType type = SoundType.EFFECT)
    {
        if (path.Contains("Sounds/") == false)
            path = $"Sounds/{path}";

        AudioClip audioClip = null;
        if (audioClips.TryGetValue(path, out audioClip) == false)
        {
            audioClip = Resources.Load<AudioClip>(path);
            if (audioClip != null)
            {
                audioClips.Add(path, audioClip);
            }
            else
            {
                Debug.LogError($"[SoundManager] AudioClip not found at path: {path}");
            }
        }
        return audioClip;
    }


    public void Stop(AudioClip audioClip, SoundType type = SoundType.EFFECT)
    {
        if (audioClip == null)
            return;

        if (type == SoundType.BGM) // BGM 배경음악 정지
        {
            AudioSource audioSource = audioSources[(int)SoundType.BGM];
            if (audioSource.clip == audioClip)
            {
                audioSource.Stop();
                audioSource.clip = null;
            }
        }
        else // Effect 효과음 정지
        {
            AudioSource audioSource = audioSources[(int)SoundType.EFFECT];
            if (audioSource.clip == audioClip)
            {
                audioSource.Stop();
                audioSource.clip = null;
            }
        }
    }

    public void Stop(string path, SoundType type = SoundType.EFFECT)
    {
        AudioClip audioClip = GetOrAddAudioClip(path, type);
        Stop(audioClip, type);
    }

    public void StopAll()
    {
        foreach (AudioSource audioSource in audioSources)
        {
            audioSource.Stop();
            audioSource.clip = null;
        }
    }

    public void FadeOut(float duration, string path)
    {
        AudioClip audioClip = GetOrAddAudioClip(path, SoundType.BGM);

        AudioSource audioSource = audioSources[(int)SoundType.BGM];
        {
            if (audioSource != null)
            {
                audioSource.clip = audioClip;

                if (!audioSource.isPlaying)
                {
                    audioSource.Play();
                }
                //볼륨을 서서히 줄이기 위한 코루틴 호출
                CoroutineHandler.StartStaticCoroutine(FadeOutCoroutine(audioSource, duration));
            }
        }
    }

    private IEnumerator FadeOutCoroutine(AudioSource audioSource, float duration)
    {
        float startVolume = audioSource.volume;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, elapsedTime / duration);
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
    }

    public void FadeIn(float duration, string path)
    {
        AudioClip audioClip = GetOrAddAudioClip(path, SoundType.BGM);

        AudioSource audioSource = audioSources[(int)SoundType.BGM];

        if (audioSource != null)
        {
            audioSource.clip = audioClip;
            audioSource.volume = 0f;

            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
            //볼륨을 서서히 키우기 위한 코루틴 호출
            CoroutineHandler.StartStaticCoroutine(FadeInCoroutine(audioSource, duration));
        }
    }

    private IEnumerator FadeInCoroutine(AudioSource audioSource, float duration)
    {
        float startVolume = audioSource.volume;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 1f, elapsedTime / duration);
            yield return null;
        }
        audioSource.volume = 1;
    }

    public void OnVolumeChanged(float value, SoundType type)
    {
        if (type == SoundType.BGM)
        {
            AudioSource audioSource = audioSources[(int)type];
            audioSource.volume = value;
        }
        else
        {
            AudioSource audioSource = audioSources[(int)type];
            audioSource.volume = value;
        }
    }

}