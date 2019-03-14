using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIcontroller : MonoBehaviour {
    //プレイヤーのオブジェクトを取得
    private GameObject player;

    //オートモードに関する変数
    private bool isAutoModeSelected; //オートモードかどうかのフラグ
    private bool isSelected; //オートモードの変更処理完了のフラグ

    //ブーストゲージに関する変数
    private GameObject boostGageSliderObject; //ブーストゲージUIのオブジェクト
    private GameObject boostGageFill;                         //ブーストゲージのゲージ部分
    private float boostGageValue;　　　　　　　　　　　　　　 //ブーストゲージの残量
    private Image boostGageFillImage;                         //ブーストゲージのimageコンポーネント 色変更に使用
    private Slider boostGageSlider;                           //ブーストゲージのSliderコンポーネント

    private GameObject magazineGageSliderObject; //弾倉ゲージUIのオブジェクト
    private GameObject magazineGageFill;                          //弾倉ゲージのゲージ部分
    private float magazineGageValue;                              //弾倉ゲージの残量
    private Image magazineGageFillImage;                          //弾倉ゲージのimageコンポーネント 色変更に使用
    private Slider magazineGageSlider;                            //弾倉ゲージのSliderコンポーネント

    private PlayerController playerController; //playerスクリプトのコンポーネント
    [SerializeField] private float boostGageAlertValue; //ブーストゲージの色変更の閾値
    [SerializeField] private float magazineGageAlertValue;//弾倉ゲージの色変更の閾値

    // Use this for initialization
    void Start() {
        isAutoModeSelected　= false;
        player = GameObject.Find("Player");
        playerController = player.GetComponent<PlayerController>();

        //ブーストゲージのUI、コンポーネントを取得
        boostGageSliderObject = GameObject.Find("BoostGage");
        boostGageFill = GameObject.Find("boostFill");
        boostGageFillImage = boostGageFill.gameObject.GetComponent<Image>();
        boostGageSlider = boostGageSliderObject.GetComponent<Slider>();

        //弾倉ゲージのUI、コンポーネントを取得
        magazineGageSliderObject = GameObject.Find("MagazineGage");
        magazineGageFill = GameObject.Find("magazineFill");
        magazineGageFillImage = magazineGageFill.gameObject.GetComponent<Image>();
        magazineGageSlider = magazineGageSliderObject.GetComponent<Slider>();


    }

    // Update is called once per frame
    void Update() {
        BoostManage();
        MagazineManage();
    }

    //ブーストゲージの管理処理
    private void BoostManage() {
        //ブーストゲージの値をplayerから取得して変更
        boostGageValue = playerController.boostGage;
        boostGageSlider.value = boostGageValue;

        //一定値以下の場合色を変更
        if (boostGageValue < boostGageAlertValue)
        {
            boostGageFillImage.color = new Color(222.0f / 255.0f, 18.0f / 255.0f, 18.0f / 255.0f, 120.0f / 255.0f);
        }
        if (boostGageValue > boostGageAlertValue)
        {
            boostGageFillImage.color = new Color(108.0f / 255.0f, 255.0f / 255.0f, 255.0f / 255.0f, 120.0f / 255.0f);
        }
    }

    //弾倉ゲージの管理処理
    private void MagazineManage() {
        //弾倉ゲージの値をplayerから取得して変更
        magazineGageValue = playerController.magazineGage;
        magazineGageSlider.value = magazineGageValue;

        //一定値以下の場合色を変更
        if (magazineGageValue < magazineGageAlertValue)
        {
            magazineGageFillImage.color = new Color(222.0f / 255.0f, 18.0f / 255.0f, 18.0f / 255.0f, 120.0f / 255.0f);
        }
        if (magazineGageValue > magazineGageAlertValue)
        {
            magazineGageFillImage.color = new Color(108.0f / 255.0f, 255.0f / 255.0f, 255.0f / 255.0f, 120.0f / 255.0f);
        }
    }

    //オート射撃モードをオンにする関数
    void AutoShot()
    {
        //1つのボタンに機能を割り当てるため現在の状態で処理を分岐
        if (isAutoModeSelected == false && isSelected == false)
        {
            player.GetComponent<PlayerController>().isAutoShot = true;
            isAutoModeSelected = true;
            isSelected = true;
        }

        if (isAutoModeSelected == true && isSelected == false)
        {
            player.GetComponent<PlayerController>().isAutoShot = false;
            isAutoModeSelected = false;
            isSelected = true;
        }

        isSelected = false;
    }

}
