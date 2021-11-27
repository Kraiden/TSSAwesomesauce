using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent (typeof(NavMeshAgent))]
public class Enemy : LivingEntity
{
    public static event System.Action OnDeathAny;
    public enum State{
        Idle, Chasing, Attacking
    }
    State currentState;

    public ParticleSystem deathEffect;

    NavMeshAgent pathfinder;
    Transform target;
    LivingEntity targetEntity;
    Material skinMaterial;

    Color originalColor;

    float attackDistanceThreshold = .5f;
    float timeBetweenAttacks = 1;
    float damage = 1;

    float nextAttackTime;

    float selfCollisionRadius;
    float targetCollisionRadius;

    bool hasTarget;

    void Awake(){
        pathfinder = GetComponent<NavMeshAgent> ();

        if(GameObject.FindGameObjectWithTag("Player") != null){ 
            hasTarget = true;

            target = GameObject.FindGameObjectWithTag("Player").transform;
            targetEntity = target.GetComponent<LivingEntity> ();

            selfCollisionRadius = GetComponent<CapsuleCollider> ().radius;
            targetCollisionRadius = target.GetComponent<CapsuleCollider> ().radius;
        }
    }

    protected override void Start() {
        base.Start();

        if(hasTarget){ 
            currentState = State.Chasing;
            targetEntity.OnDeath += OnTargetDeath;  

            StartCoroutine (UpdatePath());  
        }
    }

    public void SetWaveSettings(float moveSpeed, int hitsToPlayer, float enemyHealth, Color skinColor){
        pathfinder.speed = moveSpeed;
        
        if(hasTarget){
            damage = Mathf.Ceil(targetEntity.startingHealth / hitsToPlayer);
        }

        startingHealth = enemyHealth;

        skinMaterial = GetComponent<Renderer>().material;
        skinMaterial.color = skinColor;
        originalColor = skinMaterial.color;

    }

    public override void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        AudioManager.instance.PlaySound("Impact",transform.position);
        if(damage >= health){
            AudioManager.instance.PlaySound("Enemy Death",transform.position);
            if(OnDeathAny != null){
                OnDeathAny();
            }
            GameObject de = Instantiate(deathEffect.gameObject, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitDirection));
            de.GetComponent<Renderer>().material.color = skinMaterial.color;
            Destroy(de, deathEffect.startLifetime);
        }
        base.TakeHit(damage, hitPoint, hitDirection);
    }

    void OnTargetDeath() {

        hasTarget = false;
        currentState = State.Idle;
    }

    void Update()
    {
        if(target != null && Time.time > nextAttackTime){
            float sqrDstToTarget = (target.position - transform.position).sqrMagnitude;
            if(sqrDstToTarget < Mathf.Pow(attackDistanceThreshold + selfCollisionRadius + targetCollisionRadius, 2)) {
                nextAttackTime = Time.time + timeBetweenAttacks;
                AudioManager.instance.PlaySound("Enemy Attack",transform.position);
                StartCoroutine(Attack());
            }
        }
    }

    IEnumerator Attack() {
        currentState = State.Attacking;
        pathfinder.enabled = false;

        Vector3 origin = transform.position;
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        Vector3 end = target.position - dirToTarget * selfCollisionRadius;

        float attackSpeed = 3;
        float percent = 0;

        skinMaterial.color = Color.red;
        bool hasAppliedDamage = false;

        while (percent <= 1){

            if(percent >= .5f && !hasAppliedDamage) {
                hasAppliedDamage = true;
                targetEntity.TakeDamage(damage);
            }

            percent += Time.deltaTime * attackSpeed;
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            transform.position = Vector3.Lerp(origin, end, interpolation);

            yield return null;
        }

        skinMaterial.color = originalColor;
        currentState = State.Chasing;
        pathfinder.enabled = true;
    }

    IEnumerator UpdatePath() {
        float refreshRate = .25f;

        while(target != null) {
            if(currentState == State.Chasing){
                Vector3 dirToTarget = (target.position - transform.position).normalized;
                Vector3 targetPosition = target.position - dirToTarget * (selfCollisionRadius + targetCollisionRadius + attackDistanceThreshold/2);
                if(!dead){
                    pathfinder.SetDestination(targetPosition);
                }
            }
            yield return new WaitForSeconds(refreshRate);
        }
    }
}
