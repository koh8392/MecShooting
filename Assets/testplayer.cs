using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
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

        //引数を指定したcoroutineを作成し実行
        StartCoroutine(DelayMethod(3.0f, () =>
       {
           transform.DOLocalMoveX(20, 3.0f);
           Debug.Log("移動実行");
       }));

        StartCoroutine(DelayMethod(1.0f, () =>
        {
            Debug.Log("ゲームスタート");
        }));
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

    //単純遅延を実現するコルーチン。
    //呼び出す側でアクションを指定すれば遅延して実行される。
    private IEnumerator DelayMethod(float delayTime, UnityAction Action)
    {
        yield return new WaitForSeconds(delayTime);
        Action();
    }
}
