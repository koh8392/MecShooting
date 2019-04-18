using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemySpawner : MonoBehaviour {

    private Vector3 movePosition; //移動後の位置
    private Vector3 gameAreaPosition; //基準となる座標
    private GameObject enemyPrefab;   
    private Vector3 spawnOffset;
    private GameObject enemyInstance;
    private Quaternion enemyDirection;
    private int spawnNumber;                         //グループ内で生成した回数(処理用)
    [SerializeField] private int initSpawnNumber;    //グループに何体敵を生成するか

    [SerializeField] private float initWaitTime;     //何秒で生成を開始するか
    private float waitTime;    　　　　　　　　　　  //何秒で生成を開始するか(処理用)
    [SerializeField] private int initSpawnGroupNum;  //何グループ敵を生成するか
    private int spawnGroupCount;                       //何グループ敵を生成するか(処理用)

    private Vector3 spawnPosition;
    private Vector3 spawnTargetPosition;

    //初期移動に掛ける時間
    [SerializeField] private float moveTime;

    //敵生成管理用のタイマー
    private float spawnTimer;

    //敵グループ生成の間隔(時間)
    [SerializeField] private float spawnInterval;
    //グループ内での敵生成の間隔
    [SerializeField] private float enemyInterval;

    // Use this for initialization
    void Start () {

        gameAreaPosition = GameObject.Find("GameArea").GetComponent<Transform>().position;

        //生成後の目的地のオブジェクトから座標を取得
        spawnTargetPosition = transform.Find("SpawnTarget").gameObject.GetComponent<Transform>().position;
        //生成する位置(このスクリプトが貼ってあるオブジェクトの座標)を取得
        spawnPosition = gameObject.GetComponent<Transform>().position;

        spawnNumber = 0;
        spawnTimer = 0;

        //敵のプレハブを読み込み
        enemyPrefab = (GameObject)Resources.Load("Prefabs/General/Enemy");


        //待機時間のタイマーをリセット
        waitTime = initWaitTime;

        //生成するグループのカウンタを初期化
        spawnGroupCount = 0;


        //敵の方向を決定(現状はひとまず前方方向に)
        enemyDirection = new Quaternion(0, 0, 0, 0);

        //最初に一回呼び出し
        for(spawnNumber = 0  ; spawnNumber < initSpawnNumber;  spawnNumber = spawnNumber + 1)
        {
            waitTime += enemyInterval;
            StartCoroutine("SpawnEnemy" , waitTime);

        }
        //最初に一回呼び出した分を加算
        spawnGroupCount += 1;
    }
	
	// Update is called once per frame
	void Update () {
        //生成タイマーは時間で加算される
        spawnTimer += Time.deltaTime;
        
        //生成タイマーが生成間隔を上回った時に実行
        if (spawnTimer >= spawnInterval && spawnGroupCount < initSpawnGroupNum)
        {

            Debug.Log("生成処理を実行" + GameManager.masterTime);
            //生成タイマーをリセット
            spawnTimer = 0.0f;
            spawnGroupCount += 1;
            

            //指定された回数(initSpawnNumber)分だけ生成処理を実行
            for (spawnNumber = 0; spawnNumber < initSpawnNumber; spawnNumber = spawnNumber + 1)
            {
                //生成時間に間隔を加えて生成間隔を調整
                waitTime += enemyInterval;

                StartCoroutine("SpawnEnemy" , waitTime);
                //enemyInterval分の間隔で敵を生成。

                
            }

            //待機時間のタイマーをリセット
            waitTime = initWaitTime;  }

    }

    //敵を呼び出して開幕移動を実行させる処理
    private IEnumerator SpawnEnemy(float waitTimeInstance)
    {
        //呼び出しの待ち時間分待ったら
        yield return new WaitForSeconds(waitTimeInstance);
        //敵を読み込んでおいたプレハブから生成
        enemyInstance = Instantiate(enemyPrefab, spawnPosition , enemyDirection) as GameObject;
        //呼び出した敵に開幕移動処理をDoTweenで行う
        enemyInstance.transform.DOMove(spawnTargetPosition, moveTime);
        
        //敵の状態変更処理を予約実行
        enemyInstance.GetComponent<EnemyController>().setStateEnemyStart(moveTime);
        
    }



}
