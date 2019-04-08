using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyController : MonoBehaviour {

    private Vector3 enemyPosition;                     //敵の位置(移動用)
    private Rigidbody enemyRigidBody;                  //敵の当たり判定
    private GameManager gameManager;

    //敵の移動速度
    [SerializeField] private float enemyMoveSpeedX;
    [SerializeField] private float enemyMoveSpeedY;
    [SerializeField] private float enemyMoveSpeedZ;

    [SerializeField] private float enemyMaxHP;          //敵の最大HP
    [SerializeField] private int enemyScore;
    private float enemyCurrentHP;                      //敵の現在のHP 
    private float bulletPower;                         //弾丸の威力(bulletからの取得用)
    private bool isEnemyAlive;                         //敵が生存中かどうか
    private float waitTime;                            //敵の破壊処理開始からデスポーンまでの時間

    [SerializeField] public Vector3 enemyMovePosition; //敵の生成位置
    [SerializeField] public Vector3 enemySpawnOffset;   //敵の移動元の位置
    private Vector3 gameAreaPosition;

    // Use this for initialization
    void Awake () {
        enemyRigidBody = GetComponent<Rigidbody>();
        enemyMoveSpeedX = enemyMoveSpeedX / 10;
        enemyMoveSpeedY = enemyMoveSpeedY / 10;
        enemyMoveSpeedZ = enemyMoveSpeedZ / 10;
        enemyCurrentHP = enemyMaxHP;

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        
        isEnemyAlive = true;
        waitTime = 2.0f;

        gameAreaPosition = GameObject.Find("GameArea").GetComponent<Transform>().position;

        //敵の登場処理

        //offset(登場開始位置)移動
        //GetComponent<Transform>().position = enemySpawnOffset;

        //登場開始位置から初期位置に向けて移動
        Sequence sequence = DOTween.Sequence().OnStart(() =>
        { 
            transform.DOLocalMove(gameAreaPosition + enemyMovePosition, 3.0f).SetEase(Ease.OutQuad);
        });
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        EnemyMove();

	}

    void LateUpdate()
    {
        EnemyDeath();
    }

    //敵の移動処理
    void EnemyMove()
    {
        //現在の位置を取得→次のフレームでの位置を計算し代入
        enemyPosition = enemyRigidBody.position;

        enemyPosition = new Vector3(enemyPosition.x + enemyMoveSpeedX,
                                    enemyPosition.y + enemyMoveSpeedY,
                                    enemyPosition.z + enemyMoveSpeedZ);
        enemyRigidBody.position = enemyPosition;
    }

    void EnemyDeath()
    {
        //HPで生存判定
        if (enemyCurrentHP < 0 && isEnemyAlive == true)
        {
            //敵が死亡かつ破壊処理が未処理であれば処理実行
            isEnemyAlive = false;
            //敵の当たり判定を衝突判定に変更
            gameObject.GetComponent<BoxCollider>().isTrigger = false;
            //破壊時のエフェクトを再生
            GameObject DestructionParticleStarter = transform.Find("EnemyDestructionParticleParent").gameObject;
            DestructionParticleStarter.GetComponent<ParticleStarter>().StartParticle();
            //GameManagerにスコアを加算(するメソッドを実行)
            gameManager.addScore(enemyScore);
            
            //一定時間経過後にオブジェクト自体を削除
            StartCoroutine("Wait");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //敵に弾丸が触れたら実行
        if (other.gameObject.tag == "Bullet")
        {
            //弾丸を消滅させ弾丸の威力分を敵のHPから減少させる
            Destroy(other.gameObject);
            bulletPower = other.gameObject.GetComponent<BulletController>().bulletPower;
            enemyCurrentHP = enemyCurrentHP - bulletPower;
            //Debug.Log("敵のHPは" + enemyCurrentHP);
        }
    }

    private void OnCollisionEnter(Collision collision )
    {
        if (collision.gameObject.tag == "Bullet")
        {
            //衝突判定時は弾がバウンドするので衝突時に削除
            Destroy(collision.gameObject);
        }
    }

    private IEnumerator Wait()
    {

        //waitTime経過後に敵をデスポーン
        yield return new WaitForSeconds(waitTime);
        Destroy(gameObject);
    }
}
