using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreKeeper : MonoBehaviour
{
    public static int score { get; private set;}

    float lastKillTime;
    int streakCount;
    float streakExpirey;

    void Start()
    {
        score = 0;
        Enemy.OnDeathAny += OnEnemyDeath;
        FindObjectOfType<Player>().OnDeath += OnPlayerDeath;
    }

    void OnEnemyDeath(){
        if(Time.time < lastKillTime + streakExpirey){
            streakCount++;
        } else {
            streakCount = 0;
        }

        lastKillTime = Time.time;

        score += 5 + (int) Mathf.Pow(2, streakCount);
    }

    void OnPlayerDeath() {
        Enemy.OnDeathAny -= OnEnemyDeath;
    }
}
