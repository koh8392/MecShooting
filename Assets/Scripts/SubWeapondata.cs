﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameParameters;



[CreateAssetMenu(menuName = "SubWeaponData/Create")]
public class SubWeapondata : ScriptableObject {

    public List<SubWeaponStatus> subWeaponStatusList = new List<SubWeaponStatus>();
}

[System.Serializable]
public class SubWeaponStatus
{
    //武器の名称
    public string weaponName;

    //弾丸の発射点の個数
    public int numOfMuzzles;

    //発射点として設定するオブジェクトの名称
    [SerializeField]public List<string> listofMuzzleObjectName;

    //見た目上のオブジェクトの名称(のリスト)
    [SerializeField] public List<string> listofVisualObjectName;

    //発射点のオブジェクトからのオフセット
    public Vector3 muzzleOffset;

    //同時発射する弾丸の個数
    public int numOfBulletsPerMuzzle;

    //弾丸ごとのオフセット
    public Vector3 bulletOffset;

    //武器のモーション
    public string idleMotionFlag;

    //射撃時に呼び出す処理の名称

    public string shotMethodName;

    //読み込む弾丸
    public string bulletPrefabName;

    //弾丸のエネルギー消費量
    public int bulletConsumption;

    //弾丸の属性
    public bulletSpecies currentBullet;

    //弾丸の威力
    public float bulletPower;

    //弾速
    public float bulletSpeed;

    //弾丸の持続時間
    public float bulletDeathTime;

    //射撃レート　x/(60s * 60fps)で記入
    public float bulletFireRate;

    //弾丸にエフェクト再生処理を行うかどうか
    public bool hasBulletEffect;

    //弾丸が敵を追尾するかどうか
    public bool hasSeeker;

}
