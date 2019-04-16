using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;
using GameParameters;

public class UIcontroller :  MonoBehaviour {
    //プレイヤーのオブジェクトを取得
    private GameObject player;
    private GameState currentGameState;

    //オートモードに関する変数
    private bool isSelecting; //オートモードの変更処理完了のフラグ

    //ブーストゲージに関する変数
    private GameObject boostUI; //ブーストゲージUIのオブジェクト
    private GameObject boostGageFill;                         //ブーストゲージのゲージ部分
    private float boostGageValue;　　　　　　　　　　　　　　 //ブーストゲージの残量
    private Image boostGageFillImage;                         //ブーストゲージのimageコンポーネント 色変更に使用
    private Slider boostGageSlider;                           //ブーストゲージのSliderコンポーネント

    private GameObject magazineUI; //弾倉ゲージUIのオブジェクト
    private GameObject magazineGageFill;                          //弾倉ゲージのゲージ部分
    private float magazineGageValue;                              //弾倉ゲージの残量
    private Image magazineGageFillImage;                          //弾倉ゲージのimageコンポーネント 色変更に使用
    private Slider magazineGageSlider;                            //弾倉ゲージのSliderコンポーネント

    private GameObject playerHPUI;                                //HPゲージUIのオブジェクト
    private GameObject playerHPGageFill;                          //HPゲージのゲージ部分
    private float playerHPGageValue;                              //HPゲージの残量
    private Image playerHPGageFillImage;                          //HPゲージのimageコンポーネント 色変更に使用
    private Slider playerHPGageSlider;                            //HPゲージのSliderコンポーネント


    private PlayerController playerController; //playerスクリプトのコンポーネント
    [SerializeField] private float boostGageAlertValue; //ブーストゲージの色変更の閾値
    [SerializeField] private float magazineGageAlertValue;//弾倉ゲージの色変更の閾値

    //ブースターのパージに関する処理
    private GameObject purgeUI;                  //パージ予告表示のUI
    private CanvasGroup purgeUICanvas;           //パージUIのcanvas
    private float purgeUIalpha;                  //パージUIのalpha
    private bool isPurgeUIStarted;               //パージ予告処理を開始したかのフラグ
    private float purgeUIduration; 　　　　　　　//パージUIの点滅時間
    private float purgeUIEndDuration;            //パージUIの終了時間

    private float boostRemainTime;
    private float initBoostTime;
    private Text boostRemainTimeText;
    private CanvasGroup boostUICanvas;
    private float boostUIalpha;

    private CanvasGroup playUI;
    private CanvasGroup battleStartUI;

    private RectTransform boostUItransform;
    private Vector2 defaultBoostUIposition;

    private RectTransform magazineUItransform;
    private Vector2 defaultMagazineUIposition;

    [SerializeField] [Range(0, 255)] private float gageColorR;
    [SerializeField] [Range(0, 255)] private float gageColorG;
    [SerializeField] [Range(0, 255)] private float gageColorB;
    [SerializeField] [Range(0, 255)] private float gageColorAlpha;

    [SerializeField] [Range(0, 255)] private float gageAlertColorR;
    [SerializeField] [Range(0, 255)] private float gageAlertColorG;
    [SerializeField] [Range(0, 255)] private float gageAlertColorB;
    [SerializeField] [Range(0, 255)] private float gageAlertColorAlpha;


    //private RectTransform UItransform;
    //private Vector2 defaultUIposition;

    private Text inputText;
    private Text timeText;
    private Text scoreText;
    private Slider scoreSlider;
        

    //以下、UIの透明度の初期化ユーティリティ
    [SerializeField] [Range(0, 1)] private float purgeUIdefaultalpha; //
    [SerializeField] [Range(0, 1)] private float boostUIdefaultalpha; //
    [SerializeField] [Range(0, 1)] private float playUIdefaultalpha; //
    //[Range(0, 1)] private float UIdefaultalpha; //
    //[Range(0, 1)] private float UIdefaultalpha; //

    Vector2[] defaultPositionArray;



    // Use this for initialization
    void Start() {
        //プレイヤーの取得処理
        player = GameObject.Find("Player");
        playerController = player.GetComponent<PlayerController>();
        //ゲームステートの管理
        currentGameState = GameManager.gameState;

        inputText = GameObject.Find("inputUI").GetComponent<Text>();
        timeText = GameObject.Find("timeUI").GetComponent<Text>();


        //ブーストゲージのUI、コンポーネントを取得
        scoreText = GameObject.Find("scoreText").GetComponent<Text>();
        scoreSlider = GameObject.Find("scoreSlider").GetComponent<Slider>();

        //パージUIの取得処理]
        purgeUI = GameObject.Find("PurgeUI");
        purgeUICanvas = purgeUI.GetComponent<CanvasGroup>();
        isPurgeUIStarted = false;
        purgeUIduration = GetComponent<GameManager>().purgeUIduration;
        purgeUIEndDuration = purgeUIduration * 3;

        //ブーストゲージのUI、コンポーネントを取得
        boostUI = GameObject.Find("BoostGage");
        boostGageFill = GameObject.Find("boostFill");
        boostGageFillImage = boostGageFill.gameObject.GetComponent<Image>();
        boostGageSlider = boostUI.GetComponent<Slider>();

        //弾倉ゲージのUI、コンポーネントを取得
        magazineUI = GameObject.Find("MagazineGage");
        magazineGageFill = GameObject.Find("magazineFill");
        magazineGageFillImage = magazineGageFill.gameObject.GetComponent<Image>();
        magazineGageSlider = magazineUI.GetComponent<Slider>();

        //HPゲージのUI、コンポーネントを取得
        playerHPUI = GameObject.Find("HPGage");
        playerHPGageFill = GameObject.Find("HPGageFill");
        playerHPGageFillImage = playerHPGageFill.gameObject.GetComponent<Image>();
        playerHPGageSlider = playerHPUI.GetComponent<Slider>();

        //ブースト残り時間関連のテキストを取得
        boostRemainTimeText = GameObject.Find("BoostTimeText").GetComponent<Text>();
        initBoostTime = GameObject.Find("GameManager").GetComponent<GameManager>().boostTime;
        boostUICanvas = GameObject.Find("BoostUI").GetComponent<CanvasGroup>();
        boostRemainTime = 0;

        playUI = GameObject.Find("PlayUI").GetComponent<CanvasGroup>();

        battleStartUI = GameObject.Find("battlestartUI").GetComponent<CanvasGroup>();

        playUI.gameObject.GetComponentsInChildren<PlayUITag>().ToList().ForEach(ui =>
        {
            RectTransform uiPosition = ui.gameObject.GetComponent<RectTransform>();
            Vector2 defaultuiposition = uiPosition.anchoredPosition;

            uiPosition.anchoredPosition = new Vector2(0.0f, 0.0f);
            //配列に獲得したdefaultpositionを格納→スタートのタイミングで呼び出し

            Sequence seq = DOTween.Sequence();
            seq.Join(
            uiPosition.DOAnchorPos(defaultuiposition, 0.5f)).SetDelay(7.5f);
        });

        //以下、UIの透明度の初期化処理
        purgeUI.GetComponent<CanvasGroup>().alpha = purgeUIdefaultalpha;
        boostUICanvas.GetComponent<CanvasGroup>().alpha = boostUIdefaultalpha;
        playUI.GetComponent<CanvasGroup>().alpha = playUIdefaultalpha;
    }

    // Update is called once per frame
    void Update() {
        ManageBoostgage();
        ManageMagazinegage();
        ManagePlayerHPgage();
        ManageBoostTime();
        ManageScore();

        inputText.text = playerController.moveX.ToString("f2");
        timeText.text = GameManager.masterTime.ToString("f0");


}

    private void ManageScore()
    {
        scoreText.text = GameManager.score.ToString();
        scoreSlider.value = GameManager.score % scoreSlider.maxValue;
    }

    //ブースト残り時間の表示
    private void ManageBoostTime()
    {
        if (boostRemainTime >= 0)
        {
            boostRemainTime = initBoostTime - GameManager.masterTime;
            boostRemainTimeText.text = boostRemainTime.ToString();
        }

        if (boostRemainTime < 0)
        {
            boostRemainTime = initBoostTime - GameManager.masterTime;
            boostRemainTimeText.text = 0.0f.ToString();
        }

    }

    //ブーストゲージの管理処理
    private void ManageBoostgage() {
        //ブーストゲージの値をplayerから取得して変更
        boostGageValue = playerController.boostGage;
        boostGageSlider.value = boostGageValue;

        //一定値以下の場合色を変更
        if (boostGageValue < boostGageAlertValue)
        {
            boostGageFillImage.color = new Color(gageAlertColorR / 255.0f, gageAlertColorG / 255.0f, gageAlertColorB / 255.0f, gageAlertColorAlpha / 255.0f);
        }
        if (boostGageValue > boostGageAlertValue)
        {
            boostGageFillImage.color = new Color(gageColorR / 255.0f, gageColorG / 255.0f, gageColorB / 255.0f, gageColorAlpha / 255.0f);
        }
    }

    //弾倉ゲージの管理処理
    private void ManageMagazinegage() {
        //弾倉ゲージの値をplayerから取得して変更
        magazineGageValue = playerController.magazineGage;
        magazineGageSlider.value = magazineGageValue;

        //一定値以下の場合色を変更
        if (magazineGageValue < magazineGageAlertValue)
        {
            magazineGageFillImage.color = new Color(gageColorR / 255.0f, gageColorG / 255.0f, gageColorB / 255.0f, gageColorAlpha / 255.0f);
        }
        if (magazineGageValue > magazineGageAlertValue)
        {
            magazineGageFillImage.color = new Color(gageColorR / 255.0f, gageColorG / 255.0f, gageColorB / 255.0f, gageColorAlpha / 255.0f);
        }
    }

    //HPゲージの管理処理
    private void ManagePlayerHPgage()
    {
        //弾倉ゲージの値をplayerから取得して変更
        playerHPGageValue = playerController.playerCurrentHP / playerController.playerDefaultHP;
        playerHPGageSlider.value = playerHPGageValue;

        //一定値以下の場合色を変更
        if (playerHPGageValue < magazineGageAlertValue)
        {
            playerHPGageFillImage.color = new Color(gageColorR / 255.0f, gageColorG / 255.0f, gageColorB / 255.0f, gageColorAlpha / 255.0f);
        }
        if (playerHPGageValue > magazineGageAlertValue)
        {
            playerHPGageFillImage.color = new Color(gageColorR / 255.0f, gageColorG / 255.0f, gageColorB / 255.0f, gageColorAlpha / 255.0f);
        }
    }

    //開始時演出の終了処理開始。
    public void StartBoostEndUI()
    {
        //マスタータイマーがブースト時間以降かつフラグがオフの場合実行
        if (isPurgeUIStarted == false)
        {
            
            //パージ時のUIの点滅処理
            purgeUIalpha = purgeUICanvas.alpha;

            //canvasgroupの透明度に対してDoFadeでTween
            purgeUICanvas.DOFade(1.0f, purgeUIduration).SetLoops(3);

            StartCoroutine("FadeOutUI");

            //パージが実行されたかのフラグをオン
            isPurgeUIStarted = true;

           
        }
    }

    public IEnumerator FadeOutUI()
    {
        yield return new WaitForSeconds(purgeUIEndDuration);
        Debug.Log("UI点滅終了処理開始");
        //canvasgroupの透明度に対してDoFadeでTween

        purgeUIalpha = purgeUICanvas.alpha;
        Sequence seq = DOTween.Sequence();

        seq.Append(
        purgeUICanvas.DOFade(0.0f, 1.0f));

        seq.Join(
        boostUICanvas.DOFade(0.0f, 1.0f)).AppendInterval(0.3f);

        seq.Join(
        battleStartUI.DOFade(1.0f, 0.15f).SetEase(Ease.Flash).SetLoops(3)).AppendInterval(1f);

        seq.Append(
        battleStartUI.DOFade(0.0f, 0.6f));

        seq.Join(
        playUI.DOFade(1.0f, 0.7f));


    }

    //オート射撃モードをオンにする関数
    public void AutoShot()
    {
        //1つのボタンにオン/オフ機能を割り当てるため現在の状態で処理を分岐



        //オートモードが実行中でない場合
        if (playerController.isAutoShot == false && isSelecting == false)
        {
            playerController.isAutoShot = true;
            isSelecting = true;
        }


        //オートモードが実行中の場合
        if (playerController.isAutoShot == true && isSelecting == false)
        {
            playerController.isAutoShot = false;
            isSelecting = true;
        }

        isSelecting = false;
    }

    public void exitButton()
    {
        UnityEngine.Application.Quit();
    }

}
