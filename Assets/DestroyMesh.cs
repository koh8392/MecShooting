using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DestroyMesh : MonoBehaviour {
    //private Rigidbody[] partsRigidbodyArray;

	// Use this for initialization
	void Start () {
        //StartCoroutine("SetCollapse");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public IEnumerator SetCollapse() {
        Debug.Log("破壊処理を開始します");
        yield return new WaitForSeconds(3.0f);
        CollapseObject();
    }



    public void CollapseObject()
    {
        Debug.Log("破壊処理を開始します");

        //randomは整数しか取り出せない
        var random = new System.Random();
        int min = -5;
        int max = 5;

        //GetCompornentで子オブジェクトを取得。配列に格納。
        //配列に入ってる分すべてに対して以下の処理を行う。
        gameObject.GetComponentsInChildren<Rigidbody>().ToList().ForEach(r => {
            //リジッドボディを動けるように設定。
            r.isKinematic = false;

            //親オブジェクトをnullに設定し解除。
            r.transform.SetParent(null);

            //既に自作している自動でn秒後にオブジェクトを廃棄するスクリプトを添付し２秒に設定。
            //r.gameObject.AddComponent<AutoDestroy>().time = 2f;

            //破壊時にそれぞれの破片が飛ばされる方向をvector3で生成。
            var collapseDirection = new Vector3(random.Next(min, max), random.Next(min, max), random.Next(-8, 0));
            r.AddForce(collapseDirection, ForceMode.Impulse);
            r.AddTorque(collapseDirection, ForceMode.Impulse);
        });
        Destroy(gameObject);
    }
}
