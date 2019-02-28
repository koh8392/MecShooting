using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

    private Vector3 enemyPosition;
    private Rigidbody enemyRigidBody;
    [SerializeField] private float enemyMoveSpeedX;
    [SerializeField] private float enemyMoveSpeedY;
    [SerializeField] private float enemyMoveSpeedZ;
    [SerializeField]private float enemyMaxHP;
    private float enemyCurrentHP;
    private float bulletPower;
    private bool isEnemyAlive;
    private float waitTime;

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

    void EnemyMove()
    {
        enemyPosition = enemyRigidBody.position;

        enemyPosition = new Vector3(enemyPosition.x + enemyMoveSpeedX,
                                    enemyPosition.y + enemyMoveSpeedY,
                                    enemyPosition.z + enemyMoveSpeedZ);
        enemyRigidBody.position = enemyPosition;
    }

    void EnemyDeath()
    {
        if(enemyCurrentHP < 0 && isEnemyAlive == true)
        {
            isEnemyAlive = false;
            //Debug.Log("敵の破壊処理を開始します。");
            gameObject.GetComponent<BoxCollider>().isTrigger = false;

            StartCoroutine("Wait");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Bullet")
        {
            Destroy(other.gameObject);
            bulletPower = other.gameObject.GetComponent<BulletController>().bulletPower;
            enemyCurrentHP = enemyCurrentHP - bulletPower;
            Debug.Log("現在の敵HPは " + enemyCurrentHP);
        }
    }

    private void OnCollisionEnter(Collision collision )
    {
        if (collision.gameObject.tag == "Bullet")
        {
            Destroy(collision.gameObject);
        }
    }

    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(waitTime);
        Destroy(gameObject);
    }
}
