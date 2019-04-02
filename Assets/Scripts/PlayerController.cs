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
    private Rigidbody playerRigidBody;              //プレイヤーの物理判定
    Vector3 playerPosition;                         //プレイヤーの現在位置

    [SerializeField] private float limitX;           //プレイヤーの横方向の移動可能範囲
    private float negativeLimitX;                    //プレイヤーの左方向の移動可能範囲(自動で合わせる)
    [SerializeField] private float bottomLimitY;     //プレイヤーの下方向の移動可能範囲
    [SerializeField] private float upperLimitY;      //プレイヤーの上方向の移動可能範囲

    private float rollRecastTimer;                    //緊急回避のタイマー
    [SerializeField] private float rollRecastTime;    //緊急回避の間隔
    [SerializeField] private float rollDistance;      //緊急回避の想定距離
    private float negativeRollDistance;               //左方向の緊急回避の距離(計算用)
    private float finallyRollDistance;                //緊急回避の最終移動距離
    private bool isRolling;                           //緊急回避状態かどうか
    private int currentRollDirection;                 //緊急回避の方向。0で右方向、1で左方向

    public float boostGage; //ブーストゲージ
    [SerializeField] private float boostConsumption;

    //アニメーションに関する変数
    private GameObject playerModel;               //プレイヤーの3Dモデル
    private Animator playerAnimator;
    public bool isAnimationChanged;
    public float moveMotionTimer;
    [SerializeField] private float moveRecastTime;
    public bool isMoveAnimationChanged;

    //パーティクルに関する変数
    [SerializeField]
    private List<GameObject> particleStarterL_List;
    [SerializeField]
    private List<GameObject> particleStarterR_List;


    //プレイヤーの射撃に関する変数
    private GameObject bullet01;                   //生成する弾丸
    private GameObject bullet02;　　　　　　　　　 //ダブルショットの場合2発目の弾丸
    private GameObject bulletPrefab;               //弾丸のプレファブ
    private GameObject muzzle;
    private Transform  muzzleTransform;            //銃口のTransform
    private Vector3 shootForce;                    //射撃する際に加える力
    private float reloadTimer;                     //リロード用のタイマー
    public bool isAutoShot;                        //オートで射撃を行うか
    private WeaponState weaponState;               //武器の種類
    private SubWeaponState subWeaponState;         //サブウェポンの種類
    private GameObject muzzleObject_L;             //発射点のオブジェクト(左)
    private GameObject muzzleObject_R;             //発射点のオブジェクト(右)
    private Transform muzzleObject_L_transform;    //発射点オブジェクトのトランスフォーム(右)
    private Transform muzzleObject_R_transform;    //発射点オブジェクトトランスフォーム(左)
    private Vector3 muzzleObjectL_bulletPos;       //弾丸の生成位置
    private Vector3 muzzleObjectR_bulletPos;       //弾丸の生成位置
    private int bulletConsumption;                 //1発当たりのマガジン消費量 

    public float magazineGage;                     //弾倉の残量
    private float magazineConsumption;             //弾倉の発射時の消費量

    //データから代入する武器情報の格納先
    private Weapondata weaponData;
    private WeaponStatus weaponStatus;

    //弾丸の情報
    //private float bulletPower;                       //弾丸の威力
    //private bulletSpecies currentBullet;             //弾丸の属性
    private float bulletSpeed;                         //弾速
    //private float bulletDeathTime;                   //弾丸の消失時間
    private float bulletFireRate;                      //リロード間隔

    // Use this for initialization
    void Start() {
        //ゲームステートの変更を感知するため、ゲームステートの初期値を取得
        currentGameState = GameManager.gameState;


        /*プレイヤー移動に関する初期処理*/
        //プレイヤーのrigidbodyを取得
        playerRigidBody = GetComponent<Rigidbody>();
        isRolling = false;

        rollRecastTimer = rollRecastTime;

   　　

        negativeLimitX = -limitX;
        negativeRollDistance = -rollDistance;
        /*プレイヤー移動に関する初期処理ここまで*/

        //武器データのScriptableObject全体を読み込み
        weaponData = Resources.Load<Weapondata>("MainWeapondata");

        weaponStatus = weaponData.weaponStatusList[0];

        Debug.Log("選択中の武器は" + weaponStatus.weaponName);


        /*アニメーションに関する処理*/
        playerModel = GameObject.Find("mechmodel");
        playerAnimator = playerModel.GetComponent<Animator>();

        //緊急回避の初期方向を設定 
        currentRollDirection = 0;

        //アニメーション変更中のフラグをオフ
        isAnimationChanged = false;
        //移動時のアニメーション変更中のフラグをオフ
        isMoveAnimationChanged = false;

        //ロール時に使用するパーティクルを取得
        //thrusterParticleStarter_R = GameObject.Find("thrusterEffectR").GetComponent<ParticleStarter>();

        /*アニメーションに関する処理ここまで*/

        //射撃に関する初期処理
        //武器の種類を設定
        weaponState = WeaponState.doubleMachineGun;

        //弾丸のプレハブをロードする処理
        
        //weaponDataから読み込む弾丸のパスを文字列で生成
        string currentBulletName = "Prefabs/" + weaponStatus.bulletPrefabName;

        //弾丸のプレハブを読み込み
        bulletPrefab = (GameObject)Resources.Load(currentBulletName);

        //弾丸のパラメータを事前取得
        bulletFireRate = bulletPrefab.GetComponent<BulletController>().bulletFireRate;
        //射撃レートを秒数に変換(レート/60秒/FixedUpdate60FPS)
        bulletFireRate = bulletFireRate / 3600;
        bulletSpeed = bulletPrefab.GetComponent<BulletController>().bulletSpeed;
        reloadTimer = bulletFireRate;

        

        //ブーストゲージと弾倉ゲージの初期値を設定
        boostGage = 0.0f;
        magazineGage = 0.0f;



        //ダブルショット時の武器のオブジェクトの取得

        //射出点のオブジェクトのモデルを取得
        string muzzleObjectName_L = "mechmodel/" + weaponStatus.listofMuzzleObjectName[0];
        muzzleObject_L = transform.Find(muzzleObjectName_L).gameObject;

        //射出点のオブジェクトのトランスフォームを取得
        muzzleObject_L_transform = muzzleObject_L.GetComponent<Transform>();

        //以下同様
        string muzzleObjectName_R = "mechmodel/" + weaponStatus.listofMuzzleObjectName[1];
        muzzleObject_R = transform.Find(muzzleObjectName_R).gameObject;
        muzzleObject_R_transform = muzzleObject_R.GetComponent<Transform>();

        //全体の射撃方向を管理するオブジェクトを取得
        muzzle = transform.Find("PlayerMuzzle").gameObject;
        muzzleTransform = muzzle.GetComponent<Transform>();

        //呼び出された武器に応じて武器のエネルギー使用量を決定
        magazineConsumption = weaponStatus.bulletConsumption;


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

    private void LateUpdate()
    {
        if (GameManager.gameState == GameState.play)
        {
            ModifyPlayerPosition();
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
                playerAnimator.SetBool("DoubleShot_set", true);
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

    private void ModifyPlayerPosition()
    {

        //プレイヤーの位置を取得
        playerPosition = new Vector3(Mathf.Clamp(playerRigidBody.position.x, negativeLimitX, limitX), Mathf.Clamp(playerRigidBody.position.y, bottomLimitY, upperLimitY), 0);

        gameObject.transform.localPosition = playerPosition;
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
                    playerAnimator.SetBool("move_left", true);
                    //transform.rotation = Quaternion.Euler(0, 0, 15);
                }
                if (-0.1 < moveX && moveX <= 0.1)
                {
                    playerAnimator.SetBool("move_left", false);
                    playerAnimator.SetBool("move_right", false);
                    //transform.rotation = Quaternion.Euler(0, 0, 0);
                }
                if (0.1 <= moveX && moveX <= 1
                    && !playerAnimator.IsInTransition(0))
                {
                    playerAnimator.SetBool("move_right", true);
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

            if (reloadTimer >= bulletFireRate && magazineGage >= magazineConsumption)
            {

                if (weaponState == WeaponState.doubleMachineGun)
                {
                    shotDMG();
                }

            }

        }

    }


    //ダブルマシンガンの攻撃処理
    private void shotDMG()
    {
        //リロードタイマーを0にする
        reloadTimer = 0.0f;

        magazineGage -= magazineConsumption;

        //左の銃弾の生成
        
        //射出点の位置をvector3で生成
        muzzleObjectL_bulletPos = new Vector3(muzzleObject_L_transform.position.x + weaponStatus.muzzleOffset.x,
                                              muzzleObject_L_transform.position.y + weaponStatus.muzzleOffset.y,
                                              muzzleObject_L_transform.position.z + weaponStatus.muzzleOffset.z);

        bullet01 = Instantiate(bulletPrefab, muzzleObjectL_bulletPos, muzzleTransform.rotation) as GameObject;

        

        //右の銃弾の生成
        muzzleObjectR_bulletPos = new Vector3(muzzleObject_R_transform.position.x - weaponStatus.muzzleOffset.x,
                                              muzzleObject_R_transform.position.y + weaponStatus.muzzleOffset.y,
                                              muzzleObject_R_transform.position.z + weaponStatus.muzzleOffset.z);
        bullet02 = Instantiate(bulletPrefab, muzzleObjectR_bulletPos, muzzleTransform.rotation) as GameObject;



        //銃弾に発射処理を行う。
        shootForce = muzzle.transform.forward * bulletSpeed;
        bullet01.GetComponent<Rigidbody>().AddForce(shootForce);

        //銃弾に発射処理を行う。
        shootForce = muzzle.transform.forward * bulletSpeed;
        bullet02.GetComponent<Rigidbody>().AddForce(shootForce);

        playerAnimator.SetBool("Doubleshot_shot", true);

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
                    //移動後のXの値が許容範囲を超えない際
                    if ((playerRigidBody.transform.position.x + rollDistance) <= limitX && playerRigidBody.transform.position.x + negativeRollDistance >= negativeLimitX){

                        switch (currentRollDirection)
                        {
                            case 1:
                                Debug.Log("左にステップ");
                                //移動可能範囲を超えないのでそのまま代入
                                finallyRollDistance = negativeRollDistance;
                                Debug.Log("現在地は" + playerRigidBody.transform.position.x + "ステップ距離は" + finallyRollDistance);
                                playerAnimator.SetBool("rolltoLeft", true);
                                //リスト内に格納されたオブジェクトそれぞれにパーティクル動作開始メソッドを実行
                                foreach (GameObject particleStarterChildren in particleStarterR_List)
                                {
                                    particleStarterChildren.GetComponent<ParticleStarter>().StartParticle();
                                }
                                break;
                            case 2:
                                Debug.Log("右にステップ");
                                //移動可能範囲を超えないのでそのまま代入
                                finallyRollDistance = rollDistance;
                                Debug.Log("現在地は" + playerRigidBody.transform.position.x + "ステップ距離は" + finallyRollDistance);
                                playerAnimator.SetBool("rolltoRight", true);
                                foreach (GameObject particleStarterChildren in particleStarterL_List)
                                {
                                    particleStarterChildren.GetComponent<ParticleStarter>().StartParticle();
                                }
                                break;
                        }
                    }
                    //右方向に緊急回避の距離が超過する場合
                    if(playerRigidBody.transform.position.x > 0 && playerRigidBody.transform.position.x + rollDistance > limitX)
                    {
                        switch (currentRollDirection)
                        {
                            case 1:
                                Debug.Log("左にステップ");
                                finallyRollDistance = negativeRollDistance;
                                Debug.Log("現在地は" + playerRigidBody.transform.position.x + "ステップ距離は" + finallyRollDistance);
                                playerAnimator.SetBool("rolltoLeft", true);
                                foreach (GameObject particleStarterChildren in particleStarterR_List)
                                {
                                    particleStarterChildren.GetComponent<ParticleStarter>().StartParticle();
                                }
                                break;
                            case 2:
                                Debug.Log("右に超過");
                                //ステップ距離が超過しないように、limitxとの残りの距離分だけ移動させる
                                finallyRollDistance = Mathf.Clamp(limitX - playerRigidBody.transform.position.x, 0, limitX);
                                Debug.Log("現在地は" + playerRigidBody.transform.position.x + "ステップ距離は" + finallyRollDistance);
                                playerAnimator.SetBool("rolltoRight", true);
                                foreach (GameObject particleStarterChildren in particleStarterL_List)
                                {
                                    particleStarterChildren.GetComponent<ParticleStarter>().StartParticle();
                                }
                                break;
                        }
                    }

                    //左方向に緊急回避の距離が超過する場合
                    if (playerRigidBody.transform.position.x < 0 && playerRigidBody.transform.position.x + negativeRollDistance < negativeLimitX)
                    {
                        switch (currentRollDirection)
                        {
                            case 1:
                                Debug.Log("左に超過");
                                finallyRollDistance = Mathf.Clamp(negativeLimitX - playerRigidBody.transform.position.x, negativeLimitX , 0);
                                Debug.Log("現在地は" + playerRigidBody.transform.position.x + "ステップ距離は" + finallyRollDistance);
                                playerAnimator.SetBool("rolltoLeft", true);
                                foreach (GameObject particleStarterChildren in particleStarterR_List)
                                {
                                    particleStarterChildren.GetComponent<ParticleStarter>().StartParticle();
                                }
                                break;
                            case 2:
                                Debug.Log("右にステップ");
                                //ステップ距離が超過しないように、limitxとの残りの距離分だけ移動させる
                                finallyRollDistance = rollDistance;
                                Debug.Log("現在地は" + playerRigidBody.transform.position.x + "ステップ距離は" + finallyRollDistance);
                                playerAnimator.SetBool("rolltoRight", true);
                                foreach (GameObject particleStarterChildren in particleStarterL_List)
                                {
                                    particleStarterChildren.GetComponent<ParticleStarter>().StartParticle();
                                }
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
