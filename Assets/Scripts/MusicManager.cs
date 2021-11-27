using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public AudioClip mainTheme;
    public AudioClip menuTheme;
    
    string sceneName;

    void Start() {
        sceneName = SceneManager.GetActiveScene().name;
        PlayMusic();
        SceneManager.sceneLoaded += OnLevelLoaded;
    }

    void OnLevelLoaded(Scene loaded, LoadSceneMode mode){
        string newSceneName = SceneManager.GetActiveScene().name;

        if(newSceneName != sceneName){
            sceneName = newSceneName;
            Invoke("PlayMusic", .2f);
        }
    }

    void PlayMusic(){
        AudioClip track = null;

        if(sceneName == "Menu"){
            track = menuTheme;
        } else if (sceneName == "Game" || sceneName == "Endless") {
            track = mainTheme;
        }

        if(track != null){
            AudioManager.instance.PlayMusic(track, 2);
            Invoke("PlayMusic", track.length);
        }
    }
}
