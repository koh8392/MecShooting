using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {

    private Vector3 SpawnPosition; //初期位置
    private Vector3 movePosition; //移動後の位置
    private Vector3 gameAreaPosition; //基準となる座標
    private GameObject enemyPrefab;   
    private Vector3 spawnOffset;
    private GameObject enemyInstance;
    private Quaternion enemyDirection;
    private int spawnNumber;
    [SerializeField]private int initSpawnNumber;
    private float waitTime;

    private float spawnTimer;

    // Use this for initialization
    void Start () {

        gameAreaPosition = GameObject.Find("GameArea").GetComponent<Transform>().position;

        waitTime = 5.0f;

        Debug.Log(gameObject.name);

        spawnNumber = 0;

        //敵のプレハブを読み込み
        enemyPrefab = (GameObject)Resources.Load("Prefabs/Enemy");

        //Debug.Log(enemyPrefab.name);

        //敵の目標位置からのオフセットをを事前取得
        spawnOffset = enemyPrefab.GetComponent<EnemyController>().enemySpawnOffset;

        //敵の方向を決定(現状はひとまず前方方向に)
        enemyDirection = new Quaternion(0, 0, 0, 0);

        for(spawnNumber = 0  ; spawnNumber < initSpawnNumber;  spawnNumber = spawnNumber + 1)
        {
            StartCoroutine("SpawnEnemy");
            waitTime += 1.0f;
        }


    }
	
	// Update is called once per frame
	void Update () {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= 10)
        {
            spawnTimer = 0.0f;

            for (spawnNumber = 0; spawnNumber < initSpawnNumber; spawnNumber = spawnNumber + 1)
            {
                StartCoroutine("SpawnEnemy");
                waitTime += 1.0f;
            }

            Debug.Log("ここまで実装");
        }

    }

    private IEnumerator SpawnEnemy()
    {
        yield return new WaitForSeconds(waitTime);
        enemyInstance = Instantiate(enemyPrefab, new Vector3(gameAreaPosition.x + spawnOffset.x,gameAreaPosition.y + spawnOffset.y, gameAreaPosition.z + spawnOffset.z), enemyDirection) as GameObject;
    }
}
