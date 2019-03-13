using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralParameters : MonoBehaviour {

    //ゲームマネージャーのオブジェクト
    private GameObject gameManager;

    private GameManager gameManagerScript;
    //実際のゲームステート
    public GameState gameStateClone;
    //全体時間管理のタイマー
    public float masterTimeClone;



    // Use this for initialization
    void Start () {
        gameManager = GameObject.Find("GameManager");
        gameManagerScript = gameManager.GetComponent<GameManager>();
    }
	
	// Update is called once per frame
	void Update () {
        //毎フレームゲームステートと全体時間のタイマーを取得
        gameStateClone = gameManagerScript.gameState;
        masterTimeClone = gameManagerScript.masterTime;
        //Debug.Log("ゲームステート取得時は" + gameStateClone);
    }
}
