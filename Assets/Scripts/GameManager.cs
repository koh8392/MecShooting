using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using GameParameters;
using UnityEngine.Rendering.PostProcessing;

public class GameManager : MonoBehaviour {

    //GameManagerではゲームステートの変化や時間の経過など、全体の状態の変化によって起こる処理を管理する。
    
    
    
    /*全体の処理の基準になる変数*/

    //実際のゲームステート
    public static GameState gameState;
    //全体時間管理のタイマー
    public static float masterTime;

    /**********ここまで*********/


    //基本的なオブジェクト、コンポーネント
    private UIcontroller uIController;           //UIの制御コンポーネント
    private GameObject playerObject;             //プレイヤーのオブジェクト
    private GameObject gameAreaObject;           //ゲームエリアのオブジェクト

    //ゲームエリア→ゲームが進行する空間の基準となる空のオブジェクト。子にPlayerとCameraを持っていて、ゲームエリア内のローカルポジションで移動などを行う。

    //ブースターのパージに関する処理
    private GameObject booster;                    //ブースターのオブジェクト
    private GameObject mainCameraObject;           //メインカメラのオブジェクト
    private Camera mainCamera;                     //メインカメラのコンポーネント
    private PurgeBooster destroyMesh;               //ブースターのパージ処理
    [SerializeField] public float boostTime;       //開幕のブースト時間
    private GameObject purgeUI;                    //パージ予告表示のUI
    private CanvasGroup purgeUICanvas;             //パージUIのcanvas
    private float purgeUIalpha;                    //パージUIのalpha
    private bool isPurgeUIStarted;                 //パージ予告処理を開始したかのフラグ
    [SerializeField] public float purgeUIduration; //パージUIの点滅時間
    private float purgeUIEndDuration;              //パージUIの終了時間

    private PostProcessVolume mainPPSvolume;          //ポストプロセスの設定コンポーネント
    private ChromaticAberration chromaticAberration;  //PPSvolume内の色収差エフェクトのコンポーネント

    private Vector3 initCameraPosition; //カメラのデフォルト位置




    // Use this for initialization
    void Start () {
        gameState = GameState.boosted;



        /*オブジェクトの事前取得*/

        //UI制御スクリプトの取得
        uIController = GetComponent<UIcontroller>();

        //メインカメラの取得
        mainCameraObject = GameObject.Find("Main Camera");
        mainCamera = mainCameraObject.GetComponent<Camera>();

        //プレイヤーの取得
        playerObject = GameObject.Find("Player");
        //ゲームエリアの取得
        gameAreaObject = GameObject.Find("GameArea");

        /*オブジェクトの事前取得ここまで*/



        /*ポストプロセス関連の初期化処理*/

        //ポストプロセスの設定コンポーネントの取得
        mainPPSvolume = gameAreaObject.GetComponent<PostProcessVolume>();

        //chromaticAbberrationのインスタンスを設定ごとScriptableObjectで作成
        chromaticAberration = ScriptableObject.CreateInstance<ChromaticAberration>();
        
        //PPSのchromaticAbberrationインスタンスを上書きして設定を変更。
        //chromaticAbberrationの有効/無効の値にtrueをoverrideして有効にする
        chromaticAberration.enabled.Override(true);
        //chromaticAbberrationのintensityの値に1をoverrideして有効にする
        chromaticAberration.intensity.Override(1);

        //PostProcessManagerの中のQuickVolumeを使って、使用したいvolumeのインスタンスをPPSのvolumeに設定。
        //QuickVolume(対象となるlayer, レンダーレイヤー, 対象となるVolumeのインスタンス)
        mainPPSvolume = PostProcessManager.instance.QuickVolume(8, 100f, chromaticAberration);

        //quickvolumeを使用する場合は生成したインスタンスがPPSvolumeの該当volumeとして振る舞うので(パスが通っているので)
        //値を変更する場合はインスタンスを通常のgetcomponentのように扱えばよい。

        /*ポストエフェクト関連の初期化処理ここまで*/




        /*ゲーム開始時の演出に関する処理*/

        //カメラの初期位置を変更
        initCameraPosition = new Vector3(0, 14, -20);

        //ゲームエリアを移動
        gameAreaObject.transform.DOMove(new Vector3(0.0f, gameAreaObject.transform.position.y, 0.0f), boostTime);

        //ブースターパージ関連のUIに関する処理

        //パージUIを取得
        purgeUI = GameObject.Find("PurgeUI");
        //UIのキャンバスグループを取得
        purgeUICanvas = purgeUI.GetComponent<CanvasGroup>();
        //UI点滅処理開始のフラグをオフ
        isPurgeUIStarted = false;

        //パージするブースターのオブジェクトを取得
        booster = GameObject.Find("booster");


        //演出終了時間を常に
        purgeUIEndDuration = purgeUIduration * 3;


        //高速移動中はカメラにシェイクを加える。
        mainCameraObject.transform.DOShakePosition(duration: 1.0f,
                                                   strength: 1.0f,
                                                   vibrato: 20,
                                                   randomness: 10,
                                                   snapping: false,
                                                   fadeOut: false)
                                                  .SetLoops((int)boostTime);
          
        //カメラのFOVを高速移動中は高くする。
        mainCamera.fieldOfView = 110;

        /*ゲーム開始時の演出に関する処理ここまで*/
    }

    // Update is called once per frame
    void Update () {
        //マスタータイマーのカウントを増加
        masterTime += Time.deltaTime;

        //開始時演出の終了処理
        CheckBoostEnd();

    }

    //開始時演出の終了処理
    private void CheckBoostEnd()
    {
        //マスタータイマーがブースト時間以降かつフラグがオフの場合実行
        if (masterTime >= boostTime - purgeUIEndDuration && isPurgeUIStarted == false)
        {
            mainCameraObject.transform.DOShakePosition(duration: purgeUIEndDuration,
                                     strength: 1.0f,
                                     vibrato: 20,
                                     randomness: 10,
                                     snapping: false,
                                     fadeOut: false);

            //ブースターオブジェクトの崩壊処理を予約実行
            booster.GetComponent<PurgeBooster>().CollapseStart();

            //ブースト終了時のUIアニメーションを実行
            uIController.StartBoostEndUI();

            //高速移動時の色収差エフェクトをオフ
            chromaticAberration.intensity.value = 0;

            //パージが実行されたかのフラグをオン
            isPurgeUIStarted = true;
        }

        //マスタータイマーがブースト時間以降かつゲームステートがブースト状態の場合実行
        if (masterTime >= boostTime && gameState == GameState.boosted)
        {
            //ゲームステートを通常状態に変更
            gameState = GameState.play;

            //メインカメラを通常位置に遷移
            mainCameraObject.transform.DOLocalMove(initCameraPosition, 1.0f).SetEase(Ease.OutCirc);

            //メインカメラのFOVを元に戻す。
            mainCamera.DOFieldOfView(70, 2.0f);

            //高速移動時のエフェクトをオフ
            GameObject boostparticle = GameObject.Find("boostparticle");
            boostparticle.SetActive(false);


        }

    }

}
