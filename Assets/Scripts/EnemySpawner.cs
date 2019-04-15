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
    private int spawnNumber;
    [SerializeField] private int initSpawnNumber;

    [SerializeField] private float initWaitTime;
    private float waitTime;
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

        Debug.Log(gameObject.name);

        spawnNumber = 0;

        //敵のプレハブを読み込み
        enemyPrefab = (GameObject)Resources.Load("Prefabs/General/Enemy");


        //待機時間のタイマーをリセット
        waitTime = initWaitTime;

        //Debug.Log(enemyPrefab.name);


        //敵の方向を決定(現状はひとまず前方方向に)
        enemyDirection = new Quaternion(0, 0, 0, 0);

        //最初に一回呼び出し
        for(spawnNumber = 0  ; spawnNumber < initSpawnNumber;  spawnNumber = spawnNumber + 1)
        {
            waitTime += enemyInterval;
            StartCoroutine("SpawnEnemy" , waitTime);

        }


    }
	
	// Update is called once per frame
	void Update () {
        //生成タイマーは時間で加算される
        spawnTimer += Time.deltaTime;
        
        //生成タイマーが生成間隔を上回った時に実行
        if (spawnTimer >= spawnInterval)
        {
            //生成タイマーをリセット
            spawnTimer = 0.0f;

            //指定された回数(initSpawnNumber)分だけ生成処理を実行
            for (spawnNumber = 0; spawnNumber < initSpawnNumber; spawnNumber = spawnNumber + 1)
            {
                waitTime += enemyInterval;

                StartCoroutine("SpawnEnemy" , waitTime);
                //enemyInterval分の間隔で敵を生成。
                
            }

            //待機時間のタイマーをリセット
            waitTime = initWaitTime;

            Debug.Log("ここまで実装");
        }

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

        Debug.Log(moveTime);
        
    }



}
