using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class testplayer : MonoBehaviour {
    //対応するUIをアタッチ
    [SerializeField] private GameObject PlayerUI;

    float moveX;
    float moveY;
    [SerializeField]float playerMoveSpeedX;
    [SerializeField]float playerMoveSpeedY;


    // Use this for initialization
    void Start () {
        //対応するUIを有効化、オブジェクトを指定。
        PlayerUI.GetComponent<EnemyUIController>().ActivateUI(this.gameObject);
        //GetComponent<Transform>().DOLocalMoveX(20, 3.0f);
    }
	
	// Update is called once per frame
	void Update () {
        //PlayerMove();

    }

    void PlayerMove()
    {

            //現在の方向キーの入力の値を取得
            moveX = Input.GetAxis("Horizontal");
            moveY = Input.GetAxis("Vertical");

            //実際の移動処理
            gameObject.transform.Translate((moveX * playerMoveSpeedX), (moveY * playerMoveSpeedY), 0, Space.Self);

    }
}
