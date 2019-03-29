using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapondata : ScriptableObject {

    public List<WeaponStatus> weaponStatusList = new List<WeaponStatus>();
}

[System.Serializable]
public class WeaponStatus
{
    //武器の名称
    public string weaponName;

    //武器設定に含まれるデータ 
    [SerializeField]public List<GameObject> ListofMuzzleObject;

    //武器のモーション
    public string idleMotionFlag;

    //射撃時に呼び出す処理の名称

    public string shotMethodName;

    //読み込む弾丸
    public string bulletPrefabName;

}
