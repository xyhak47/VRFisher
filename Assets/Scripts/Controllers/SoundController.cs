using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundController : MonoBehaviour
{
    [System.Serializable]
    public class Music
    {
        public AudioClip audioClip;
        public string MusicName;
    }

    public static SoundController Instance;
    SoundController()
    {
        Instance = this;
    }

    public List<Music> List_Music;
    private AudioSource audioSource;
    private float BackGroundMusicVolume;


    public AudioClip bgm;

    public Transform musicPos;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        PlayMusicBg();
    }

    public void PlayMusic(string MusicName)
    {
        AudioSource.PlayClipAtPoint(List_Music.Find(it => it.MusicName == MusicName).audioClip, musicPos.position, 3);
    }

    void PlayMusicBg()
    {
        audioSource.clip = bgm;
        audioSource.Play();
        audioSource.loop = true;
        audioSource.volume = 0.3f;
    }

    public void StopPlayBgm()
    {
        audioSource.Stop();
    }
}
