using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;


public class WingmateController : MonoBehaviour {

    private GameObject Trail;

    // Use this for initialization
    void Start () {
        Vector3 wingmatePos = GetComponent<Transform>().position;

        float DestroyTime = 5.0f + Random.Range(0.0f, 1.0f);
        transform.DOMove(new Vector3(wingmatePos.x, wingmatePos.y, 50.0f), DestroyTime);
        //Invoke("DestroyMesh", DestroyTime);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "EnemyBullet")
        {
            //FadeOutMesh();
            DestroyMesh();
            //Debug.Log("Hit" + other.gameObject.name);
        }
    }

    void FadeOutMesh()
    {
        //randomは整数しか取り出せない
        var random = new System.Random();
        int min = -5;
        int max = 5;

        //GetCompornentで子オブジェクトを取得。配列に格納。
        //配列に入ってる分すべてに対して以下の処理を行う。
        gameObject.GetComponentsInChildren<Rigidbody>().ToList().ForEach(r =>
        {

            //既に自作している自動でn秒後にオブジェクトを廃棄するスクリプトを添付しn秒に設定。
            r.gameObject.AddComponent<AutoDestroy>().StartDestroyThis(3.0f);

            //破壊時にそれぞれの破片が飛ばされる方向をvector3で生成。
        });

        gameObject.GetComponent<TrailRenderer>().time = 0;



        Destroy(gameObject);
    }

    void DestroyMesh()
    {
        //randomは整数しか取り出せない
        var random = new System.Random();
        int min = -5;
        int max = 5;

        //GetCompornentで子オブジェクトを取得。配列に格納。
        //配列に入ってる分すべてに対して以下の処理を行う。
        gameObject.GetComponentsInChildren<Rigidbody>().ToList().ForEach(r =>
        {
            //リジッドボディを動けるように設定。
            r.isKinematic = false;

            //親オブジェクトをnullに設定し解除。
            r.transform.SetParent(null);

            //既に自作している自動でn秒後にオブジェクトを廃棄するスクリプトを添付しn秒に設定。
            r.gameObject.AddComponent<AutoDestroy>().StartDestroyThis(5.0f);

            //破壊時にそれぞれの破片が飛ばされる方向をvector3で生成。
            var collapseDirection = new Vector3(random.Next(min, max), random.Next(min, max), random.Next(-8, 0));
            r.AddForce(collapseDirection, ForceMode.Impulse);
            r.AddTorque(collapseDirection, ForceMode.Impulse);

            //iTween.FadeTo(r.gameObject, iTween.Hash("alpha", 0, "time", 2, "delay", 0.5));
        });

        gameObject.GetComponent<TrailRenderer>().time = 0;
        


        Destroy(gameObject);
    }
}
