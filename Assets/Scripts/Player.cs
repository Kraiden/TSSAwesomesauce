using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (PlayerController)) ]
[RequireComponent (typeof (GunController)) ]
public class Player : LivingEntity
{
    public float moveSpeed = 10;

    public Crosshair crosshairs;

    Camera viewCamera;
    PlayerController controller;
    GunController gunController;

    protected override void Start() {
        base.Start();
    }

    void Awake() {
        controller = GetComponent<PlayerController> ();
        gunController = GetComponent<GunController> ();
        viewCamera = Camera.main;
        FindObjectOfType<Spawner>().OnNewWave += OnNewWave;
    }

    void OnNewWave(int waveNumber){
        health = startingHealth;
    }

    void Update()
    {
        handleMove();
        handleLook();
        handleWeapon();
    }

    private void handleMove() {
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"),0,Input.GetAxisRaw("Vertical"));
        Vector3 moveVelocity = moveInput.normalized * moveSpeed;

        controller.Move(moveVelocity);

        if(transform.position.y < -10){
            TakeDamage(health);
        }
    }

    private void handleLook() {
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.up * gunController.gunHeight);
        float rayDistance;

        if(groundPlane.Raycast(ray, out rayDistance)) {
            Vector3 point = ray.GetPoint(rayDistance);
            Debug.DrawLine(ray.origin, point, Color.red);

            controller.LookAt(point);
            crosshairs.transform.position = point;
            crosshairs.DetectTargets(ray);
            if((new Vector2(point.x, point.z) - new Vector2(transform.position.x, transform.position.z)).sqrMagnitude > 1){
                gunController.Aim(point);
            }
        }
    }

    private void handleWeapon() {
        handleFireWeapon();
        handleReloadWeapon();
        handleChangeWeapon();
    }

    private void handleFireWeapon(){
        if(Input.GetMouseButton(0)) {
            gunController.OnTriggerHold();
        }
        if(Input.GetMouseButtonUp(0)){
            gunController.OnTriggerRelease();
        }
    }

    private void handleReloadWeapon(){
        if(Input.GetKeyDown(KeyCode.R)){
            gunController.Reload();
        }
    }

    private void handleChangeWeapon(){
        int changeTo = -1;
        if(Input.GetKeyDown(KeyCode.Keypad1)){
            changeTo = 0;          
        }
        if(Input.GetKeyDown(KeyCode.Keypad2)){
            changeTo = 1;          
        }
        if(Input.GetKeyDown(KeyCode.Keypad3)){
            changeTo = 2;          
        }
        if(Input.GetKeyDown(KeyCode.Keypad4)){
            changeTo = 3;          
        }
        if(Input.GetKeyDown(KeyCode.Keypad5)){
            changeTo = 4;          
        }
        if(Input.GetKeyDown(KeyCode.Keypad6)){
            changeTo = 5;          
        }

        if(changeTo >= 0){
            gunController.Equip(changeTo);
        }
    }

    public override void Die()
    {
        base.Die();
        AudioManager.instance.PlaySound("Player Death", transform.position);
    }
}
