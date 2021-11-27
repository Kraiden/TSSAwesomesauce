using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
    Camera viewCamera;
    Vector3 original;
    // Start is called before the first frame update
    void Start()
    {
        viewCamera = Camera.main;
        original = viewCamera.transform.position;
    }

    public void SmoothMove(Vector3 to){
        StartCoroutine(MoveCamera(to));
    }

    IEnumerator MoveCamera(Vector3 to){
        float moveTime = 1f;
        float moveSpeed = 1f / moveTime;
        
        Vector3 start = viewCamera.transform.position;

        float pct = 0f;
        while(pct <= 1){
            pct += Time.deltaTime * moveSpeed;

            float interpolation = -Mathf.Pow(pct, 2) + 2 * pct;
            viewCamera.transform.position = Vector3.Lerp(start, to, pct);; 
            yield return null;
        }
    }
}