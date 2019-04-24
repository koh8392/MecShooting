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
    public enum WeaponType
    {
        doubleMachineGun = 0,
        longRifle,


    }

    //サブウェポンの種類
    public enum SubWeaponType
    { 
        cannon = 0,
        shield,


    }

    //弾丸の属性
    public enum bulletSpecies
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
