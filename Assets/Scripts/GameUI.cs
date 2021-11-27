using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public Image fadePlane;
    public GameObject gameOverUI;

    public CanvasGroup newWaveBanner;
    public Text newWaveTitle;
    public Text newWaveSubtitle;
    public Text scoreUI;
    public Text scoreUIEnd;
    public GameObject gameUI;
    public RectTransform healthBar;
    public GameObject[] gunIndicators;

    Spawner spawner;
    Player player;

    void Start(){
        player = FindObjectOfType<Player> ();
        player.OnDeath += OnGameOver;

        GunController.OnEquipNew += HandleGunEquip;
    }

    void Awake(){
        spawner = FindObjectOfType<Spawner>();
        spawner.OnNewWave += OnNewWave;
    }

    void Update(){
        scoreUI.text = ScoreKeeper.score.ToString("D6");

        float healthPct = 0;
        if(player != null){
            healthPct = player.health / player.startingHealth;
        }
        healthBar.localScale = new Vector3(healthPct, 1, 1);

    }

    void OnNewWave(int waveNumber){
        newWaveSubtitle.text = spawner.waves[waveNumber - 1].enemyCount + " opponents taking " + spawner.waves [waveNumber -1].enemyHealth + " hits";
        if(waveNumber == 1){
            newWaveTitle.text = "- GOOD LUCK -";
        } else if(spawner.IsEnd){
            newWaveTitle.text = "- YOU DID IT -";
            newWaveSubtitle.text ="now survive this";
        } else {
            newWaveTitle.text = "- NEXT WAVE -";
        }
        
        StartCoroutine(AnimateNewWaveBanner());
    }

    void OnGameOver() {
        GunController.OnEquipNew -= HandleGunEquip;
        gameOverUI.SetActive(true);
        gameUI.SetActive(false);
        scoreUIEnd.text = ScoreKeeper.score.ToString("D6");
        StartCoroutine(Fade (Color.clear,new Color(0f,0f,0f,0.85f), 3f));
    }

    IEnumerator Fade(Color from, Color to, float time){
        float speed = 1 / time;
        float percent = 0;

        while (percent < 1) {
            percent += Time.deltaTime * speed;
            fadePlane.color = Color.Lerp(from, to, percent);

            yield return null;
        }
    }

    IEnumerator AnimateNewWaveBanner(){
        float delayTime = 1.5f;
        float speed = 2.5f;
        float pct = 0;

        int dir = 1;
        float endDelayTime = Time.time + 1 / speed + delayTime;

        while(pct >= 0){
            pct += Time.deltaTime * speed * dir;

            if(pct >= 1){
                pct = 1;
                if(Time.time > endDelayTime) {
                    dir = -1;
                }
            }

            newWaveBanner.alpha = pct;
            yield return null;
        }

        newWaveBanner.alpha = 0;
    }

    void HandleGunEquip(int selectedIndex){
        StartCoroutine(AnimateGunUI(selectedIndex));
    }

    IEnumerator AnimateGunUI(int selectedIndex){
        float time = 2f;
        float speed = 1 / time;
        float percent = 0;

        float selectedY = -52;
        float unselectedY = -99.65643f;

        while (percent < 1) {
            percent += Time.deltaTime * speed;
            for (int i = 0; i < gunIndicators.Length; i++){
                GameObject indicator = gunIndicators[i];
                Vector3 start = indicator.transform.localPosition;
                Vector3 end;

                if(i == selectedIndex){
                    end = new Vector3(start.x,selectedY,start.z);
                } else {
                    end = new Vector3(start.x,unselectedY,start.z);
                }

                indicator.transform.localPosition = Vector3.Lerp(start, end, percent);
            }

            yield return null;
        }
    }

    public void StartNewGame() {
        string current = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(current);
    }

    
    public void BackToMenu() {
        SceneManager.LoadScene("Menu");
    }
}
