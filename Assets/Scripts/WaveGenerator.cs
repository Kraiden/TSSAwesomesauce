using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveGenerator{
    private static System.Random rando = new System.Random();

    public static void SetSeed(int seed){
        rando = new System.Random(seed);
    }

    public static Wave NewWave(){
        Wave wave = new Wave();
        wave.enemyCount = rando.Next(1,25);
        wave.timeBetweenSpawns = rando.Next(0,2);
        wave.spawnSpeedUp = 0;
        wave.moveSpeed = rando.Next(3,10);
        wave.hitsToPlayer = rando.Next(2,10);
        wave.enemyHealth = rando.Next(1,5);
        Color skinColor = randColor();

        return wave;
    }

    public static Map NewMap(){
        Map map = new Map();
        map.mapSize = new Coord(rando.Next(3, 50),rando.Next(3, 50));
        map.obstaclePercent = randPct(0.05f,0.75f);
        map.seed = rando.Next();
        Vector2 height = randHeight();
        map.minObstacleHeight = (float) height.x;
        map.maxObstacleHeight = (float) height.y;
        map.foregroundColor = randColor();
        map.backgroundColor = randColor();

        return map;
    }

    private static Color randColor(){
        return new Color(randPct(),randPct(), randPct());
    }

    private static float randPct(float min = 0f, float max = 1f) {
        float random = rando.Next(100) / 100f;
        return Mathf.Clamp(random,min,max);
    }

    private static Vector2 randHeight() {
        int rand1 = rando.Next(2,5);
        int rand2 = rando.Next(2,5);

        return new Vector2(
            Mathf.Min(rand1, rand2),
            Mathf.Max(rand1, rand2)
        );
    }

    

}