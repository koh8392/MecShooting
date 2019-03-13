using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIcontroller : MonoBehaviour {
    private GameObject player;
    private bool isAutoModeSelected;
    private bool isSelected;

    private GameObject boostGageSliderObject;
    private GameObject boostGageFill;
    private float boostGageValue;
    private Image boostGageFillImage;
    private Slider boostGageSlider;

    private GameObject magazineGageSliderObject;
    private GameObject magazineGageFill;
    private float magazineGageValue;
    private Image magazineGageFillImage;
    private Slider magazineGageSlider;

    private PlayerController playerController;
    [SerializeField] private float boostGageAlertValue;

    // Use this for initialization
    void Start() {
        isAutoModeSelected　= false;
        player = GameObject.Find("Player");
        boostGageSliderObject = GameObject.Find("BoostGage");
        boostGageFill = GameObject.Find("Fill");
        boostGageFillImage = boostGageFill.gameObject.GetComponent<Image>();
        boostGageSlider = boostGageSliderObject.GetComponent<Slider>();

        magazineGageSliderObject = GameObject.Find("MagazineGage");
        magazineGageFill = GameObject.Find("magazineFill");
        magazineGageFillImage = boostGageFill.gameObject.GetComponent<Image>();
        magazineGageSlider = boostGageSliderObject.GetComponent<Slider>();

        playerController = player.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update() {
        boostGageValue = playerController.boostgage;
        boostGageSlider.value = boostGageValue;
        if (boostGageValue < boostGageAlertValue)
        {
            boostGageFillImage.color = new Color(222.0f / 255.0f, 18.0f / 255.0f, 18.0f / 255.0f, 120.0f / 255.0f);
        }
        if (boostGageValue > boostGageAlertValue){
            boostGageFillImage.color = new Color(108.0f / 255.0f, 255.0f / 255.0f, 255.0f / 255.0f, 120.0f / 255.0f);
        }
        magazineGageValue = playerController.magagineGage;
        magazineGageSlider.value = magazineGageValue;

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
