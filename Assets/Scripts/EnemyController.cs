using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

    private Vector3 enemyPosition;                     //敵の位置(移動用)
    private Rigidbody enemyRigidBody;                  //敵の当たり判定

    //敵の移動速度
    [SerializeField] private float enemyMoveSpeedX;   
    [SerializeField] private float enemyMoveSpeedY;
    [SerializeField] private float enemyMoveSpeedZ;

    [SerializeField]private float enemyMaxHP;          //敵の最大HP
    private float enemyCurrentHP;                      //敵の現在のHP 
    private float bulletPower;                         //弾丸の威力(bulletからの取得用)
    private bool isEnemyAlive;                         //敵が生存中かどうか
    private float waitTime;                            //敵の破壊処理開始からデスポーンまでの時間

    // Use this for initialization
    void Start () {
        enemyRigidBody = GetComponent<Rigidbody>();
        enemyMoveSpeedX = enemyMoveSpeedX / 10;
        enemyMoveSpeedY = enemyMoveSpeedY / 10;
        enemyMoveSpeedZ = enemyMoveSpeedZ / 10;
        enemyMaxHP = enemyCurrentHP;
        isEnemyAlive = true;
        waitTime = 4.0f;
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
