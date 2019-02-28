using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {

    private GameObject player;   //プレイヤーの持続時間
    private float time;          //ローカル時間

    //弾丸の属性
    public enum bulletSpecies
    {
        none = 0,
        normal,
        he,
        ap,
        beam,
    };

    //弾丸の情報(インスペクター上で記述)
    public float bulletPower;                       //弾丸の威力
    public bulletSpecies currentBullet;             //弾丸の属性
    public float bulletSpeed;                       //弾速
    public float bulletDeathTime;                   //弾丸の持続時間
    public float bulletFireRate;                    //射撃レート　x/60s * 50fpsで記入


	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        BulletDestroy();

    }

    void BulletDestroy()
    {
        //弾丸の消失処理
        time += Time.deltaTime;
        if (time > bulletDeathTime)
        {
            Destroy(gameObject);
        }
    }



}
