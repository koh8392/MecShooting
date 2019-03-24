using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class meshFadeout : MonoBehaviour {

	// Use this for initialization
	void Start () {
        iTween.FadeTo(this.gameObject, iTween.Hash("alpha", 0, "time", 2));
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
