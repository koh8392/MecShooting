using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    //プレイヤーの移動に関する変数
    [SerializeField] private float playerMoveSpeedX; //横方向の移動スピード
    [SerializeField] private float playerMoveSpeedY; //縦方向の移動スピード
    private Rigidbody playerRigidBody;               //プレイヤーの物理判定
    Vector3 playerPosition;                          //プレイヤーの現在位置

    [SerializeField] private float limitX;
    [SerializeField] private float bottomLimitY;
    [SerializeField] private float upperLimitY;

    //プレイヤーの射撃に関する変数
    private GameObject bullet;                     //生成する弾丸
    private GameObject bulletPrefab;               //弾丸のプレファブ
    private GameObject Muzzle;                     //銃口のゲームオブジェクト
    private Transform  MuzzleTransform;            //銃口のTransform
    private Vector3 shootForce;                    //射撃する際に加える力
    public float reloadTime;                       //リロード用のタイマー
    public bool isAutoShot;                        //オートで射撃を行うか

    //弾丸の属性
    private enum bulletSpecies
    {
        none = 0,
        normal,
        he,
        ap,
        beam,
    };

    //弾丸の情報
    //private float bulletPower;                       //弾丸の威力
    //private bulletSpecies currentBullet;             //弾丸の属性
    private float bulletSpeed;                         //弾速
    //private float bulletDeathTime;                   //弾丸の消失時間
    private float bulletFireRate;                      //リロード間隔

    // Use this for initialization
    void Start() {
        //プレイヤー移動に関する初期処理
        playerRigidBody = GetComponent<Rigidbody>();

        //射撃に関する初期処理
        //弾丸のプレハブをロード
        bulletPrefab = (GameObject)Resources.Load("Prefabs/Bullet");

        //弾丸のパラメータを事前取得
        bulletFireRate = bulletPrefab.GetComponent<BulletController>().bulletFireRate;
        //射撃レートを秒数に変換(レート/60秒/FixedUpdate50FPS)
        bulletFireRate = bulletFireRate / 3000;
        bulletSpeed = bulletPrefab.GetComponent<BulletController>().bulletSpeed;
        //マズルを取得
        Muzzle = GameObject.Find("PlayerMuzzle");
        reloadTime = bulletFireRate;

    }

    // Update is called once per frame
    void FixedUpdate() {
        PlayerMove();

        ReloadTimer();

        //スペースを押している間orオート射撃がオンの際に射撃関数を実行する。
        if (Input.GetKey("space") || isAutoShot == true)
        {
            PlayerShot();
        }



    }

    //プレイヤーの移動に関する処理
    void PlayerMove()
    {
        //プレイヤーの位置を取得
        playerPosition = playerRigidBody.position;

        //現在の方向キーの入力の値を取得
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        //プレイヤーが次に動く座標を設定
        playerPosition = new Vector3(
            //座標の計算。Clampを使用して次の位置をlimit内に収める。
            Mathf.Clamp(playerRigidBody.position.x + (moveX * playerMoveSpeedX), -limitX, limitX),
            Mathf.Clamp(playerRigidBody.position.y + (moveY * playerMoveSpeedY), -bottomLimitY, upperLimitY),
            0);

        //次に動く座標をプレイヤーの位置に適用
        playerRigidBody.position = playerPosition;
    }

    //プレイヤーの攻撃に関する処理
    void PlayerShot()
    {
        //前回の発射からの経過時間(reloadTime)がリロードに掛かる時間(bulletFireRate)より長ければ発射処理を行う。
        if (reloadTime >= bulletFireRate)
        {
            //リロードタイマーを0にする
            reloadTime = 0.0f;

            //銃弾の生成
            MuzzleTransform = Muzzle.transform;
            bullet = Instantiate(bulletPrefab, MuzzleTransform.position, MuzzleTransform.rotation) as GameObject;


            //銃弾に発射処理を行う。
            shootForce = Muzzle.transform.forward * bulletSpeed;
            bullet.GetComponent<Rigidbody>().AddForce(shootForce);

        }


    }

    //リロード時間計算のためのタイマー
    void ReloadTimer()
    {
        reloadTime += Time.deltaTime;
    }
}
