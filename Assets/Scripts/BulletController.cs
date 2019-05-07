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

    private GameObject effectCollision;
    [SerializeField]private float delayTime;

    private bool isDestroyed;

    // Use this for initialization
    void Start () {
        isDestroyed = false;

        if (hasBulletEffect == true)
        {
            //delayTime = gameObject.GetComponentInChildren<ParticleSystem>().main.duration;
            gameObject.GetComponentInChildren<ParticleStarter>().StartParticle();
        }


    }
	
	// Update is called once per frame
	void Update () {
        BulletDestroy();
    }

    //時間経過での弾の消失処理
    void BulletDestroy()
    {
        //弾丸の消失処理
        time += Time.deltaTime;
        if (time > (bulletDeathTime - 0.4f) && isDestroyed == false)
        {
            //消滅時エフェクトを持たない場合、即時にdestroy処理
            if (hasBulletEffect == false)
            {
                Destroy(gameObject);
            }

            //消滅時エフェクトを持つ場合、実行
            if (hasBulletEffect == true)
            {

                //判定オブジェクトを有効化
                transform.Find("ExplosionCollision").gameObject.SetActive(true);
                //エフェクト再生後の弾の消滅処理を予約
                StartCoroutine("DelayDestroyBullet", delayTime);
                //破壊処理はフラグで1回のみ行う
                isDestroyed = true;

            }
        }
    }

    private IEnumerator DelayDestroyBullet(float DelayTime)
    {
        yield return new WaitForSeconds(DelayTime);
        //弾丸のオブジェクト自体を削除
        Destroy(gameObject);
    }


}
