using UnityEngine;
using System.Collections.Generic;

public class EffectSoundManager : MonoBehaviour
{
    [System.Serializable]
    public class SoundEffect
    {
        public string name;
        public AudioClip clip;
    }
    
    [SerializeField] private SoundEffect[] soundEffects;  // 인스펙터에서 이펙트 사운드 설정
    
    private Dictionary<string, AudioClip> effectDictionary = new Dictionary<string, AudioClip>();
    private AudioSource effectAudioSource;
    private float volumeLevel = 0.6f; // 기본값 3/5 = 0.6
    
    private static EffectSoundManager instance;
    
    public static EffectSoundManager Instance
    {
        get { return instance; }
    }
    
    private void Awake()
    {
        // 싱글톤 패턴 구현
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        instance = this;
        DontDestroyOnLoad(gameObject); // 씬 전환시에도 유지
        
        // 효과음 사전 초기화
        foreach (SoundEffect effect in soundEffects)
        {
            if (effect.clip != null)
                effectDictionary.Add(effect.name, effect.clip);
        }
        
        // 기본 AudioSource 컴포넌트 추가
        effectAudioSource = gameObject.AddComponent<AudioSource>();
        effectAudioSource.playOnAwake = false;
    }
    
    private void Start()
    {
        // 기본 볼륨 설정 (3/5 = 0.6)
        SetVolumeLevel(3, 5);
    }
    
    // 효과음 재생
    public void PlaySound(string soundName)
    {
        if (effectDictionary.ContainsKey(soundName))
        {
            AudioClip clip = effectDictionary[soundName];
            effectAudioSource.PlayOneShot(clip, volumeLevel);
        }
        else
        {
            Debug.LogWarning("효과음 '" + soundName + "'을(를) 찾을 수 없습니다.");
        }
    }
    
    // 볼륨 레벨 설정 (0~maxStep)
    public void SetVolumeLevel(int level, int maxStep)
    {
        volumeLevel = (float)level / maxStep;
    }
    
    // 현재 볼륨 가져오기
    public float GetVolume()
    {
        return volumeLevel;
    }
    
    // 런타임에 효과음 추가
    public void AddSound(string name, AudioClip clip)
    {
        if (!effectDictionary.ContainsKey(name) && clip != null)
        {
            effectDictionary.Add(name, clip);
        }
    }
}