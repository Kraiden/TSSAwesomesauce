using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    public LayerMask targetMask;
    public SpriteRenderer dot;
    public Color highlightColor;
    Color originalColor;

    void Start(){
        originalColor = dot.color;
        Cursor.visible = false;

        FindObjectOfType<Player> ().OnDeath += OnGameOver;
    }

    void OnGameOver(){
        Cursor.visible = true;
    }

    void Update()
    {
        transform.Rotate (Vector3.forward * 40 * Time.deltaTime);
    }

    public void DetectTargets(Ray ray){
        if(Physics.Raycast(ray, 100, targetMask)) {
            dot.color = highlightColor;
        } else {
            dot.color = originalColor;
        }
    }
}
