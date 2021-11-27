using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public LayerMask collisionMask;
    float speed = 10;
    float damage = 1;

    float lifetime = 3;
    float skinWidth = .1f;

    void Start() {
        Destroy(gameObject, lifetime);
        Collider[] initialCollisions = Physics.OverlapSphere (transform.position, .1f, collisionMask);
        if(initialCollisions.Length > 0){
            OnHitObject(initialCollisions[0], transform.position);
        }
    }

    public void SetSpeed(float newSpeed) {
        speed = newSpeed;
    }

    void Update()
    {
        float moveDistance = speed * Time.deltaTime;
        CheckCollisions(moveDistance);
        transform.Translate (Vector3.forward * Time.deltaTime * speed);
    }

    void CheckCollisions(float moveDistance) {
        Ray ray = new Ray (transform.position, transform.forward);
        RaycastHit hit;

        if(Physics.Raycast(ray, 
                        out hit,
                        moveDistance + skinWidth, 
                        collisionMask, 
                        QueryTriggerInteraction.Collide)) {
            OnHitObject(hit.collider, hit.point);
        }
    }

    void OnHitObject(Collider c, Vector3 hitPoint){
        IDamagable damageObject = c.GetComponent<IDamagable> ();
        if(damageObject != null) {
            damageObject.TakeHit(damage, hitPoint, transform.forward);
        }
        GameObject.Destroy(gameObject);
    }
}
