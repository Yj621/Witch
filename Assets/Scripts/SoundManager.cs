using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    // 0부터 3까지, 총 4단계
    private const int MaxVolumeStep = 4;

    // 원하는 dB 범위
    private const float MinVolumeDB = -22f;
    private const float MaxVolumeDB = 5f;
    public int MasterSoundLevel = 2;
    public int BgmLevel = 2;
    public int SfxLevel = 2;

    public AudioMixer audioMixer;
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    [SerializeField] private List<AudioClip> bgmClips;
    private Dictionary<string, AudioClip> bgmDict = new();

    [SerializeField] private List<AudioClip> sfxClips;
    private Dictionary<string, AudioClip> sfxDict = new();

    public static SoundManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        foreach (var clip in bgmClips) bgmDict[clip.name] = clip;
        foreach (var clip in sfxClips) sfxDict[clip.name] = clip;
    }

    public void PlaySFX(string name)
    {
        if (sfxDict.TryGetValue(name, out var clip))
            sfxSource.PlayOneShot(clip);
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
 // -22dB(0단) ~ +5dB(3단) 사이를 균등 분할
    private void ApplyVolume(string param, int step)
    {
        // step이 0~MaxVolumeStep 사이에 있도록 보장
        float t = Mathf.Clamp01(step / (float)MaxVolumeStep);
        float dB = Mathf.Lerp(MinVolumeDB, MaxVolumeDB, t);
        audioMixer.SetFloat(param, dB);
    }

    public void ChangeVol(int index, int stepDelta)
    {
        switch (index)
        {
            case 0:
                MasterSoundLevel = Mathf.Clamp(MasterSoundLevel + stepDelta, 0, MaxVolumeStep);
                ApplyVolume("Master", MasterSoundLevel);
                break;
            case 1:
                BgmLevel = Mathf.Clamp(BgmLevel + stepDelta, 0, MaxVolumeStep);
                ApplyVolume("Bgm", BgmLevel);
                break;
            case 2:
                SfxLevel = Mathf.Clamp(SfxLevel + stepDelta, 0, MaxVolumeStep);
                ApplyVolume("Sfx", SfxLevel);
                break;
        }
    }
}