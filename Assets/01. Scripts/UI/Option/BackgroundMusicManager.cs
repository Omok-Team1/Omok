using UnityEngine;
using System.Collections.Generic;

public class BackgroundMusicManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] musicClips; // 인스펙터에서 음악 파일들을 할당할 배열
    [SerializeField] private int defaultMusicIndex = 0; // 시작할 때 재생할 음악 인덱스
    
    private AudioSource audioSource;
    private int currentMusicIndex = 0;
    private float maxVolume = 1.0f;
    private float currentVolumeLevel = 0.6f; // 기본값 3/5 = 0.6
    
    private static BackgroundMusicManager instance;
    
    public static BackgroundMusicManager Instance
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
        
        // AudioSource 컴포넌트 추가
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true; // 반복 재생
        audioSource.playOnAwake = false; // 시작 시 자동 재생하지 않음
    }
    
    private void Start()
    {
        // 기본 음악 재생
        currentMusicIndex = defaultMusicIndex;
        if (musicClips != null && musicClips.Length > 0)
        {
            PlayMusic(currentMusicIndex);
        }
        
        // 기본 볼륨 설정 (3/5 = 0.6)
        SetVolumeLevel(3, 5);
    }
    
    // 특정 인덱스의 음악 재생
    public void PlayMusic(int index)
    {
        if (musicClips == null || index < 0 || index >= musicClips.Length)
            return;
            
        audioSource.clip = musicClips[index];
        audioSource.Play();
        currentMusicIndex = index;
    }
    
    // 다음 음악 재생
    public void PlayNextMusic()
    {
        int nextIndex = (currentMusicIndex + 1) % musicClips.Length;
        PlayMusic(nextIndex);
    }
    
    // 특정 씬에서 특정 음악 재생
    public void PlayMusicForScene(string sceneName, int musicIndex)
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == sceneName)
        {
            PlayMusic(musicIndex);
        }
    }
    
    // 볼륨 레벨 설정 (0~maxStep)
    public void SetVolumeLevel(int level, int maxStep)
    {
        currentVolumeLevel = (float)level / maxStep;
        audioSource.volume = currentVolumeLevel;
    }
    
    // 볼륨 가져오기
    public float GetVolume()
    {
        return audioSource.volume;
    }
    
    // 음악 일시 정지
    public void PauseMusic()
    {
        if (audioSource.isPlaying)
            audioSource.Pause();
    }
    
    // 음악 재개
    public void ResumeMusic()
    {
        if (!audioSource.isPlaying)
            audioSource.UnPause();
    }
    
    // 음악 정지
    public void StopMusic()
    {
        audioSource.Stop();
    }
}