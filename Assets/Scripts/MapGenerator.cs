using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{

    public bool isEndless;

    public Map[] maps;
    public int mapIndex;

    public Transform tilePrefab;
    public Transform obstaclePrefab;
    public Transform mapFloor;
    public Transform navmeshFloor;
    public Transform navmeshMaskPrefab;
    public Vector2 maxMapSize;

    public float tileSize;

    [Range(0,1)]
    public float outlinePercent;

    List<Coord> allTileCoords;
    Queue<Coord> shuffledTileCoords;
    Queue<Coord> shuffledOpenTileCoords;
    Transform[,] tileMap;

    Map currentMap;

    GameCamera camera;

    void Start()
    {
        FindObjectOfType<Spawner> ().OnNewWave += OnNewWave;
        camera = FindObjectOfType<GameCamera>();
    }

    void OnNewWave(int waveNumber) {
        mapIndex = waveNumber - 1;
        bool isEnd = !isEndless && mapIndex == maps.Length -1;
        GenerateMap(isEnd);
    }

    public void GenerateMap() {
        GenerateMap(false);
    }

    public void GenerateMap(bool isEnd) {
        if(isEndless){
            currentMap = WaveGenerator.NewMap();
        } else{
            currentMap = maps[mapIndex];
        }
        tileMap = new Transform[currentMap.mapSize.x,currentMap.mapSize.y];
        System.Random prng = new System.Random(currentMap.seed);

        allTileCoords = new List<Coord> ();
        for (int x = 0; x < currentMap.mapSize.x; x ++) {
            for (int y = 0; y < currentMap.mapSize.y; y ++) {
                allTileCoords.Add(new Coord(x,y));
            }
        }
        shuffledTileCoords = new Queue<Coord> (Utility.ShuffleArray (allTileCoords.ToArray(), currentMap.seed));

        string holderName = "Generated Map";
        if(transform.FindChild(holderName)) {
            DestroyImmediate(transform.FindChild(holderName).gameObject);
        }

        Transform mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = transform;

        for(int x = 0; x < currentMap.mapSize.x; x++){
            for (int y = 0; y < currentMap.mapSize.y; y++) {
                Vector3 tilePosition = CoordToPosition(x,y);
                Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90));
                newTile.localScale = Vector3.one * (1 - outlinePercent) * tileSize;
                newTile.parent = mapHolder;
                //newTile.gameObject.GetComponent<Renderer>().material.color = new Color((float)y / (float)currentMap.mapSize.y,0,0, (float)x / (float)currentMap.mapSize.x);

                tileMap[x,y] = newTile;
            }
        }

        bool [,] obstacleMap = new bool[(int) currentMap.mapSize.x, (int) currentMap.mapSize.y];

        int obstacleCount = (int) (currentMap.mapSize.x * currentMap.mapSize.y * currentMap.obstaclePercent);
        int currentObstacleCount = 0;

        List<Coord> allOpenCoords = new List<Coord> (allTileCoords);

        if(!isEnd){
            for(int i = 0; i < obstacleCount; i ++) {
                Coord randomCoord = GetRandomCoord();

                obstacleMap[randomCoord.x, randomCoord.y] = true;
                currentObstacleCount ++;

                if(randomCoord != currentMap.mapCenter && MapIsFullyAccessible(obstacleMap, currentObstacleCount)) {
                    float obstacleHeight = Mathf.Lerp(currentMap.minObstacleHeight, currentMap.maxObstacleHeight, (float) prng.NextDouble());
                    Vector3 obstaclePosition = CoordToPosition(randomCoord) + Vector3.up * obstacleHeight / 2;
                    
                    Transform newObstacle = Instantiate(obstaclePrefab, obstaclePosition, Quaternion.identity);
                    float tileXY = (1 - outlinePercent) * tileSize;
                    newObstacle.localScale = new Vector3(tileXY, obstacleHeight, tileXY) ;
                    newObstacle.parent = mapHolder;

                    Renderer obstacleRenderer = newObstacle.GetComponent<Renderer>();
                    Material obstacleMaterial = new Material(obstacleRenderer.sharedMaterial);

                    float colorPercent = randomCoord.x / (float) currentMap.mapSize.x;
                    obstacleMaterial.color = Color.Lerp(currentMap.foregroundColor, currentMap.backgroundColor, colorPercent);
                    obstacleRenderer.sharedMaterial = obstacleMaterial;

                    allOpenCoords.Remove(randomCoord);
                } else {
                    obstacleMap[randomCoord.x, randomCoord.y] = false;
                    currentObstacleCount --;
                }
            }
        } else {
            //Camera points from -x to x
            int mapCY = currentMap.mapCenter.x; 
            int mapCX = currentMap.mapCenter.y;
            Coord[] endCoords = {
                new Coord(mapCX -6,mapCY -1),
                new Coord(mapCX -6,mapCY -2),
                new Coord(mapCX -6,mapCY -3),
                new Coord(mapCX -6,mapCY -4),
                new Coord(mapCX -6,mapCY -5),
                new Coord(mapCX -5,mapCY -1),
                new Coord(mapCX -5,mapCY -3),
                new Coord(mapCX -5,mapCY -5),
                new Coord(mapCX -4,mapCY -1),
                new Coord(mapCX -4,mapCY -5),
                new Coord(mapCX -2,mapCY -1),
                new Coord(mapCX -2,mapCY -2),
                new Coord(mapCX -2,mapCY -3),
                new Coord(mapCX -2,mapCY -4),
                new Coord(mapCX -2,mapCY -5),
                new Coord(mapCX -1,mapCY -2),
                new Coord(mapCX ,mapCY -3),
                new Coord(mapCX +1 ,mapCY -4),
                new Coord(mapCX +2 ,mapCY -1),
                new Coord(mapCX +2 ,mapCY -2),
                new Coord(mapCX +2 ,mapCY -3),
                new Coord(mapCX +2 ,mapCY -4),
                new Coord(mapCX +2 ,mapCY -5),
                new Coord(mapCX +4 ,mapCY -1),
                new Coord(mapCX +4 ,mapCY -2),
                new Coord(mapCX +4 ,mapCY -3),
                new Coord(mapCX +4 ,mapCY -4),
                new Coord(mapCX +4 ,mapCY -5),
                new Coord(mapCX +5 ,mapCY -1),
                new Coord(mapCX +5 ,mapCY -5),
                new Coord(mapCX +6 ,mapCY -2),
                new Coord(mapCX +6 ,mapCY -3),
                new Coord(mapCX +6 ,mapCY -4),
            };

            for(int i = 0; i < endCoords.Length; i++){
                Coord coord = endCoords[i];

                float obstacleHeight = Mathf.Lerp(currentMap.minObstacleHeight, currentMap.maxObstacleHeight, (float) prng.NextDouble());
                Vector3 obstaclePosition = CoordToPosition(coord) + Vector3.up * obstacleHeight / 2;
                
                Transform newObstacle = Instantiate(obstaclePrefab, obstaclePosition, Quaternion.identity);
                float tileXY = (1 - outlinePercent) * tileSize;
                newObstacle.localScale = new Vector3(tileXY, obstacleHeight, tileXY) ;
                newObstacle.parent = mapHolder;

                Renderer obstacleRenderer = newObstacle.GetComponent<Renderer>();
                Material obstacleMaterial = new Material(obstacleRenderer.sharedMaterial);

                float colorPercent = coord.x / (float) currentMap.mapSize.x;
                obstacleMaterial.color = Color.Lerp(currentMap.foregroundColor, currentMap.backgroundColor, colorPercent);
                obstacleRenderer.sharedMaterial = obstacleMaterial;

                allOpenCoords.Remove(coord);
            }
        }

        shuffledOpenTileCoords = new Queue<Coord> (Utility.ShuffleArray(allOpenCoords.ToArray(), currentMap.seed));

        Transform maskLeft = Instantiate (navmeshMaskPrefab, Vector3.left * (currentMap.mapSize.x + maxMapSize.x) / 4f * tileSize, Quaternion.identity);
        maskLeft.parent = mapHolder;
        maskLeft.localScale = new Vector3 ((maxMapSize.x - currentMap.mapSize.x) / 2f, 1, currentMap.mapSize.y) * tileSize;

        Transform maskRight = Instantiate (navmeshMaskPrefab, Vector3.right * (currentMap.mapSize.x + maxMapSize.x) / 4f * tileSize, Quaternion.identity);
        maskRight.parent = mapHolder;
        maskRight.localScale = new Vector3 ((maxMapSize.x - currentMap.mapSize.x) / 2f, 1, currentMap.mapSize.y) * tileSize;

        Transform maskTop = Instantiate (navmeshMaskPrefab, Vector3.forward * (currentMap.mapSize.y + maxMapSize.y) / 4f * tileSize, Quaternion.identity);
        maskTop.parent = mapHolder;
        maskTop.localScale = new Vector3 (maxMapSize.x, 1, (maxMapSize.y - currentMap.mapSize.y) / 2f) * tileSize;

        Transform maskBottom = Instantiate (navmeshMaskPrefab, Vector3.back * (currentMap.mapSize.y + maxMapSize.y) / 4f * tileSize, Quaternion.identity);
        maskBottom.parent = mapHolder;
        maskBottom.localScale = new Vector3 (maxMapSize.x, 1, (maxMapSize.y - currentMap.mapSize.y) / 2f) * tileSize;

        navmeshFloor.localScale = new Vector3 (maxMapSize.x, maxMapSize.y) * tileSize;
        mapFloor.localScale = new Vector3(currentMap.mapSize.x * tileSize, currentMap.mapSize.y * tileSize);

        camera.SmoothMove(GetCameraForMap());
    }

    bool MapIsFullyAccessible(bool[,] obstacleMap, int currentObstacleCount){
        bool [,] mapFlags = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];
        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(currentMap.mapCenter);
        mapFlags[currentMap.mapCenter.x, currentMap.mapCenter.y] = true;

        int accessibleTileCount = 1;

        while (queue.Count > 0) {
            Coord tile = queue.Dequeue();

            for(int x = -1; x <= 1; x++){
                for (int y = -1; y <= 1; y++) {
                    int adjX = tile.x + x;
                    int adjY = tile.y + y;

                    // We only care about directly above or next to
                    if(x == 0 || y == 0) { 
                        // Only check existing tiles in the map
                        if(adjX >= 0 && adjX <obstacleMap.GetLength(0) 
                                && adjY >= 0 && adjY < obstacleMap.GetLength(1)){
                            
                            //If we haven't checked this adjacent tile, and it's not an obstacle
                            if (!mapFlags[adjX, adjY] && !obstacleMap[adjX, adjY]){
                                mapFlags[adjX, adjY] = true;
                                queue.Enqueue(new Coord(adjX, adjY));
                                accessibleTileCount++;
                            }
                        }
                    }
                }
            }
        }

        int targetAccessibleTileCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y - currentObstacleCount);
        return targetAccessibleTileCount == accessibleTileCount;
    }

    Vector3 CoordToPosition(Coord coord){
        return CoordToPosition(coord.x, coord.y);
    }

    Vector3 CoordToPosition(int x, int y){
        return new Vector3(-currentMap.mapSize.x/2f + .5f + x,
                            0,
                            -currentMap.mapSize.y/2f +.5f + y) * tileSize;
    }

    public Transform GetTileFromPosition(Vector3 position){
        int x = Mathf.RoundToInt(position.x / tileSize + (currentMap.mapSize.x - 1) / 2f);
        int y = Mathf.RoundToInt(position.z / tileSize + (currentMap.mapSize.y - 1) / 2f);
        x = Mathf.Clamp(x, 0, tileMap.GetLength(0) -1);
        y = Mathf.Clamp(y, 0, tileMap.GetLength(1) -1);
        return tileMap[x,y];
    }

    public Coord GetRandomCoord() {
        Coord randomCoord = shuffledTileCoords.Dequeue();
        shuffledTileCoords.Enqueue (randomCoord);
        return randomCoord;
    }

    public Transform GetRandomOpenTile() {
        Coord randomCoord = shuffledOpenTileCoords.Dequeue();
        shuffledOpenTileCoords.Enqueue(randomCoord);
        return tileMap[randomCoord.x, randomCoord.y];
    }

    Vector3 GetCameraForMap(){
        List<Vector3> mapEdgeVectors = new List<Vector3>();
        mapEdgeVectors.Add(CoordToPosition(new Coord(-3,-3)));
        mapEdgeVectors.Add(CoordToPosition(new Coord(currentMap.mapSize.x,currentMap.mapSize.y)));

        return GetClosestCameraPos(Camera.main, mapEdgeVectors);
    }

    // float GetClosestCameraDist(Camera cam, Vector3 point)
    // { 
    //     // cam vertical/horizontal fov in radians
    //     float vFov = cam.fieldOfView * Mathf.Deg2Rad;
    //     float hFov = Camera.VerticalToHorizontalFieldOfView(cam.fieldOfView,
    //             cam.aspect) * Mathf.Deg2Rad;

    //     // how far point is from origin along camera's axes
    //     float Yp = Vector3.Dot(point, cam.transform.up);
    //     float Xp = Vector3.Dot(point, cam.transform.right);
    //     float Zp = Vector3.Dot(point, cam.transform.forward);

    //     // how far point should be from cam along cam forward to ensure 
    //     // point is within fov along camera axes
    //     float d = Mathf.Max(
    //             Mathf.Abs(Yp) / Mathf.Tan(vFov/2f),
    //             Mathf.Abs(Xp) / Mathf.Tan(hFov/2f) );

    //     // return how far cam should be from origin along cam forward
    //     // should be negative if it goes cam->  ... origin 
    //     //           positive if it goes origin ... cam->
    //     return Zp - d; 
    // }

    // Vector3 GetClosestCameraPos(Camera cam, List<Vector3> points)
    // {
    //     // calculate lowest needed distance from origin to cam along cam forward
    //     // lowest because the larger this is, the more forward camera is
    //     float c = points.Select(p => GetClosestCameraDist(cam, p)).Min();

    //     // go that distance along cam forward to find closest-to-points position.
    //     return c * cam.transform.forward;
    // }

    float GetClosestCameraDist(Vector3 point, float vCot, float hCot)
    { 
        // point is in camera's local space
        // v/hCot = cotan of half fovs

        // how far point should be from cam along cam forward to ensure 
        //   point is within fov along camera axes
        float d = Mathf.Max(Mathf.Abs(point.y) * vCot,
                            Mathf.Abs(point.x) * hCot);

        // return how far cam needs to move from its current position 
        //   along cam forward
        // should be negative if it goes cam->  ... origin 
        //           positive if it goes origin ... cam->
        return point.z - d; 
    }

    Vector3 GetClosestCameraPos(Camera cam, List<Vector3> points)
    {
        Transform tCam = cam.transform;

        // cam vertical/horizontal fov in radians
        float vFov = cam.fieldOfView * Mathf.Deg2Rad;
        float hFov = Camera.VerticalToHorizontalFieldOfView(cam.fieldOfView,
                cam.aspect) * Mathf.Deg2Rad;

        float vCot = 1f / Mathf.Tan(vFov/2f);
        float hCot = 1f / Mathf.Tan(hFov/2f);

        // calculate lowest needed distance from current position to valid position 
        //   along cam forward
        // lowest because the larger this is, the more forward camera is
        float c = points.Select(p => GetClosestCameraDist(tCam.InverseTransformPoint(p),
                vCot, hCot)).Min();

        // go that distance along cam forward from current cam position 
        // to find closest-to-points valid position.
        return c * tCam.forward + tCam.position;
    }
}
