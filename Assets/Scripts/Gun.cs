using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public enum FireMode { Auto, Burst, Single }

    [Header("Assets")]
    public Transform[] projectileSpawn;
    public Projectile projectile;
    public Transform shell;
    public Transform shellEjection;

    [Header("Gun Settings")]
    public FireMode fireMode;
    public AudioClip fireAudio;
    public AudioClip reloadAudio;
    public float msBetweenShots = 100;
    public float muzzleVelocity = 35;
    public float reloadTime = .3f;
    public int clipSize = 100;
    public int burstCount;
    int shotsRemainingInBurst;

    [Header("Recoil Settings")]
    public Vector2 kickbackMinMax = new Vector2(.1f,.3f);
    public Vector2 recoilAngleMinMax = new Vector2(3f,5f);
    public float recoilSettleTime = .1f;
    public float recoilRotSettleTime = .1f;

    bool triggerReleased;

    float nextShotTime;
    MuzzleFlash muzzleFlash;

    bool isReloading;
    int clipRemaining;

    Vector3 recoilSmoothDampVelocity;
    float recoilRotSmoothDampVelocity;
    float recoilAngle;

    void Start(){
        muzzleFlash = GetComponent<MuzzleFlash> ();
        shotsRemainingInBurst = burstCount;
        clipRemaining = clipSize;
    }

    void LateUpdate(){
        //Reset gun for recoil
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, Vector3.zero, ref recoilSmoothDampVelocity, recoilSettleTime);
        recoilAngle = Mathf.SmoothDamp(recoilAngle, 0, ref recoilRotSmoothDampVelocity,recoilRotSettleTime);
        transform.localEulerAngles = transform.localEulerAngles + Vector3.left * recoilAngle;

        if(!isReloading && clipRemaining == 0){
            Reload();
        }
    }

    void Shoot() {
        if(!isReloading && Time.time > nextShotTime && clipRemaining > 0) {

            if(fireMode == FireMode.Burst){
                if(shotsRemainingInBurst == 0){
                    return;
                }

                shotsRemainingInBurst --;
            }
            if(fireMode == FireMode.Single){
                if(!triggerReleased) return;
            }

            nextShotTime = Time.time + msBetweenShots / 1000;

            clipRemaining --;

            for(int i = 0; i < projectileSpawn.Length; i ++){
                Transform muzzle = projectileSpawn[i];
                Projectile newProjectile = Instantiate(projectile, muzzle.position, muzzle.rotation);
                newProjectile.SetSpeed (muzzleVelocity);
            }

            Instantiate(shell, shellEjection.position, shellEjection.rotation);
            muzzleFlash.Activate();

            //Move gun back for recoil
            transform.localPosition -= Vector3.forward * Random.Range(kickbackMinMax.x, kickbackMinMax.y);
            recoilAngle += Random.Range(recoilAngleMinMax.x, recoilAngleMinMax.y);
            recoilAngle = Mathf.Clamp(recoilAngle, 0, 30);

            AudioManager.instance.PlaySound(fireAudio, transform.position);
        }
    }

    public void Aim(Vector3 aimPoint){
        if(!isReloading){
            transform.LookAt(aimPoint);
        }
    }

    public void Reload() {
        if(!isReloading && clipRemaining != clipSize){
            StartCoroutine(DoReload());
            AudioManager.instance.PlaySound(reloadAudio, transform.position);
        }
    }

    IEnumerator DoReload(){
        isReloading = true;
        yield return new WaitForSeconds(.2f);

        float reloadSpeed = 1/reloadTime;
        float maxReloadAngle = 30;
        Vector3 initialRotation = transform.localEulerAngles;

        float pct = 0;
        while(pct < 1){
            pct += Time.deltaTime * reloadSpeed;
            float interpolation = (-Mathf.Pow(pct, 2) + pct) * 4;

            float reloadAngle = Mathf.Lerp(0, maxReloadAngle, interpolation);
            transform.localEulerAngles = initialRotation + Vector3.left * reloadAngle;

            yield return null;
        }

        isReloading = false;
        clipRemaining = clipSize;
    }

    public void OnTriggerHold() {
        Shoot();
        triggerReleased = false;
    }

    public void OnTriggerRelease() {
        triggerReleased = true;
        shotsRemainingInBurst = burstCount;
    }
}
