using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

//ゲームステートをenum型で設定
public enum GameState
{
    play = 1,
    boosted,
}


public class GameManager : MonoBehaviour {
    /*全体の処理の基準になる変数*/

    //実際のゲームステート
    public GameState gameState;
    //全体時間管理のタイマー
    public float masterTime;

    /**********ここまで*********/

    [SerializeField] private GameObject booster;
    private DestroyMesh destroyMesh;

    [SerializeField] private float boostTime;

    [SerializeField] private GameObject purgeUI;
    private CanvasGroup purgeUICanvas;
    private float purgeUIalpha;
    private bool isPurgeUIStarted;
    [SerializeField] private float purgeUIduration;
    private float purgeUIEndDuration;



    // Use this for initialization
    void Start () {
        gameState = GameState.boosted;
        
       
        //ブースターのパージ関連の初期化処理
        booster = GameObject.Find("booster");
        purgeUICanvas = purgeUI.GetComponent<CanvasGroup>();
        isPurgeUIStarted = false;

        purgeUIEndDuration = purgeUIduration * 3;

    }
	
	// Update is called once per frame
	void Update () {
        //マスタータイマーのカウントを増加
        masterTime += Time.deltaTime;

        //マスタータイマーがブースト時間以降かつフラグがオフの場合実行
        if (masterTime >= boostTime - purgeUIEndDuration && isPurgeUIStarted == false)
        {
            
            Debug.Log("UI点滅処理開始");

            //パージ時のUIの点滅処理
            purgeUIalpha = purgeUICanvas.alpha;

            //canvasgroupの透明度に対してDoFadeでTween
            purgeUICanvas.DOFade(1.0f, purgeUIduration).SetLoops(3);

            //ブースターオブジェクトの崩壊処理を予約実行
            booster.SendMessage("CollapseStart", 0);

            StartCoroutine("FadeOutUI");

            //パージが実行されたかのフラグをオン
            isPurgeUIStarted = true;
        }

        //マスタータイマーがブースト時間以降かつゲームステートがブースト状態の場合実行
        if (masterTime >= boostTime && gameState == GameState.boosted)
        {
            //ゲームステートを通常状態に変更
            gameState = GameState.play;
        }


	}


    public IEnumerator FadeOutUI()
    {
        yield return new WaitForSeconds(purgeUIEndDuration);
        Debug.Log("UI点滅終了処理開始");
        //canvasgroupの透明度に対してDoFadeでTween
        purgeUIalpha = purgeUICanvas.alpha;
        purgeUICanvas.DOFade(0.0f, 1.0f);
    }
}
