using System.Collections.Generic;
using Unity.VisualScripting;
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
        audioMixer.SetFloat("Bgm", Mathf.Log10(value) * 20);
    }

    public void SetSFXVolume(float value)
    {
        audioMixer.SetFloat("Sfx", Mathf.Log10(value) * 20);
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
        int[] dBLevels = { -80, -60, -40, -20, 0 }; // 5단계 볼륨

        switch (index)
        {
            case 0:
                MasterSoundLevel += vol;
                MasterSoundLevel = Mathf.Clamp(MasterSoundLevel, 0, 4);
                audioMixer.SetFloat("Master", dBLevels[MasterSoundLevel]);
                break;
            case 1:
                BgmLevel += vol;
                BgmLevel = Mathf.Clamp(BgmLevel, 0, 4);
                audioMixer.SetFloat("Bgm", dBLevels[BgmLevel]);
                break;
            case 2:
                SfxLevel += vol;
                SfxLevel = Mathf.Clamp(SfxLevel, 0, 4);
                audioMixer.SetFloat("Sfx", dBLevels[SfxLevel]);
                break;
        }
    }


    private void Update()
    {
        Debug.Log($"clip: {bgmSource.clip?.name}");
        Debug.Log($"isPlaying: {bgmSource.isPlaying}");
        Debug.Log($"mute: {bgmSource.mute}");
        Debug.Log($"volume: {bgmSource.volume}");
    }
}
