using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public bool isEndless;

    public Wave[] waves;
    public Enemy enemy;
    public Material floorMaterial;

    LivingEntity playerEntity;
    Transform playerT;

    Wave currentWave;
    int currentWaveNumber;

    int enemiesAlive;
    int enemiesRemainingToSpawn;
    float nextSpawnTime;

    MapGenerator map;

    float campCheckTime = 2;
    float campDistanceThreshold = 1.5f;
    float nextCampCheckTime;
    Vector3 campPositionOld;
    bool isCamping;
    bool isEnd;
    bool isDisabled;

    float timeBetweenSpawns;

    Color initialColor;

    public event System.Action<int> OnNewWave;

    public bool IsEnd{
        get {
            return isEnd;
        }
    }

    void Start()
    {
        initialColor = floorMaterial.color;

        playerEntity = FindObjectOfType<Player> ();
        playerT = playerEntity.transform;

        playerEntity.OnDeath += OnPlayerDeath;

        nextCampCheckTime = campCheckTime + Time.time;
        campPositionOld = playerT.position;
        
        map = FindObjectOfType<MapGenerator>();
        NextWave();
    }

    void Update()
    {
        if(!isDisabled){
            if(Time.time > nextCampCheckTime) {
                nextCampCheckTime = Time.time + campCheckTime;

                isCamping = (Vector3.Distance(playerT.position,campPositionOld) < campDistanceThreshold);
                campPositionOld = playerT.position;
            }

            if((enemiesRemainingToSpawn > 0 || isEnd) && Time.time > nextSpawnTime) {
                enemiesRemainingToSpawn --;
                if(timeBetweenSpawns > 0){
                    timeBetweenSpawns -= currentWave.spawnSpeedUp;
                }
                nextSpawnTime = Time.time + timeBetweenSpawns;

                StartCoroutine(SpawnEnemy());
            }
        }
    }

    IEnumerator SpawnEnemy() {
        float spawnDelay = 1;
        float tileFlashSpeed = 4;

        Transform spawnTile = map.GetRandomOpenTile();
        if(isCamping){
            spawnTile = map.GetTileFromPosition(playerT.position);
        }

        Material tileMaterial = spawnTile.GetComponent<Renderer>().material;

        Color flashColor = Color.red;

        float spawnTimer = 0;
        while(spawnTimer < spawnDelay) {
            tileMaterial.color = Color.Lerp(initialColor, flashColor, Mathf.PingPong(spawnTimer * tileFlashSpeed, 1));

            spawnTimer += Time.deltaTime;
            yield return null;
        }

        Enemy spawned = Instantiate(enemy, spawnTile.position + Vector3.up, Quaternion.identity);
        spawned.OnDeath += OnEnemyDeath;
        Color color = isEnd ? Random.ColorHSV(0,1,0.5f,1,0.5f,1,1,1) : currentWave.skinColor;
        spawned.SetWaveSettings(currentWave.moveSpeed, currentWave.hitsToPlayer, currentWave.enemyHealth, color);
    }

    void OnPlayerDeath() {
        isDisabled = true;
    }

    void OnEnemyDeath() {
        enemiesAlive --;

        if(enemiesAlive == 0) {
            NextWave();
        }
    }

    void ResetPlayerPosition() {
        //playerT.position = map.GetTileFromPosition (Vector3.zero).position + Vector3.up * 3;
        playerT.position = Vector3.zero + Vector3.up * 3;
    }

    void NextWave() {
        if(currentWaveNumber > 0){
            AudioManager.instance.PlaySound2d("Next Wave");
        }
        currentWaveNumber ++;
        if(isEndless){
            isEnd = false;
            currentWave = WaveGenerator.NewWave();
            StartWave();

        } else if((currentWaveNumber -1 < waves.Length)) {
            isEnd = !isEndless && currentWaveNumber == waves.Length;
            currentWave = waves [currentWaveNumber -1];
            StartWave();
        } // else don't start wave

            
    }

    private void StartWave(){
        timeBetweenSpawns = currentWave.timeBetweenSpawns;

        enemiesRemainingToSpawn = currentWave.enemyCount;
        enemiesAlive = enemiesRemainingToSpawn;

        if(OnNewWave != null) {
            OnNewWave(currentWaveNumber);
        }

        ResetPlayerPosition();
    }
}
