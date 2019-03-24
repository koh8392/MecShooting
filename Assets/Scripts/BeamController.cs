using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BeamController : MonoBehaviour {


	// Use this for initialization
	void Awake () {
        Invoke("LaunchBeam", 3.0f + Random.Range(0.0f, 3.0f));
    }
	
    void LaunchBeam()
    {
        transform.DOMove(new Vector3(transform.position.x, transform.position.y, -100), 2.0f);
    }

	// Update is called once per frame
	void Update () {
		
	}
}
