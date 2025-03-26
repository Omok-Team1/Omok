using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSoundManager : MonoBehaviour, IOnEvent
{
    void Awake()
    {
        Init();
    }
    
    public void OnEvent(EventMessage msg)
    {
        PlayEffectSound(msg.EventName);
    }

    private void PlayEffectSound(string eventName)
    {
        if (_eventSounds.TryGetValue(eventName, out var effectSound))
        {
            _audioSource.PlayOneShot(effectSound);
        }
        else
        {
            throw new Exception("Sound not found");
        }
    }

    private void Init()
    {
        Dictionarylization();

        TryGetComponent(out _audioSource);
        
        foreach (string se in eventName)
        {
            EventManager.Instance.AddListener(se, this);
        }
    }
    
    private void Dictionarylization()
    {
        if (eventName.Count != effectSounds.Count)
        {
            throw new Exception("이벤트 이름과 이벤트 사운드의 개수가 맞지 않습니다.");
        }
        else
        {
            for (int idx = 0; idx < effectSounds.Count; idx++)
            {
                _eventSounds.Add(eventName[idx], effectSounds[idx]);
            }
        }
    }

    private AudioSource _audioSource;
    private Coroutine _coroutine = null;

    private readonly Dictionary<string, AudioClip> _eventSounds = new();
    [Header("이펙트 사운드가 재생되어야 하는 이벤트 이름들입니다.")]
    [SerializeField] private List<string> eventName;
    
    [Header("이벤트 이름들에 대응되는 사운드입니다. 반드시 eventName의 인덱스에 맞게 설정해주세요")]
    [SerializeField] private List<AudioClip> effectSounds;
}
