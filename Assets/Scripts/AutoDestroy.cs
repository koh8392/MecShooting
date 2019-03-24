using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AutoDestroy : MonoBehaviour {

    private float waitTimeInstance;

	// Use this for initialization
	void Start () {
        
    }

    public void StartDestroyThis(float waitTime)
    {
        waitTimeInstance = waitTime;
        if (this.GetComponent<MeshRenderer>())
        {

            iTween.FadeTo(this.gameObject, iTween.Hash("alpha", 0, "time", waitTimeInstance));
            StartCoroutine("DestroyThis");
        }
        else { }
    }

    private IEnumerator DestroyThis ()
    {
        yield return new WaitForSeconds(waitTimeInstance);
        Destroy(this.gameObject);
    }

	// Update is called once per frame
	void Update () {
        //Color matColor = gameObject.GetComponent<MeshRenderer>().material.color;
        //gameObject.GetComponent<MeshRenderer>().material.SetColor("matColor", new Color(matColor.r, matColor.g, matColor.b, Mathf.Lerp(255.0f, 0.0f, 0.25f) / 255.0f));
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "EnemyBullet")
        {
            Destroy(this.gameObject);
        }
    }
}
