using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public int MasterSoundLevel = 2;
    public int BgmLevel = 2;
    public int SfxLevel = 2;

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
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
     }
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

    public void ChangeVol(int index, int stepDelta) // stepDelta는 그냥 +1, -1
    {
        switch (index)
        {
            case 0:
                MasterSoundLevel += stepDelta;
                MasterSoundLevel = Mathf.Clamp(MasterSoundLevel, 0, 4);
                if (MasterSoundLevel == 0)
                    audioMixer.SetFloat("Master", -80);
                else
                audioMixer.SetFloat("Master", Mathf.Lerp(-40f, 10f, MasterSoundLevel - 1 / 3f));
                break;
            case 1:
                BgmLevel += stepDelta;
                BgmLevel = Mathf.Clamp(BgmLevel, 0, 4);
                if (MasterSoundLevel == 0)
                    audioMixer.SetFloat("Bgm", -80);
                else
                audioMixer.SetFloat("Bgm", Mathf.Lerp(-40f, 10f, BgmLevel - 1 / 3f));
                break;
            case 2:
                SfxLevel += stepDelta;
                SfxLevel = Mathf.Clamp(SfxLevel, 0, 4);
                if (MasterSoundLevel == 0)
                    audioMixer.SetFloat("Sfx", -80);
                else
                audioMixer.SetFloat("Sfx", Mathf.Lerp(-40f, 10f, SfxLevel - 1 / 3f));
                break;
        }
        Debug.Log($"[Master:{MasterSoundLevel}] → {Mathf.Lerp(-80f, 0f, MasterSoundLevel / 4f)} dB");
    }

}
