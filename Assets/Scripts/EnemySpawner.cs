using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {

    private GameObject enemyPrefab;
    private Vector3 spawnOffset;
    private GameObject enemyInstance;
    private Quaternion enemyDirection;
    private int spawnNumber;
    [SerializeField]private int initSpawnNumber;
    private float waitTime;

    // Use this for initialization
    void Start () {
        waitTime = 5.0f;

        Debug.Log(gameObject.name);

        spawnNumber = 0;

        //敵のプレハブを読み込み
        enemyPrefab = (GameObject)Resources.Load("Prefabs/Enemy");

        Debug.Log(enemyPrefab.name);

        //敵のパラメータを事前取得
        spawnOffset = enemyPrefab.GetComponent<EnemyController>().enemySpawnOffset;

        enemyDirection = new Quaternion(0, 0, 0, 0);

        for(spawnNumber = 0  ; spawnNumber < initSpawnNumber;  spawnNumber = spawnNumber + 1)
        {
            StartCoroutine("SpawnEnemy");
            waitTime += 1.0f;

            Debug.Log("ここまで実行");
        }


    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private IEnumerator SpawnEnemy()
    {
        yield return new WaitForSeconds(waitTime);
        enemyInstance = Instantiate(enemyPrefab, spawnOffset, enemyDirection) as GameObject;
    }
}
