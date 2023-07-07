using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public enum Sound { Master, BGM, Effect, Size}

    AudioSource[] audioSources = new AudioSource[(int)Sound.Size];
    Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();

    AudioMixer audioMixer;
    AudioMixerGroup[] audioMixerGroup;

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        audioMixer = GameManager.Resource.Load<AudioMixer>("Sound/MyMixer");
        audioMixerGroup = audioMixer.FindMatchingGroups("Master");

        GameObject root = GameObject.Find("@Sound");
        if (root == null)
        {
            root = new GameObject { name = "@Sound" };
            Object.DontDestroyOnLoad(root);

            string[] soundName = System.Enum.GetNames(typeof(Sound)); // BGM, Effect
            for (int i = 1; i < soundName.Length - 1; i++)
            {
                GameObject go = new GameObject { name = soundName[i] };
                audioSources[i] = go.AddComponent<AudioSource>();
                go.transform.SetParent(root.transform);
                go.GetComponent<AudioSource>().outputAudioMixerGroup = audioMixerGroup[i];
            }

            audioSources[(int)Sound.BGM].loop = true;
        }
    }

    public void Clear()
    {
        foreach (AudioSource audioSource in audioSources)
        {
            audioSource.clip = null;
            audioSource.Stop();
        }
        audioClips.Clear();
    }

    public void Play(AudioClip audioClip, Sound type = Sound.Effect, float pitch = 1.0f)
    {
        if (audioClip == null)
            return;

        if (type == Sound.BGM) // BGM 배경음악 재생
        {
            AudioSource audioSource = audioSources[(int)Sound.BGM];
            if (audioSource.isPlaying)
                audioSource.Stop();

            audioSource.pitch = pitch;
            audioSource.clip = audioClip;
            audioSource.Play();
        }
        else // Effect 효과음 재생
        {
            AudioSource audioSource = audioSources[(int)Sound.Effect];
            audioSource.pitch = pitch;
            audioSource.PlayOneShot(audioClip);
        }
    }

    public void Play(string path, Sound type = Sound.Effect, float pitch = 1.0f)
    {
        AudioClip audioClip = GetOrAddAudioClip(path, type);
        Play(audioClip, type, pitch);
    }

    AudioClip GetOrAddAudioClip(string path, Sound type = Sound.Effect)
    {
        if (path.Contains("Sound/") == false)
            path = $"Sound/{path}"; // Sound 폴더 안에 저장될 수 있도록

        AudioClip audioClip = null;

        if (type == Sound.BGM) // BGM 배경음악 클립 붙이기
        {
            audioClip = GameManager.Resource.Load<AudioClip>(path);
        }
        else // Effect 효과음 클립 붙이기
        {
            if (audioClips.TryGetValue(path, out audioClip) == false)
            {
                audioClip = GameManager.Resource.Load<AudioClip>(path);
                audioClips.Add(path, audioClip);
            }
        }

        if (audioClip == null)
            Debug.Log($"AudioClip Missing ! {path}");

        return audioClip;
    }

    public void Stop(Sound type)
    {
        audioSources[(int)type].Stop();
    }

    public void SetAudioVolume(string type, float value)
    {
        audioMixer.SetFloat(type, value);
    }

    public float GetAudioVolume(string type)
    {
        float value;
        audioMixer.GetFloat(type, out value);
        return value;
    }
}


