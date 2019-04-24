using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameParameters;



public class BulletController : MonoBehaviour {

    private GameObject player;   //プレイヤーの持続時間
    private float time;          //ローカル時間

    //弾丸の情報(インスペクター上で記述)
    public bulletSpecies currentBullet;             //弾丸の属性
    public float bulletPower;                       //弾丸の威力
    public float bulletDeathTime;                   //弾丸の持続時間
    public bool hasBulletEffect;                    //エフェクトを持つかどうか

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

            if(hasBulletEffect == true)
            {
                StartCoroutine("setEffectStarter");
            }
        }
    }

    private IEnumerator setEffectStarter()
    {
        yield return new WaitForSeconds(bulletDeathTime);
        gameObject.GetComponentInChildren<ParticleStarter>().StartParticle();
    }


}
