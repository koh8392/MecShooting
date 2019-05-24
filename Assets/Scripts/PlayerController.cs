using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;
using GameParameters;

namespace PlayerControllerScript
{

    public class PlayerController : DelayScript
    {

        private GameState currentGameState;

        public float playerDefaultHP;                  //プレイヤーのデフォルトのHP
        public float playerCurrentHP;                  //プレイヤーの現在のHP

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
        public bool isStateChanged;
        public float moveMotionTimer;
        [SerializeField] private float moveRecastTime;
        public bool isMoveAnimationChanged;

        //パーティクルに関する変数
        [SerializeField]
        private List<GameObject> particleStarterL_List;
        [SerializeField]
        private List<GameObject> particleStarterR_List;


        //プレイヤーの射撃全体に関わる変数

        private GameObject muzzle;
        private Transform muzzleTransform;            //銃口のTransform

        public bool isAutoShot;                        //オートで射撃を行うか

        //データから代入する武器情報の格納先


        //メインウェポンに関する変数
        private Weapondata weaponData;                 //武器データの格納先(ScriptableObject)
        private WeaponStatus weaponStatus;             //武器の種類(ScriptableObject内のリスト)
        private WeaponType weaponType;                 //武器の種類

        private GameObject bulletPrefab;               //弾丸のプレファブ
        private float reloadTimer;                     //リロード用のタイマー

        private GameObject muzzleObject_L;             //発射点のオブジェクト(左)
        private GameObject muzzleObject_R;             //発射点のオブジェクト(右)
        private Transform muzzleObject_L_transform;    //発射点オブジェクトのトランスフォーム(右)
        private Transform muzzleObject_R_transform;    //発射点オブジェクトトランスフォーム(左)

        public float RemainMagazine;                //弾倉の残量
        private float magazineConsumption;             //弾倉の発射時の消費量
        private float bulletFireRate;                  //リロード間隔

        //サブウェポンに関する変数

        private SubWeapondata subweaponData;                 //武器データの格納先(ScriptableObject)
        private SubWeaponStatus subweaponStatus;             //武器の種類(ScriptableObject内のリスト)
        private SubWeaponType subweaponType;              //武器の種類

        private GameObject subBulletPrefab;               //弾丸のプレファブ
        private float reloadSubWeaponTimer;               //リロード用のタイマー

        private GameObject subMuzzleObject_L;             //発射点のオブジェクト(左)
        private GameObject subMuzzleObject_R;             //発射点のオブジェクト(右)
        private Transform subMuzzleObject_L_transform;    //発射点オブジェクトのトランスフォーム(右)
        private Transform subMuzzleObject_R_transform;    //発射点オブジェクトトランスフォーム(左)
        public List<GameObject> subWeaponMuzzleObjects;  //見た目上のオブジェクト
        public List<GameObject> subWeaponVisualObjects;  //見た目上のオブジェクト
        public GameObject[] lockOnObjects;
        public List<GameObject> lockOnUIs;

        public float subRemainMagazine;                     //弾倉の残量
        private float subMagazineConsumption;             //弾倉の発射時の消費量
        private float subBulletFireRate;                  //リロード間隔

        [SerializeField] private GameObject PlayerUI;

        private bool isTouched;

        private RectTransform LockOnUIRectTransform;

        // Use this for initialization
        void Start()
        {
            //ゲームステートの変更を感知するため、ゲームステートの初期値を取得
            currentGameState = GameManager.gameState;

            /*プレイヤーのHPに関する処理*/

            playerCurrentHP = playerDefaultHP;

            /*プレイヤーのHPに関する処理ここまで*/

            /*アニメーションに関する処理*/
            playerModel = GameObject.Find("mechmodel");
            playerAnimator = playerModel.GetComponent<Animator>();

            //アニメーション変更中のフラグをオフ
            isStateChanged = false;
            //移動時のアニメーション変更中のフラグをオフ
            isMoveAnimationChanged = false;

            /*アニメーションに関する処理ここまで*/


            /*プレイヤー移動に関する初期処理*/

            //プレイヤーのrigidbodyを取得
            playerRigidBody = GetComponent<Rigidbody>();

            //インスペクターで設定した移動限界の値を逆方向の変数にも代入
            negativeLimitX = -limitX;

            /*プレイヤー移動に関する初期処理ここまで*/


            //武器データの読み込み。weaponStatus紐付け部分はSetWeaponStatus内にモジュール化
            //武器の種類を設定
            weaponType = WeaponType.doubleMachineGun;

            SetWeaponStatus(WeaponType.doubleMachineGun);

            SetSubWeaponStatus(SubWeaponType.cannon);


            /*プレイヤーの緊急回避に関する初期処理*/

            //緊急回避中のフラグの初期値はオフ
            isRolling = false;

            //インスペクターで設定した緊急回避距離を逆方向の変数にも代入
            negativeRollDistance = -rollDistance;

            //緊急回避の初期方向を設定 
            currentRollDirection = 0;

            /*プレイヤーの緊急回避に関する初期処理ここまで*/

            //ブーストゲージと弾倉ゲージの初期値を設定
            boostGage = 0.0f;
            RemainMagazine = 0.0f;
            subRemainMagazine = 0.0f;

            //PlayerUI.GetComponent<EnemyUIController>().ActivateUI(gameObject);

            LockOnUIRectTransform = GameObject.Find("LockOnUI").GetComponent<RectTransform>();
        }


        private void SetWeaponStatus(WeaponType weaponType)
        {

            /*射撃に関する処理*/

            //全体の射撃方向を管理するオブジェクトを取得
            muzzle = transform.Find("PlayerMuzzle").gameObject;
            muzzleTransform = muzzle.GetComponent<Transform>();

            /*武器データの読み込みに関する変数*/

            //武器データのScriptableObject全体を読み込み
            weaponData = Resources.Load<Weapondata>("MainWeapondata");

            weaponStatus = weaponData.weaponStatusList[(int)weaponType];

            Debug.Log("選択中の武器は" + weaponStatus.weaponName);

            /*武器データの読み込みに関する変数ここまで*/

            /*武器設定の読み込み処理*/

            //射出点の数を取得
            int numOfMuzzleObject = weaponStatus.listofMuzzleObjectName.Count;

            //射出点のオブジェクトのモデルを取得
            string muzzleObjectName_L = "mechmodel/" + weaponStatus.listofMuzzleObjectName[0];
            muzzleObject_L = transform.Find(muzzleObjectName_L).gameObject;

            //射出点のオブジェクトのトランスフォームを取得
            muzzleObject_L_transform = muzzleObject_L.GetComponent<Transform>();
            muzzleObject_L.SetActive(true);

            if (numOfMuzzleObject == 2)
            {
                string muzzleObjectName_R = "mechmodel/" + weaponStatus.listofMuzzleObjectName[1];
                muzzleObject_R = transform.Find(muzzleObjectName_R).gameObject;
                muzzleObject_R_transform = muzzleObject_R.GetComponent<Transform>();
                muzzleObject_R.SetActive(true);

            }

            //弾倉消費量を取得
            magazineConsumption = weaponStatus.bulletConsumption;

            /*武器設定の読み込み処理ここまで*/



            /*弾薬の読み込み処理*/

            //弾丸のプレハブをロードする処理
            //weaponDataから読み込む弾丸のパスを文字列で生成
            string currentBulletName = "Prefabs/General/" + weaponStatus.bulletPrefabName;

            //弾丸のプレハブを読み込み
            bulletPrefab = (GameObject)Resources.Load(currentBulletName);

            //弾丸のパラメータを事前取得
            bulletFireRate = weaponStatus.bulletFireRate;

            //射撃レートを秒数に変換(レート/60秒/FixedUpdate60FPS)
            bulletFireRate = bulletFireRate / 3600;
            reloadTimer = 0;

            /*弾薬の読み込み処理ここまで*/

            playerAnimator.SetBool(weaponStatus.idleMotionFlag, true);

            /*射撃に関する処理*ここまで*/
        }

        //サブウェポンのデータの読み込み
        private void SetSubWeaponStatus(SubWeaponType subweaponType)
        {

            /*射撃に関する処理*/

            /*武器データの読み込みに関する変数*/

            //武器データのScriptableObject全体を読み込み
            subweaponData = Resources.Load<SubWeapondata>("SubWeapondata");

            //武器データのScriptableObject内から現在の武器状態の物を読み込み。
            subweaponStatus = subweaponData.subWeaponStatusList[(int)subweaponType];

            Debug.Log("選択中の武器は" + subweaponStatus.weaponName);



            /*武器設定の読み込み処理*/

            //射出点の数を取得
            int numOfSubWeaponMuzzleObject = subweaponStatus.listofMuzzleObjectName.Count;

            //射出点のオブジェクトの名前が入ったリストを読み込み
            List<string> listofSubMuzzleObjectName = subweaponStatus.listofMuzzleObjectName;

            //射出点オブジェクトのリストを初期化
            subWeaponMuzzleObjects.Clear();
      

            foreach (string subMuzzleObjectName in listofSubMuzzleObjectName)
            {
                //射出点オブジェクトのパスを設定
                string subMuzzleObjectPass = "mechmodel/" + subMuzzleObjectName;
                //射出点オブジェクトを取得
                GameObject subMuzzleObject = transform.Find(subMuzzleObjectPass).gameObject;
                //射出点のオブジェクトを有効化
                subMuzzleObject.SetActive(true);

                //射出点オブジェクトをリストに格納
                subWeaponMuzzleObjects.Add(subMuzzleObject);
            }


            //表示オブジェクトの名前が入ったリストを読み込み
            List<string> listofVisualObjectName = subweaponStatus.listofVisualObjectName;

            //射出点オブジェクトのリストを初期化
            subWeaponVisualObjects.Clear();

            foreach (string visualObjectName in listofVisualObjectName)
            {
                
                //見た目を切り替えるオブジェクトのパスを設定。
                string visualObjectPass = "mechmodel/" + visualObjectName;
                //見た目を切り替えるオブジェクトを取得
                GameObject visualObject = transform.Find(visualObjectPass).gameObject;
                //見た目のオブジェクトを有効化する
                visualObject.SetActive(true);

                //リストに見た目のオブジェクトを格納しておく
                subWeaponVisualObjects.Add(visualObject);

            }


            //弾倉消費量を取得
            subMagazineConsumption = subweaponStatus.bulletConsumption;

            /*武器設定の読み込み処理ここまで*/



            /*弾薬の読み込み処理*/

            //弾丸のプレハブをロードする処理
            //weaponDataから読み込む弾丸のパスを文字列で生成
            string currentBulletName = "Prefabs/General/" + subweaponStatus.bulletPrefabName;

            //弾丸のプレハブを読み込み
            subBulletPrefab = (GameObject)Resources.Load(currentBulletName);

            //弾丸のパラメータを事前取得
            subBulletFireRate = subweaponStatus.bulletFireRate;

            //射撃レートを秒数に変換(レート/60秒/FixedUpdate60FPS)
            subBulletFireRate = subBulletFireRate / 3600;
            reloadSubWeaponTimer = 0;

            /*弾薬の読み込み処理ここまで*/

            /*射撃に関する処理*ここまで*/
        }


        // Update is called once per frame
        void FixedUpdate()
        {
            //ゲームステートに合わせて実行する処理を変更
            if (GameManager.gameState == GameState.boosted)
            {
                CheckState();      //ゲームステートの変更確認
            }

            if (GameManager.gameState == GameState.play)
            {
                CheckState();      //ゲームステートの変更確認
                Timer();             //リキャスト時間の加算
                ChargeGage();        //時間で回復する数値の管理
                Roll();              //緊急回避
                PlayerMove();        //移動
                PlayerShot();        //射撃
                PlayerSubShot();
            }
        }

        private void LateUpdate()
        {
            //フレームの最後でプレイヤーの位置がプレイ範囲外に飛んでいたら戻す
            if (GameManager.gameState == GameState.play)
            {
                ModifyPlayerPosition();
            }
        }

        private void CheckState()
        {
            //保存しているゲームステートとGameManagerのゲームステートが異なる場合。
            if (GameManager.gameState != currentGameState && isStateChanged == false)
            {
                //ゲームマネージャーの値に応じて処理を変更
                switch (GameManager.gameState)
                {
                    case GameState.play:
                        playerAnimator.SetBool(weaponStatus.idleMotionFlag, true);
                        break;

                    case GameState.boosted:
                        break;

                }

                //現在のゲームステイトを保存し、再処理されないようにフラグをオン
                currentGameState = GameManager.gameState;
                isStateChanged = true;
            }

            if (GameManager.gameState == currentGameState)
            {
                //ゲームステイトに変化がなければ再処理防止フラグをオフ
                if (isStateChanged == true)
                {
                    isStateChanged = false;
                }
            }

        }

        //時間で回復するパラメーターの回復処理
        private void ChargeGage()
        {
            if (GameManager.masterTime >= 8.0f)
            {
                if (boostGage < 100)
                {
                    boostGage += 1;
                }

                if (RemainMagazine < 100)
                {
                    RemainMagazine += 1;
                }

                if (subRemainMagazine < 100)
                {
                    subRemainMagazine += 1;
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

            if (isRolling == false)
            {


                //現在の方向キーの入力の値を取得
                moveX = Input.GetAxis("Horizontal");
                moveY = Input.GetAxis("Vertical");

                //アニメーションの変更処理
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

                //実際の移動処理
                gameObject.transform.Translate((moveX * playerMoveSpeedX), (moveY * playerMoveSpeedY), 0, Space.Self);

            }
        }

        //プレイヤーの攻撃に関する処理
        void PlayerShot()
        {
            //左マウスを押している間orオート射撃がオンの際に射撃関数を実行する。
            if (Input.GetKey(KeyCode.Mouse0) || isAutoShot == true)
            {
                //前回の発射からの経過時間(reloadTime)がリロードに掛かる時間(bulletFireRate)より長ければ発射処理を行う。

                if (reloadTimer >= bulletFireRate && RemainMagazine >= magazineConsumption)
                {



                    if (weaponType == WeaponType.doubleMachineGun)
                    {
                        shotDMG();
                    }

                    if (weaponType == WeaponType.longRifle)
                    {
                        shotLongRifle();

                    }
                }

            }

        }


        //ダブルマシンガンの攻撃処理
        private void shotDMG()
        {
            //リロードタイマーを0にする
            reloadTimer = 0.0f;

            //射出前に弾倉ゲージを減らす
            RemainMagazine -= magazineConsumption;

            //左の銃弾の生成

            //生成位置は射出点のオブジェクトにオフセット分ずらした座標
            Vector3 muzzleObjectL_bulletPos = new Vector3(muzzleObject_L_transform.position.x + weaponStatus.muzzleOffset.x,
                                                  muzzleObject_L_transform.position.y + weaponStatus.muzzleOffset.y,
                                                  muzzleObject_L_transform.position.z + weaponStatus.muzzleOffset.z);
            //読み込んでいた弾丸のプレファブを生成
            GameObject bullet01 = Instantiate(bulletPrefab, muzzleObjectL_bulletPos, muzzleTransform.rotation) as GameObject;



            //右の銃弾の生成処理
            //生成位置は射出点のオブジェクトにオフセット分ずらした座標
            Vector3 muzzleObjectR_bulletPos = new Vector3(muzzleObject_R_transform.position.x - weaponStatus.muzzleOffset.x,
                                                  muzzleObject_R_transform.position.y + weaponStatus.muzzleOffset.y,
                                                  muzzleObject_R_transform.position.z + weaponStatus.muzzleOffset.z);
            //読み込んでいた弾丸のプレファブを生成
            GameObject bullet02 = Instantiate(bulletPrefab, muzzleObjectR_bulletPos, muzzleTransform.rotation) as GameObject;

            //弾丸に威力などのデータを渡す
            setBulletStatusToInstance(bullet01);
            setBulletStatusToInstance(bullet02);

            //銃弾に力を加えて発射。
            Vector3 shootForce = muzzle.transform.forward * weaponStatus.bulletSpeed;
            bullet01.GetComponent<Rigidbody>().AddForce(shootForce);

            //銃弾に力を加えて発射。
            shootForce = muzzle.transform.forward * weaponStatus.bulletSpeed;
            bullet02.GetComponent<Rigidbody>().AddForce(shootForce);

            //発射アニメーションを実行
            playerAnimator.SetTrigger("Doubleshot_shot");
        }

        private void shotLongRifle()
        {
            //リロードタイマーを0にする
            reloadTimer = 0.0f;

            //射出前に弾倉ゲージを減らす
            RemainMagazine -= magazineConsumption;

            //左の銃弾の生成

            //生成位置は射出点のオブジェクトにオフセット分ずらした座標
            Vector3 muzzleObjectL_bulletPos = new Vector3(muzzleObject_L_transform.position.x + weaponStatus.muzzleOffset.x,
                                                  muzzleObject_L_transform.position.y + weaponStatus.muzzleOffset.y,
                                                  muzzleObject_L_transform.position.z + weaponStatus.muzzleOffset.z);
            //読み込んでいた弾丸のプレファブを生成
            GameObject bullet01 = Instantiate(bulletPrefab, muzzleObjectL_bulletPos, muzzleTransform.rotation) as GameObject;

            setBulletStatusToInstance(bullet01);

            //銃弾に力を加えて発射。
            Vector3 shootForce = muzzle.transform.forward * weaponStatus.bulletSpeed;
            bullet01.GetComponent<Rigidbody>().AddForce(shootForce);

            //発射アニメーションを実行
            playerAnimator.SetTrigger("Rifle_shot");
        }

        //弾丸に情報を設定する処理。引数：弾丸のGameobject
        private void setBulletStatusToInstance(GameObject selectedBullet)
        {
            //対象のbulletのbulletcontrollerを取得
            BulletController selectedBulletController = selectedBullet.GetComponent<BulletController>();

            //各値をweaponStatusに格納された値から渡す。
            selectedBulletController.currentBullet = weaponStatus.currentBullet;
            selectedBulletController.bulletPower = weaponStatus.bulletPower;
            selectedBulletController.bulletDeathTime = weaponStatus.bulletPower;
            selectedBulletController.hasBulletEffect = weaponStatus.hasBulletEffect;
        }


        //プレイヤーの攻撃に関する処理
        void PlayerSubShot()
        {
            //前回の発射からの経過時間(reloadTime)がリロードに掛かる時間(bulletFireRate)より長ければ発射処理を行う。
            if (reloadSubWeaponTimer >= subBulletFireRate && subRemainMagazine >= subMagazineConsumption)
            {

                    if (subweaponType == SubWeaponType.cannon)
                    {
                        shotCannon();
                    }


                    if (subweaponType == SubWeaponType.missile)
                    {
                        shotMissile();
                    }

            }

        }

        void shotCannon()
        {
            //マウスの右クリック(サブウェポンボタン)を押したとき
            if (Input.GetMouseButtonDown(1))
            { }
            //マウスの右クリック(サブウェポンボタン)を離したとき
            if (Input.GetMouseButtonUp(1))
            {
                //発射アニメーションを実行
                playerAnimator.SetTrigger("Cannon_shot");

                //リロードタイマーを0にする
                reloadSubWeaponTimer = 0.0f;

                //射出前に弾倉ゲージを減らす
                subRemainMagazine -= subMagazineConsumption;

                StartCoroutine(Delay(0.2f, () =>
                {
                    //左の銃弾の生成
                    foreach (GameObject subMuzzleObject in subWeaponMuzzleObjects)
                    {
                        for (int bulletCount = 0; bulletCount <= subweaponStatus.numOfBulletsPerMuzzle; ++bulletCount) { }
                        Vector3 subMuzzleObjectPosition = subMuzzleObject.transform.position;

                        //生成位置は射出点のオブジェクトにオフセット分ずらした座標
                        Vector3 subMuzzleObject_bulletPos = new Vector3(subMuzzleObjectPosition.x + subweaponStatus.muzzleOffset.x + subweaponStatus.bulletOffset.x,
                                                                        subMuzzleObjectPosition.y + subweaponStatus.muzzleOffset.y + subweaponStatus.bulletOffset.y,
                                                                        subMuzzleObjectPosition.z + subweaponStatus.muzzleOffset.z + subweaponStatus.bulletOffset.z);

                        //読み込んでいた弾丸のプレファブを生成
                        GameObject bullet = Instantiate(subBulletPrefab, subMuzzleObject_bulletPos, muzzleTransform.rotation) as GameObject;

                        setSubBulletStatusToInstance(bullet);

                        //銃弾に力を加えて発射。
                        bullet.GetComponent<Rigidbody>().DOMoveZ(transform.position.z + 100, 2).SetEase(Ease.OutQuad);
                    }


                }));
            }
        }

        void shotMissile()
        {
            if (Input.GetMouseButtonDown(1))
            {
                bool keyDownFlag = false;

                //タグ検索を行って、一番近い敵オブジェクトを取得する(近い方から順に配列に格納)
                lockOnObjects = GameObject.FindGameObjectsWithTag("Enemy");
                GameObject UICanvas = GameObject.Find("Canvas");

                lockOnUIs = new List<GameObject>();

                //キーダウン時の処理が未処理状態なら実行
                if (keyDownFlag == false)
                {
                    //ロックオンしたオブジェクトに対してUiを表示する。
                    foreach (GameObject lockonObject in lockOnObjects)
                    {
                        //Vector3 enemyPosition = lockonObject.transform.position;

                        //取得したターゲットの座標をUI座標に変換
                        RectTransform targetRectTransform = LockOnUIRectTransform;

                        //ロックオンUIを読み込み
                        GameObject targetUIprefab = (GameObject)Resources.Load("Prefabs/General/LockOnUI");

                        //取得して置いた敵の座標にUIを表示する。
                        GameObject targetUI = Instantiate(targetUIprefab, targetRectTransform);
                        //生成したUIをcanvasの子にする。
                        targetUI.transform.SetParent(UICanvas.transform, false);
                        //生成したUIをリストに格納して置く(あとで削除する時のため)
                        lockOnUIs.Add(targetUI);

                        //UIの位置を敵に合わせる。
                        targetRectTransform.anchoredPosition = RectTransformUtility.WorldToScreenPoint(Camera.main, lockonObject.transform.position);

                        //残りの追従はUi側のスクリプトで行う。
                        //取得したlockonObjectを渡して引数で管理。

                    }

                    //キーダウン時の処理を実行済みに
                    keyDownFlag = true;
                }
            }

            //マウスの右クリック(サブウェポンボタン)を離したとき
            if (Input.GetMouseButtonUp(1))
            {
                //弾の生成を同時発射数の分だけforで回す。


                foreach (GameObject subMuzzleObject in subWeaponMuzzleObjects)
                {
                    for (int bulletCount = 0; bulletCount <= subweaponStatus.numOfBulletsPerMuzzle; ++bulletCount) { }
                    Vector3 subMuzzleObjectPosition = subMuzzleObject.transform.position;

                    //生成位置は射出点のオブジェクトにオフセット分ずらした座標
                    Vector3 subMuzzleObject_bulletPos = new Vector3(subMuzzleObjectPosition.x + subweaponStatus.muzzleOffset.x + subweaponStatus.bulletOffset.x,
                                                                    subMuzzleObjectPosition.y + subweaponStatus.muzzleOffset.y + subweaponStatus.bulletOffset.y,
                                                                    subMuzzleObjectPosition.z + subweaponStatus.muzzleOffset.z + subweaponStatus.bulletOffset.z);

                    Debug.Log(subBulletPrefab.name);

                    //読み込んでいた弾丸のプレファブを生成
                    GameObject bullet = Instantiate(subBulletPrefab, subMuzzleObject_bulletPos, muzzleTransform.rotation) as GameObject;

                    setSubBulletStatusToInstance(bullet);

                    BulletController selectedBulletController = bullet.GetComponent<BulletController>();

                    GameObject targetEnemy = lockOnObjects[0];

                    selectedBulletController.SetTarget(targetEnemy);


                    //銃弾に力を加えて発射。
                    bullet.GetComponent<Rigidbody>().DOMoveZ(transform.position.z + 100, 2).SetEase(Ease.OutQuad);


                }

                //重複表示されないように、発射時にロックオン表示を全て破棄する。
                foreach(GameObject lockOnUI in lockOnUIs)
                {
                    //リストに入っているUIを破棄
                    Destroy(lockOnUI);
                }

                //全てロックオン表示を消したらリストを初期化
                lockOnUIs.Clear();


                //同時にターゲットのgameObjectと弾のデータを渡す
                //初期無誘導射出のためのaddforceを行う
                //ロックオンUIを削除する。


            

            }
        }


        //弾丸に情報を設定する処理。引数：弾丸のGameobject
        private void setSubBulletStatusToInstance(GameObject selectedBullet)
        {

            Debug.Log(subweaponStatus.weaponName);

            Debug.Log(subweaponStatus.currentBullet);

            //対象のbulletのbulletcontrollerを取得
            BulletController selectedBulletController = selectedBullet.GetComponent<BulletController>();

            //各値をweaponStatusに格納された値から渡す。
            selectedBulletController.currentBullet = subweaponStatus.currentBullet;
            selectedBulletController.bulletPower = subweaponStatus.bulletPower;
            selectedBulletController.bulletDeathTime = subweaponStatus.bulletDeathTime;
            selectedBulletController.hasBulletEffect = subweaponStatus.hasBulletEffect;
        }

        //各クール計算のためのタイマー
        void Timer()
        {
            //毎フレームdeltaTimeを加算
            reloadTimer += Time.deltaTime;
            reloadSubWeaponTimer += Time.deltaTime;
            rollRecastTimer += Time.deltaTime;
            moveMotionTimer += Time.deltaTime;
        }

        //緊急回避回避(ステップ)の処理
        void Roll()
        {
            //Eキーを押したときに動作
            if (Input.GetKey(KeyCode.Space))
            {

                //緊急回避がチャージされていれば処理を実行
                if (boostGage >= boostConsumption && isRolling == false)
                {
                    //Debug.Log("緊急回避処理を開始");

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

                    //if (!playerAnimator.IsInTransition(0))
                    //{
                    //移動後のXの値が許容範囲を超えない際
                    if ((playerRigidBody.transform.position.x + rollDistance) <= limitX && playerRigidBody.transform.position.x + negativeRollDistance >= negativeLimitX)
                    {

                        switch (currentRollDirection)
                        {
                            case 1:
                                //Debug.Log("左にステップ");
                                //移動可能範囲を超えないのでそのまま代入
                                finallyRollDistance = negativeRollDistance;
                                //Debug.Log("現在地は" + playerRigidBody.transform.position.x + "ステップ距離は" + finallyRollDistance);
                                playerAnimator.SetTrigger("Roll_toLeft");
                                //リスト内に格納されたオブジェクトそれぞれにパーティクル動作開始メソッドを実行
                                foreach (GameObject particleStarterChildren in particleStarterR_List)
                                {
                                    particleStarterChildren.GetComponent<ParticleStarter>().StartParticle();
                                }
                                break;
                            case 2:
                                //Debug.Log("右にステップ");
                                //移動可能範囲を超えないのでそのまま代入
                                finallyRollDistance = rollDistance;
                                //Debug.Log("現在地は" + playerRigidBody.transform.position.x + "ステップ距離は" + finallyRollDistance);
                                playerAnimator.SetTrigger("Roll_toRight");
                                foreach (GameObject particleStarterChildren in particleStarterL_List)
                                {
                                    particleStarterChildren.GetComponent<ParticleStarter>().StartParticle();
                                }
                                break;
                        }
                    }
                    //右方向に緊急回避の距離が超過する場合
                    if (playerRigidBody.transform.position.x > 0 && playerRigidBody.transform.position.x + rollDistance > limitX)
                    {
                        switch (currentRollDirection)
                        {
                            case 1:
                                //Debug.Log("左にステップ");
                                finallyRollDistance = negativeRollDistance;
                                //Debug.Log("現在地は" + playerRigidBody.transform.position.x + "ステップ距離は" + finallyRollDistance);
                                playerAnimator.SetTrigger("Roll_toLeft");
                                foreach (GameObject particleStarterChildren in particleStarterR_List)
                                {
                                    particleStarterChildren.GetComponent<ParticleStarter>().StartParticle();
                                }
                                break;
                            case 2:
                                //Debug.Log("右に超過");
                                //ステップ距離が超過しないように、limitxとの残りの距離分だけ移動させる
                                finallyRollDistance = Mathf.Clamp(limitX - playerRigidBody.transform.position.x, 0, limitX);
                                //Debug.Log("現在地は" + playerRigidBody.transform.position.x + "ステップ距離は" + finallyRollDistance);
                                playerAnimator.SetTrigger("Roll_toRight");
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
                                //Debug.Log("左に超過");
                                finallyRollDistance = Mathf.Clamp(negativeLimitX - playerRigidBody.transform.position.x, negativeLimitX, 0);
                                //Debug.Log("現在地は" + playerRigidBody.transform.position.x + "ステップ距離は" + finallyRollDistance);
                                playerAnimator.SetTrigger("Roll_toLeft");
                                foreach (GameObject particleStarterChildren in particleStarterR_List)
                                {
                                    particleStarterChildren.GetComponent<ParticleStarter>().StartParticle();
                                }
                                break;
                            case 2:
                                //Debug.Log("右にステップ");
                                //ステップ距離が超過しないように、limitxとの残りの距離分だけ移動させる
                                finallyRollDistance = rollDistance;
                                //Debug.Log("現在地は" + playerRigidBody.transform.position.x + "ステップ距離は" + finallyRollDistance);
                                playerAnimator.SetTrigger("Roll_toRight");
                                foreach (GameObject particleStarterChildren in particleStarterL_List)
                                {
                                    particleStarterChildren.GetComponent<ParticleStarter>().StartParticle();
                                }
                                break;
                        }
                    }
                    //}


                    //Dotweenで緊急回避の移動処理を実行
                    playerRigidBody.transform.DOLocalMoveX(playerRigidBody.transform.position.x + finallyRollDistance, 0.3f).SetEase(Ease.OutCirc);

                    //緊急回避の終了処理を予約実行
                    StartCoroutine("RollReset");
                }


            }
        }

        //緊急回避の終了処理
        private IEnumerator RollReset()
        {
            float rollWaitTime = 0.1f;

            //緊急回避処理完了後にフラグをオフ
            yield return new WaitForSeconds(rollWaitTime);
            //緊急回避フラグをオフ
            isRolling = false;

        }

        //敵との接触時にプレイヤーのHPを減らす処理
        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.tag == "Enemy" && isTouched == false)
            {
                playerCurrentHP -= 20;
                isTouched = true;
                StartCoroutine(Delay(1.0f , () => {
                    isTouched = false;
                }));

            }
        }

        //武器を切り替える処理
        public void weaponChange(WeaponType switchingWeaponType)
        {
            //現在の装備の見た目をオフに
            muzzleObject_L.SetActive(false);
            muzzleObject_R.SetActive(false);
            playerAnimator.SetBool(weaponStatus.idleMotionFlag, false);



            /*武器データの読み込みに関する変数*/
            //武器の種類を設定
            switch (weaponType)
            {
                case WeaponType.longRifle:
                    weaponType = WeaponType.doubleMachineGun;
                    break;

                case WeaponType.doubleMachineGun:
                    weaponType = WeaponType.longRifle;
                    break;

            }

            SetWeaponStatus(weaponType);


        }


        //武装変更処理の前段階
        public void subWeaponChange(SubWeaponType switchingSubWeaponType)
        {
            //現在の装備の見た目をオフに
            muzzleObject_L.SetActive(false);
            muzzleObject_R.SetActive(false);

            //表示オブジェクトの名前が入ったリストを読み込み
            List<string> listofVisualObjectName = subweaponStatus.listofVisualObjectName;

            foreach (string visualObjectName in listofVisualObjectName)
            {
                //見た目を切り替えるオブジェクトのパスを設定。
                string visualObjectPass = "mechmodel/" + visualObjectName;
                //見た目を切り替えるオブジェクトを取得
                GameObject visualObject = transform.Find(visualObjectPass).gameObject;
                //見た目のオブジェクトを有効化する
                visualObject.SetActive(false);

                //配列に見た目のオブジェクトを格納しておく
                //subWeaponVisualObjects.Add(visualObject);

            }

            playerAnimator.SetBool(weaponStatus.idleMotionFlag, false);



            /*武器データの読み込みに関する変数*/
            //武器の種類を設定
            subweaponType = switchingSubWeaponType;

            SetSubWeaponStatus(subweaponType);


        }

        //自動射撃のオンオフを切り替えるセッター
        public void setAutoShot()
        {
            //オートモードが実行中でない場合
            if (isAutoShot == false)
            {
                isAutoShot = true;
            }
            //オートモードが実行中の場合
            else if (isAutoShot == true)
            {
                isAutoShot = false;
            }
        }

    }
}
