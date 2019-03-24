using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameParameters;

namespace GameParameters
{
    //ゲームステートをenum型で設定
    public enum GameState
    {
        play = 1,
        boosted,
    }

    //メインウェポンの種類
    public enum WeaponState
    {
        none = 0,
        doubleMachineGun,
        longRifle,


    }

    //サブウェポンの種類
    public enum SubWeaponState
    {
        none = 0,
        missilePod,
        rocketLauncher,
        cannon,
        shield,


    }

    //弾丸の属性
    public enum BulletSpecies
    {
        none = 0,
        normal,
        he,
        ap,
        beam,
    };
    /*ここまで*/

        
}


public class GameParameter : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
