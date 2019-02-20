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
    private GameObject bullet;                  //生成する弾丸
    private GameObject bulletPrefab;
    private GameObject Muzzle;                  //銃口のゲームオブジェクト
    private Transform MuzzleTransform;          //銃口のTransform
    private Vector3 shootForce;                 //射撃する際に加える力
    [SerializeField] private float bulletSpeed; //弾速
    public float bulletDeathTime;               //弾丸の消失時間
    public float reloadTime;                    //リロード用のタイマー
    [SerializeField] private float reloadInterval; //リロード間隔

    // Use this for initialization
    void Start () {
        playerRigidBody = GetComponent<Rigidbody>();
        Muzzle = GameObject.Find("PlayerMuzzle");
        bulletDeathTime = 2;
        reloadTime = reloadInterval;

    }

    // Update is called once per frame
    void FixedUpdate () {
        PlayerMove();

        ReloadTimer();

        if (Input.GetKey("space"))
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
        if (reloadTime >= reloadInterval)
        {
            //リロードタイマーを0にする
            reloadTime = 0.0f;

            //銃弾の生成
            bulletPrefab = (GameObject)Resources.Load("Prefabs/Bullet");
            MuzzleTransform = Muzzle.transform;
            bullet = Instantiate(bulletPrefab, MuzzleTransform.position, MuzzleTransform.rotation) as GameObject;

            //銃弾に発射処理を行う。
            shootForce = Muzzle.transform.forward * bulletSpeed;
            bullet.GetComponent<Rigidbody>().AddForce(shootForce);

        }


    }

    void ReloadTimer()
    {
        reloadTime += Time.deltaTime;
    }
}
