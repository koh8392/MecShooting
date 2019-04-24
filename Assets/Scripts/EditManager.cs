using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameParameters;
using UnityEngine.SceneManagement;

public class EditManager : MonoBehaviour {

    // Use this for initialization
    void Start () {

    }

    

    // Update is called once per frame
    void Update () {
		
	}

    public void LoadMainScene()
    {
        SceneManager.LoadScene("MainGame");
    }


}
