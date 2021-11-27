using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Wave {
    public int enemyCount;
    public float timeBetweenSpawns;
    public float spawnSpeedUp;

    public float moveSpeed;
    public int hitsToPlayer;
    public float enemyHealth;
    public Color skinColor;
}