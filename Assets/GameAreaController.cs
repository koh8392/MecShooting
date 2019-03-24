using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAreaController : MonoBehaviour {
    private Rigidbody gameAreaRigidBody;
    private Vector3 gameAreaPosition;
    [SerializeField]private float AreaScrollSpeed;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GameManager.gameState == GameParameters.GameState.play)
        {
            gameObject.transform.Translate(0, 0, AreaScrollSpeed, Space.Self);
        }
    }
}
