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



    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        PlayMusicBg();
    }

    public void PlayMusic(string MusicName)
    {
        AudioSource.PlayClipAtPoint(List_Music.Find(it => it.MusicName == MusicName).audioClip, Camera.main.transform.position, BackGroundMusicVolume * 2);
    }

    void PlayMusicBg()
    {
        audioSource.clip = bgm;
        audioSource.Play();
        audioSource.loop = true;
        BackGroundMusicVolume = audioSource.volume;
    }

    public void StopPlayBgm()
    {
        audioSource.Stop();
    }
}
