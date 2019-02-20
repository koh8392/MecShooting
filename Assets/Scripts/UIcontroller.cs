using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIcontroller : MonoBehaviour {
    private GameObject player;
    private bool isAutoModeSelected;
    private bool isSelected;


    // Use this for initialization
    void Start() {
            isAutoModeSelected　= false;
            player = GameObject.Find("Player"); 

    }

    // Update is called once per frame
    void Update() {

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
