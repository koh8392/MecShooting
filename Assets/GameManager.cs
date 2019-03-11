using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GameManager : MonoBehaviour {
    private enum GameState {
        play = 1,
        boosted,
    }

    private GameState gameState;
    private float masterTime;

    [SerializeField] private GameObject booster;
    private DestroyMesh destroyMesh;

    [SerializeField] private float boostTime;

    [SerializeField] private GameObject purgeUI;
    private CanvasGroup purgeUICanvas;
    private float purgeUIalpha;
    private bool isPurgeUIStarted;

	// Use this for initialization
	void Start () {
        gameState = GameState.boosted;
        //booster = GameObject.Find("booster");
        purgeUICanvas = purgeUI.GetComponent<CanvasGroup>();
        isPurgeUIStarted = false;
	}
	
	// Update is called once per frame
	void Update () {
        //タイマーのカウントを増加
        masterTime += Time.deltaTime;

        if (masterTime >= boostTime - 2 && isPurgeUIStarted == false)
        {
            Debug.Log("UI点滅処理開始");
            purgeUIalpha = purgeUICanvas.alpha;
            //DOTween.To(() => purgeUIalpha, (n) => purgeUIalpha = n, 1.0f, 0.5f).SetLoops(3);
            purgeUIalpha = 1;
            isPurgeUIStarted = true;
        }

            if (masterTime >= boostTime && gameState == GameState.boosted) {
            gameState = GameState.play;
            booster.SendMessage("CollapseObject",0);
        }



	}
}
