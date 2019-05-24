using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameParameters;



public class BulletController : DelayScript {

    private GameObject player;   //プレイヤーの持続時間
    private float time;          //ローカル時間

    //弾丸の情報(インスペクター上で記述)
    public bulletSpecies currentBullet;             //弾丸の属性
    public float bulletPower;                       //弾丸の威力
    public float bulletDeathTime;                   //弾丸の持続時間
    public float bulletSpeed;                       //弾丸の速さ
    public bool hasBulletEffect;                    //エフェクトを持つかどうか
    public bool hasSeeker;                          //追尾機能を持つかどうか
    public float guideExtent;                       //追尾機能の強さ

    private GameObject effectCollision;
    [SerializeField]private float delayTime;

    private bool isDestroyed;

    private GameObject targetObject;

    // Use this for initialization
    void Start () {
        isDestroyed = false;
        bulletSpeed = 1;

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

    private void FixedUpdate()
    {
        if(hasSeeker == true)
        {
            SeekEnemy();
        }
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
                StartCoroutine(Delay(delayTime, () => {
                    //弾丸のオブジェクト自体を削除
                    Destroy(gameObject);
                }));


                //破壊処理はフラグで1回のみ行う
                isDestroyed = true;

            }
        }
    }
    public void SetTarget(GameObject target) {
        //GameObjectのターゲットをPlayerから受け取り座標を計算し、進行方向を決定。
        targetObject = target;
        transform.LookAt(targetObject.transform);
        hasSeeker = true;
        Debug.Log(bulletDeathTime);
    }


    private void SeekEnemy()
    {
        //前進しながら一定時間ごとに敵の位置に向けて進行方向を変更する。
        transform.LookAt(targetObject.transform);
        gameObject.transform.Translate(new Vector3(0, 0, bulletSpeed));
    }


}
