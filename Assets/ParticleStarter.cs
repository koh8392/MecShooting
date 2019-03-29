using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ParticleStarter : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void StartParticle()
    {
        //子オブジェクトの中のパーティクルシステムを全て取得
        gameObject.GetComponentsInChildren<ParticleSystem>().ToList().ForEach(particleChildren => {
            //再生
            particleChildren.Play();

        });

    }
}
