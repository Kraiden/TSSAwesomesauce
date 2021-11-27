using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public Transform weaponHold;
    public Gun[] guns;

    Gun equipped;

    public static System.Action<int> OnEquipNew;

    void Start() {
        if(equipped == null){
            Equip(0);
        }
    }

    public void Equip(int gunIndex){
        if(gunIndex >= 0 && gunIndex < guns.Length){
            if(guns[gunIndex] != equipped){
                if(OnEquipNew != null){
                    OnEquipNew(gunIndex);
                }
                Equip(guns[gunIndex]);
            }
        }
    }

    public void Equip(Gun toEquip) {
        if(equipped != null) {
            Destroy(equipped.gameObject);
        }

        equipped = Instantiate(toEquip, weaponHold.position, weaponHold.rotation);
        equipped.transform.parent = weaponHold;
    }

    public void OnTriggerHold(){
        if(equipped != null){
            equipped.OnTriggerHold();
        }
    }

    public void OnTriggerRelease() {
        if(equipped != null) {
            equipped.OnTriggerRelease();
        }
    }

    public void Aim(Vector3 aimPoint) {
        if(equipped != null) {
            equipped.Aim(aimPoint);
        }
    }

    public void Reload(){
        if(equipped != null) {
            equipped.Reload();
        }
    }

    public float gunHeight{
        get {
            return weaponHold.position.y;
        }
    }
}
