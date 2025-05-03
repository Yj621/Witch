using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public int MasterSoundLevel;
    public int BgmLevel;
    public int SfxLevel;

    public AudioMixer audioMixer;

    public AudioSource bgmSource;
    public AudioSource sfxSource;

    [SerializeField]
    private List<AudioClip> bgmClips;

    private Dictionary<string, AudioClip> bgmDict = new();

    [SerializeField]
    private List<AudioClip> sfxClips;

    private Dictionary<string, AudioClip> sfxDict = new();


    public static SoundManager Instance { get; private set; }

    void Start()
    {
        foreach (var clip in sfxClips)
        {
            sfxDict[clip.name] = clip;
        }

        foreach (var clip in bgmClips)
        {
            bgmDict[clip.name] = clip;
        }
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    public void SetMasterVolume(float value)
    {
        audioMixer.SetFloat("Master", Mathf.Log10(value) * 20);
    }

    public void SetBGMVolume(float value)
    {
        audioMixer.SetFloat("BGM", Mathf.Log10(value) * 20);
    }

    public void SetSFXVolume(float value)
    {
        audioMixer.SetFloat("SFX", Mathf.Log10(value) * 20);
    }

    public void PlaySFX(string name)
    {
        if (sfxDict.TryGetValue(name, out var clip))
        {
            sfxSource.PlayOneShot(clip);
        }
    }

        public void PlayBGM(string name, bool loop = true)
    {
        if (bgmDict.TryGetValue(name, out var clip))
        {
            bgmSource.clip = clip;
            bgmSource.loop = loop;
            bgmSource.Play();
        }
    }

    public void ChangeVol(int index, int vol)
    {
        switch (index)
        {
            case 0:
                MasterSoundLevel += vol;
                MasterSoundLevel = Mathf.Clamp(MasterSoundLevel, 0, 4);
                audioMixer.SetFloat("Master", Mathf.Lerp(-80, 0, MasterSoundLevel / 4f));
                break;
            case 1:
                BgmLevel += vol;
                BgmLevel = Mathf.Clamp(MasterSoundLevel, 0, 4);
                audioMixer.SetFloat("Bgm", Mathf.Lerp(-80, 0, BgmLevel/ 4f));
                break;
            case 2:
                SfxLevel += vol;
                SfxLevel = Mathf.Clamp(SfxLevel, 0, 4);
                audioMixer.SetFloat("Sfx", Mathf.Lerp(-80, 0, SfxLevel / 4f));
                break;
        }
    }
}
