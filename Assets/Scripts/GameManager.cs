using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using GameParameters;
using UnityEngine.Rendering.PostProcessing;

public class GameManager : MonoBehaviour {
    /*全体の処理の基準になる変数*/

    //実際のゲームステート
    public static GameState gameState;
    //全体時間管理のタイマー
    public static float masterTime;

    /**********ここまで*********/

    private UIcontroller uIController;
    private GameObject playerObject;
    private GameObject gameAreaObject;

    //ブースターのパージに関する処理
    [SerializeField] private GameObject booster; //ブースターのオブジェクト
    private GameObject mainCameraObject;         //メインカメラのオブジェクト
    private Camera mainCamera;                   //メインカメラのコンポーネント
    private DestroyMesh destroyMesh;             //ブースターのパージ処理
    [SerializeField] public float boostTime;    //開幕のブースト時間
    private GameObject purgeUI; //パージ予告表示のUI
    private CanvasGroup purgeUICanvas;           //パージUIのcanvas
    private float purgeUIalpha;                  //パージUIのalpha
    private bool isPurgeUIStarted;               //パージ予告処理を開始したかのフラグ
    [SerializeField] public float purgeUIduration; //パージUIの点滅時間
    private float purgeUIEndDuration;            //パージUIの終了時間

    private PostProcessVolume mainPPSvolume;
    private ChromaticAberration chromaticAberration;

    private Vector3 initCameraPosition;
    

    

    // Use this for initialization
    void Start () {
        gameState = GameState.boosted;

        uIController = GetComponent<UIcontroller>();

        mainCameraObject = GameObject.Find("Main Camera");
        mainCamera = mainCameraObject.GetComponent<Camera>();

        playerObject = GameObject.Find("Player");
        gameAreaObject = GameObject.Find("GameArea");
        mainPPSvolume = gameAreaObject.GetComponent<PostProcessVolume>();

        initCameraPosition = new Vector3(0, 7, -20);

        /*ポストエフェクト関連の処理*/

        //chromaticAbberrationのインスタンスをScriptableObjectで作成
        chromaticAberration = ScriptableObject.CreateInstance<ChromaticAberration>();
        
        //PPSのchromaticAbberrationインスタンスを生成した際の初期化処理。ここに初期状態を記述しておく。
        //chromaticAbberrationの有効/無効の値にtrueをoverrideして有効にする
        chromaticAberration.enabled.Override(true);
        //chromaticAbberrationのintensityの値に1をoverrideして有効にする
        chromaticAberration.intensity.Override(1);

        //PostProcessManagerの中のQuickVolumeを使って、使用したいvolumeのインスタンスをPPSのvolumeに設定。
        //QuickVolume(対象となるlayer, priority※よくわからん, 対象となるVolumeのインスタンス)
        mainPPSvolume = PostProcessManager.instance.QuickVolume(8, 100f, chromaticAberration);

        //quickvolumeを使用する場合は生成したインスタンスがPPSvolumeの該当volumeとして振る舞うので(パスが通っているので)
        //値を変更する場合はインスタンスを通常のgetcomponentのように扱えばよい。

        /*ポストエフェクト関連の処理ここまで*/

        gameAreaObject.transform.DOMove(new Vector3(0.0f, 0.0f, 0.0f), boostTime);

        //ブースターのパージ関連の初期化処理
        purgeUI = GameObject.Find("PurgeUI");
        booster = GameObject.Find("booster");                 
        purgeUICanvas = purgeUI.GetComponent<CanvasGroup>();
        isPurgeUIStarted = false;

        purgeUIEndDuration = purgeUIduration * 3;

        mainCameraObject.transform.DOShakePosition(duration: 1.0f,
                                             strength: 1.0f,
                                             vibrato: 20,
                                             randomness: 10,
                                             snapping: false,
                                             fadeOut: false)
                                             .SetLoops((int)boostTime);

        mainCamera.fieldOfView = 110;
    }
	
	// Update is called once per frame
	void Update () {
        //マスタータイマーのカウントを増加
        masterTime += Time.deltaTime;

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
            booster.GetComponent<DestroyMesh>().CollapseStart();

            uIController.StartBoostUI();

            chromaticAberration.intensity.value = 0;


            //mainCameraObject.transform.DOLocalMove(new Vector3(0, mainCameraObject.transform.position.y + 3, mainCameraObject.transform.position.z - 11), 1.0f).SetEase(Ease.OutCirc);
            //パージが実行されたかのフラグをオン
            isPurgeUIStarted = true;
        }

        //マスタータイマーがブースト時間以降かつゲームステートがブースト状態の場合実行
        if (masterTime >= boostTime && gameState == GameState.boosted)
        {
            //ゲームステートを通常状態に変更
            gameState = GameState.play;

            //uIController.blinkBattleStartUI();

            //mainCameraObject.transform.parent = null;
            mainCameraObject.transform.DOLocalMove(initCameraPosition, 1.0f).SetEase(Ease.OutCirc);
            mainCamera.DOFieldOfView(70, 2.0f);
            GameObject boostparticle = GameObject.Find("boostparticle");
            boostparticle.SetActive(false);
            
        }


	}

}
