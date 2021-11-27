using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{

    public GameObject mainMenu;
    public GameObject optionsMenu;

    public Slider[] volumeSliders;

    void Start() {
        volumeSliders[0].value = AudioManager.instance.masterVolumePercent;
        volumeSliders[1].value = AudioManager.instance.musicVolumePercent;
        volumeSliders[2].value = AudioManager.instance.effectsVolumePercent;
    }

    public void Play() {
        SceneManager.LoadScene("Game");
    }

    public void Endless() {
        WaveGenerator.SetSeed(Random.Range(0,int.MaxValue));
        SceneManager.LoadScene("Endless");
    }

    public void Quit() {
        Application.Quit();
    }

    public void OptionsMenu() {
        mainMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    public void MainMenu() {
        mainMenu.SetActive(true);
        optionsMenu.SetActive(false);
    }

    public void SetMasterVolume(float value) {
        AudioManager.instance.SetVolume(value, AudioManager.Channel.Master);
    }

    public void SetMusicVolume(float value) {
        AudioManager.instance.SetVolume(value, AudioManager.Channel.Music);
    }

    public void SetEffectsVolume(float value) {
        AudioManager.instance.SetVolume(value, AudioManager.Channel.Effect);
    }
}
