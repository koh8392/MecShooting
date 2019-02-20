using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {

    private float lifeTime;
    private GameObject player;
    private float time;

	// Use this for initialization
	void Start () {
        player = GameObject.Find("Player");
        lifeTime = player.GetComponent<PlayerController>().bulletDeathTime;
	}
	
	// Update is called once per frame
	void Update () {
        time += Time.deltaTime;
        if(time > lifeTime)
        {
            Destroy(gameObject);
        }
	}

}
