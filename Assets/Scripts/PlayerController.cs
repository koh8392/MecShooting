using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using GameParameters;

public class PlayerController : MonoBehaviour {

    private GameState currentGameState;

    //プレイヤーの移動に関する変数
    public float moveX;
    private float moveY;
    [SerializeField] public float playerMoveSpeedX; //横方向の移動スピード
    [SerializeField] public float playerMoveSpeedY; //縦方向の移動スピード
    [SerializeField] public float playerMoveSpeedZ; //縦方向の移動スピード
    private Rigidbody playerRigidBody;               //プレイヤーの物理判定
    Vector3 playerPosition;                          //プレイヤーの現在位置

    [SerializeField] private float limitX;
    [SerializeField] private float bottomLimitY;
    [SerializeField] private float upperLimitY;

    private float rollRecastTimer;                    //緊急回避のタイマー
    [SerializeField] private float rollRecastTime;     //緊急回避の間隔
    [SerializeField] private float rollDistance;       //緊急回避の想定距離
    private float finallyRollDistance;                //緊急回避の最終移動距離
    private bool isRolling;                           //緊急回避状態かどうか
    private int currentRollDirection;

    public float boostGage; //ブーストゲージ
    [SerializeField] private float boostConsumption;

    //アニメーションに関する処理
    private GameObject playerModel;
    private Animator playerAnimator;
    public bool isAnimationChanged;
    public float moveMotionTimer;
    [SerializeField] private float moveRecastTime;
    public bool isMoveAnimationChanged;

    //プレイヤーの射撃に関する変数
    private GameObject bullet01;                     //生成する弾丸
    private GameObject bullet02;
    private GameObject bulletPrefab;               //弾丸のプレファブ
    private GameObject Muzzle;                     //銃口のゲームオブジェクト
    private Transform  MuzzleTransform;            //銃口のTransform
    private Vector3 shootForce;                    //射撃する際に加える力
    private float reloadTimer;                     //リロード用のタイマー
    public bool isAutoShot;                        //オートで射撃を行うか
    private WeaponState weaponState;               //武器の種類
    private SubWeaponState subWeaponState;         //サブウェポンの種類
    private GameObject doubleMG_L;
    private GameObject doubleMG_R;
    private Transform doubleMG_L_transform;
    private Transform doubleMG_R_transform;
    private Vector3 doubleMGL_bulletPos;
    private Vector3 doubleMGR_bulletPos;

    public float magazineGage;                     //弾倉の残量
    private float magagineConsumption;             //弾倉の発射時の消費量


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
        isRolling = false;
        rollRecastTimer = rollRecastTime;
        currentGameState = GameManager.gameState;

    　　//アニメーションに関する処理
   　　 playerModel = GameObject.Find("mechmodel");
        playerAnimator = playerModel.GetComponent<Animator>();
        playerAnimator.SetBool("rolltoLeft", false);
        playerAnimator.SetBool("rolltoRight", false);
        playerAnimator.SetBool("Doubleshot_shot", false);
        currentRollDirection = 0;
        isAnimationChanged = false;
        isMoveAnimationChanged = false;

        //射撃に関する初期処理
        //武器の種類を設定
        weaponState = WeaponState.doubleMachineGun;

        //弾丸のプレハブをロード
        bulletPrefab = (GameObject)Resources.Load("Prefabs/Bullet");

        //弾丸のパラメータを事前取得
        bulletFireRate = bulletPrefab.GetComponent<BulletController>().bulletFireRate;
        //射撃レートを秒数に変換(レート/60秒/FixedUpdate50FPS)
        bulletFireRate = bulletFireRate / 3000;
        bulletSpeed = bulletPrefab.GetComponent<BulletController>().bulletSpeed;
        //マズルを取得
        Muzzle = GameObject.Find("PlayerMuzzle");
        reloadTimer = bulletFireRate;

        //ブーストゲージと弾倉ゲージの初期値を設定
        boostGage = 0.0f;
        magazineGage = 0.0f;

        //ダブルショット時の武器のオブジェクトの取得
        doubleMG_L = transform.Find("mechmodel/L_CarbinRifle").gameObject;
        doubleMG_L_transform = doubleMG_L.GetComponent<Transform>();
        doubleMG_R = transform.Find("mechmodel/R_CarbinRifle").gameObject;
        doubleMG_R_transform = doubleMG_R.GetComponent<Transform>();

        //初期の武器に応じて武器のエネルギー使用量を決定
        switch (weaponState) {
            
            case WeaponState.doubleMachineGun:
                magagineConsumption = 12;
                break;
            case WeaponState.longRifle:
                magagineConsumption = 20;
                break;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(GameManager.gameState == GameState.boosted)
        {
            StateManager();
        }

        if (GameManager.gameState == GameState.play)
        {
            Roll();
            PlayerMove();
            Timer();
            PlayerShot();
            StateManager();
            BoostGageCharger();
        }
    }

    private void StateManager()
    {
        //保存しているゲームステートとGameManagerのゲームステートが異なる場合。
        if (GameManager.gameState != currentGameState && isAnimationChanged == false)
        {
            //ゲームマネージャーの値に応じて処理を変更
            switch(GameManager.gameState)
            {
                case GameState.play:
                playerAnimator.SetBool("set_Play", true);
                break;

                case GameState.boosted:
                        break;

            }

            //現在のゲームステイトを保存し、再処理されないようにフラグをオン
            currentGameState = GameManager.gameState;
            isAnimationChanged = true;
        }

        if (GameManager.gameState == currentGameState)
        {
            //ゲームステイトに変化がなければ再処理防止フラグをオフ
            if (isAnimationChanged == true)
            {
                isAnimationChanged = false;
            }
        }

    }

    //時間で回復するパラメーターの回復処理
    private void BoostGageCharger()
    {
        if (GameManager.masterTime >= 8.0f)
        {
            if (boostGage < 100)
            {
                boostGage += 1;
            }

            if (magazineGage < 100)
            {
                magazineGage += 1;
            }
        }
    }

    //プレイヤーの移動に関する処理
    void PlayerMove()
    {

        if (isRolling == false){

            //プレイヤーの位置を取得
            //playerPosition = playerRigidBody.position;

            //現在の方向キーの入力の値を取得
            moveX = Input.GetAxis("Horizontal");
            moveY = Input.GetAxis("Vertical");

                if (-1 <= moveX && moveX < -0.1
                //現在アニメーション移行中の場合は実行じない。(遷移中に次のモーションが予約され意図しない動作を行ってしまうため。)
                    && !playerAnimator.IsInTransition(0))
                {
                    playerAnimator.SetBool("move_left02", true);
                    //transform.rotation = Quaternion.Euler(0, 0, 15);
                }
                if (-0.1 < moveX && moveX <= 0.1)
                {
                    playerAnimator.SetBool("move_left02", false);
                    playerAnimator.SetBool("move_right02", false);
                    //transform.rotation = Quaternion.Euler(0, 0, 0);
                }
                if (0.1 <= moveX && moveX <= 1
                    && !playerAnimator.IsInTransition(0))
                {
                    playerAnimator.SetBool("move_right02", true);
                    //transform.rotation = Quaternion.Euler(0, 0, -15);
                }

            gameObject.transform.Translate((moveX * playerMoveSpeedX), (moveY * playerMoveSpeedY),0,Space.Self);

/*
            //プレイヤーが次に動く座標を設定
            playerPosition = new Vector3(
                //座標の計算。Clampを使用して次の位置をlimit内に収める。
                Mathf.Clamp(playerRigidBody.position.x + (moveX * playerMoveSpeedX), -limitX, limitX),
                Mathf.Clamp(playerRigidBody.position.y + (moveY * playerMoveSpeedY), -bottomLimitY, upperLimitY),
                0);

            //次に動く座標をプレイヤーの位置に適用
            playerRigidBody.position = playerPosition;
            */
        }
    }

    //プレイヤーの攻撃に関する処理
    void PlayerShot()
    {
        //左マウスを押している間orオート射撃がオンの際に射撃関数を実行する。
        if (Input.GetKey(KeyCode.Mouse0) || isAutoShot == true)
        {
            //前回の発射からの経過時間(reloadTime)がリロードに掛かる時間(bulletFireRate)より長ければ発射処理を行う。

            if (reloadTimer >= bulletFireRate && magazineGage >= magagineConsumption)
            {

                if (weaponState == WeaponState.doubleMachineGun)
                {
                    //リロードタイマーを0にする
                    reloadTimer = 0.0f;

                    magazineGage -= magagineConsumption; 

                    MuzzleTransform = Muzzle.transform;

                    //左の銃弾の生成
                    doubleMGL_bulletPos = new Vector3(doubleMG_L_transform.position.x + 2.5f,
                                                      doubleMG_L_transform.position.y,
                                                      doubleMG_L_transform.position.z + 2.0f);
                    bullet01 = Instantiate(bulletPrefab, doubleMGL_bulletPos, MuzzleTransform.rotation) as GameObject;

                    //右の銃弾の生成
                    doubleMGR_bulletPos = new Vector3(doubleMG_R_transform.position.x - 2.5f,
                                                      doubleMG_R_transform.position.y,
                                                      doubleMG_R_transform.position.z + 2.0f);
                    bullet02 = Instantiate(bulletPrefab, doubleMGR_bulletPos, MuzzleTransform.rotation) as GameObject;

                    //銃弾に発射処理を行う。
                    shootForce = Muzzle.transform.forward * bulletSpeed;
                    bullet01.GetComponent<Rigidbody>().AddForce(shootForce);

                    //銃弾に発射処理を行う。
                    shootForce = Muzzle.transform.forward * bulletSpeed;
                    bullet02.GetComponent<Rigidbody>().AddForce(shootForce);

                    playerAnimator.SetBool("Doubleshot_shot", true);
                }

            }

        }

    }

    //リロード時間計算のためのタイマー
    void Timer()
    {
        reloadTimer += Time.deltaTime;
        rollRecastTimer += Time.deltaTime;
        moveMotionTimer += Time.deltaTime;
    }

    //緊急回避回避(ステップ)の処理
    void Roll ()
    {
        //Eキーを押したときに動作
        if (Input.GetKey(KeyCode.Space)){

            //緊急回避がチャージされていれば処理を実行
            if(boostGage >= boostConsumption && isRolling == false)
            {
                Debug.Log("緊急回避処理を開始");

                //緊急回避中かのフラグをオンに
                isRolling = true;

                boostGage = boostGage - boostConsumption;

                //移動方向がマイナスであればロール距離もマイナスに変換
                if (moveX < 0)
                {

                    currentRollDirection = 1;
                }
                if (moveX >= 0)
                {
                    currentRollDirection = 2;
                }

                if (!playerAnimator.IsInTransition(0))
                {
                    //移動後のXの値が許容範囲を超える際
                    if (Mathf.Abs(playerRigidBody.transform.position.x) + Mathf.Abs(rollDistance) >= limitX){

                        switch (currentRollDirection)
                        {
                            case 1:
                                Debug.Log("左にステップ");
                                finallyRollDistance = -limitX - playerRigidBody.transform.position.x;
                                Debug.Log("現在地は" + playerRigidBody.transform.position.x + "ステップ距離は" + finallyRollDistance);
                                playerAnimator.SetBool("rolltoLeft", true);
                                break;
                            case 2:
                                Debug.Log("右にステップ");
                                //右ステップ
                                finallyRollDistance = limitX - playerRigidBody.transform.position.x;
                                Debug.Log("現在地は" + playerRigidBody.transform.position.x + "ステップ距離は" + finallyRollDistance);
                                playerAnimator.SetBool("rolltoRight", true);
                                break;
                        }
                    }
                    else
                    {
                        switch (currentRollDirection)
                        {
                            case 1:
                                Debug.Log("左にステップ");
                                finallyRollDistance = -Mathf.Abs(rollDistance);
                                Debug.Log("現在地は" + playerRigidBody.transform.position.x + "ステップ距離は" + finallyRollDistance);
                                playerAnimator.SetBool("rolltoLeft", true);
                                break;
                            case 2:
                                Debug.Log("右にステップ");
                                //右ステップ
                                finallyRollDistance = Mathf.Abs(rollDistance);
                                Debug.Log("現在地は" + playerRigidBody.transform.position.x + "ステップ距離は" + finallyRollDistance);
                                playerAnimator.SetBool("rolltoRight", true);
                                break;
                        }
                    }
                }


                //Dotweenで緊急回避の移動処理を実行
                playerRigidBody.transform.DOLocalMoveX(playerRigidBody.transform.position.x + finallyRollDistance, 0.3f).SetEase(Ease.OutCirc);

                StartCoroutine("RollReset");
            }


        }
    }

    private IEnumerator RollReset()
    {
        //緊急回避処理完了後にフラグをオフ
        yield return new WaitForSeconds(0.1f);
        isRolling = false;
        switch (currentRollDirection)
        {
            case 1:
                playerAnimator.SetBool("rolltoLeft", false);
                break;
            case 2:
                playerAnimator.SetBool("rolltoRight", false);
                break;
        }

    }


}
