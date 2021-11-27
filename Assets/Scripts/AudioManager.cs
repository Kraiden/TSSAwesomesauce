using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public static AudioManager instance;

    public enum Channel {Master, Effect, Music}

    public float masterVolumePercent { get; private set; }
    public float effectsVolumePercent { get; private set; }
    public float musicVolumePercent { get; private set; }

    AudioSource[] musicSources;
    int activeMusicSourceIndex;

    AudioSource sound2dSource;

    Transform audioListener;
    Transform player;

    SoundLibrary library;

    void Awake(){
        if(instance != null){
            Destroy(gameObject);
            if(FindObjectOfType<Player>() != null){
                instance.player = FindObjectOfType<Player>().transform;
            }
        } else {

            instance = this;
            DontDestroyOnLoad(gameObject);

            library = GetComponent<SoundLibrary>();

            audioListener = FindObjectOfType<AudioListener>().transform; 

            masterVolumePercent = PlayerPrefs.GetFloat("vol-master", .5f);
            effectsVolumePercent = PlayerPrefs.GetFloat("vol-effects", 1);
            musicVolumePercent = PlayerPrefs.GetFloat("vol-music", 1);

            musicSources = new AudioSource[2];
            for(int i = 0; i < 2; i++){
                GameObject newMusicSource = new GameObject("Music source " + (i + 1));
                musicSources[i] = newMusicSource.AddComponent<AudioSource> ();

                newMusicSource.transform.parent = transform;
            }

            GameObject sound2d = new GameObject("2D source");
            sound2dSource = sound2d.AddComponent<AudioSource> ();

            sound2d.transform.parent = transform;
        }
    }

    void Update() {
        if(player != null){
            audioListener.position = player.position;
        }
    }

    public void SetVolume(float volumePercent, Channel channel){
        switch(channel){
            case Channel.Master: 
                masterVolumePercent = volumePercent;
                break;
            case Channel.Effect:
                effectsVolumePercent = volumePercent;
                break;
            case Channel.Music:
                musicVolumePercent = volumePercent;
                break;
        }

        float musicVolume = musicVolumePercent * masterVolumePercent;
        musicSources[0].volume = musicVolume;
        musicSources[1].volume = musicVolume;

        PlayerPrefs.SetFloat("vol-master", masterVolumePercent);
        PlayerPrefs.SetFloat("vol-effects", effectsVolumePercent);
        PlayerPrefs.SetFloat("vol-music", musicVolumePercent);

        PlayerPrefs.Save();
    }

    public void PlayMusic(AudioClip clip, float fadeDuration = 1){
        activeMusicSourceIndex = 1 - activeMusicSourceIndex;
        AudioSource src = musicSources[activeMusicSourceIndex];

        src.clip = clip;
        src.Play();

        StartCoroutine(MusicCrossFade(fadeDuration));
    }

    IEnumerator MusicCrossFade(float duration){
        float pct = 0;

        while(pct < 1){
            pct += Time.deltaTime * 1 / duration;
            musicSources[activeMusicSourceIndex].volume = Mathf.Lerp( 0, musicVolumePercent * masterVolumePercent, pct);
            musicSources[1 - activeMusicSourceIndex].volume = Mathf.Lerp( musicVolumePercent * masterVolumePercent, 0, pct);

            yield return null;
        }

    }

    public void PlaySound(AudioClip clip, Vector3 position){
        if(clip != null){
            AudioSource.PlayClipAtPoint(clip, position, effectsVolumePercent * masterVolumePercent);
        }
    }

    public void PlaySound(string name, Vector3 position){
        PlaySound(library.GetClipByName(name), position);
    }

    public void PlaySound2d(string name){
        sound2dSource.PlayOneShot(library.GetClipByName(name), effectsVolumePercent * masterVolumePercent);
    }
    
}
