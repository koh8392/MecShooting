using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameParameters;

public class WeaponChangeUIController : MonoBehaviour {

    [SerializeField] WeaponType switchingWeaponType;
    [SerializeField] SubWeaponType switchingSubWeaponType;
    UIcontroller uIcontroller;

    private void Start()
    {
        uIcontroller = GameObject.Find("GameManager").GetComponent<UIcontroller>();
    }

    public void changeWeapon()
    {
        uIcontroller.weaponChangeButton(switchingWeaponType);
    }

    public void changeSubWeapon()
    {
        uIcontroller.subWeaponChangeButton(switchingSubWeaponType);
    }
}
