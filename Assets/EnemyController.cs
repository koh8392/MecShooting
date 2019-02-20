using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {
    private Vector3 enemyPosition;
    private Rigidbody enemyRigidBody;
    [SerializeField] private float enemyMoveSpeedX;
    [SerializeField] private float enemyMoveSpeedY;
    [SerializeField] private float enemyMoveSpeedZ;

    // Use this for initialization
    void Start () {
        enemyRigidBody = GetComponent<Rigidbody>();
        enemyMoveSpeedX = enemyMoveSpeedX / 10;
        enemyMoveSpeedY = enemyMoveSpeedY / 10;
        enemyMoveSpeedZ = enemyMoveSpeedZ / 10;
    }
	
	// Update is called once per frame
	void Update () {
        enemyPosition = enemyRigidBody.position;

        enemyPosition = new Vector3(enemyPosition.x + enemyMoveSpeedX,
                                    enemyPosition.y + enemyMoveSpeedY,
                                    enemyPosition.z + enemyMoveSpeedZ);
        enemyRigidBody.position = enemyPosition;
	}
}
