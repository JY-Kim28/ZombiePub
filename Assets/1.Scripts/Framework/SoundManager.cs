using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class SoundManager : Singleton<SoundManager>
{
    //static SoundManager _Instance;
    //public static SoundManager Instance { get { return _Instance ??= new SoundManager(); } }

    public float EffectVolume;
    public float BGMVolume;

    int _EffectPlayIdx;
    AudioSource[] _EffectSources;
    AudioSource _BGMSource;

    Dictionary<string, AudioClip> _SoundDic;
    Dictionary<string, int> _PlayCountDic;

    private void Awake()
    {
        if(GetComponent<AudioListener>() == null)
        {
            gameObject.AddComponent<AudioListener>();
        }
    }

    public void Create()
    {
        if (_SoundDic != null) return;

        _SoundDic = new Dictionary<string, AudioClip>();
        _PlayCountDic = new Dictionary<string, int>();

        _EffectPlayIdx = 0;
        _EffectSources = new AudioSource[5];
        for(int i = 0; i < 5; i++)
        {
            _EffectSources[i] = gameObject.AddComponent<AudioSource>();
            _EffectSources[i].loop = false;
        }

        _BGMSource = gameObject.AddComponent<AudioSource>();
        _BGMSource.loop = true;

        LoadSoundAssets();
    }

    private void LoadSoundAssets()
    {
        var handler = Addressables.LoadAssetsAsync<AudioClip>("SoundSFX", obj =>
        {
            _SoundDic.Add(obj.name, obj);
            _PlayCountDic.Add(obj.name, 0);
        }).WaitForCompletion();
        Addressables.Release(handler);
    }

    public void PlayEffect(string soundName)
    {
        PlayEffect(_SoundDic[soundName]);
    }

    public void PlayEffect(AudioClip clip)
    {
        if (clip == null)
            return;

        //if (Root.UserInfo.IsSFXOn() == false)
        //    return;

        _EffectSources[_EffectPlayIdx].PlayOneShot(clip);

        ++_EffectPlayIdx;
        if (_EffectPlayIdx == 5)
            _EffectPlayIdx = 0;
    }

    public void PlayBGM(string soundName)
    {
        //if (Root.UserInfo.IsBGMOn() == false)
        //    return;
        if(_BGMSource.name == soundName) return;

        var handler = Addressables.LoadAssetAsync<AudioClip>($"Sounds/BGM/{soundName}.wav").WaitForCompletion();
        if (handler == null)
            return; 

        _BGMSource.Stop();

        _BGMSource.clip = handler;
        _BGMSource.Play();
    
        //SetBGMVolume(Root.UserInfo.IsBGMOn());
    }

    public void SetBGMVolume(bool isOn)
    {
        _BGMSource.volume = isOn ? 1 : 0;
    }

    public void SetSFXVolume(bool isOn)
    {
        float volume = isOn ? 1 : 0;

        foreach(var e in _EffectSources)
        {
            e.volume = volume;
        }
    }
}
