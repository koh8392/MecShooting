using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;


public class PurgeBooster : MonoBehaviour {

    [SerializeField] private Vector3 boostPurgeOffset; //ブースター切り離し時にどれだけ後方にスライドさせるか


    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void CollapseStart() {
        
        //ブースター自体を切り離し後方に移動開始
        StartCoroutine("SetCollapseCoroutine");

    }

    public IEnumerator SetCollapseCoroutine() {
        yield return new WaitForSeconds(1.0f);
        Vector3 boostPosition = GetComponent<Transform>().position;
        //ブースター自体を切り離し後方に移動開始
        gameObject.transform.DOLocalMove(new Vector3(boostPosition.y - boostPurgeOffset.x, boostPosition.y - boostPurgeOffset.y, boostPosition.z - boostPurgeOffset.z), 2.0f).SetEase(Ease.OutCirc);
        yield return new WaitForSeconds(1.0f);
        CollapseObject();
    }



    public void CollapseObject()
    {

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
            r.gameObject.AddComponent<AutoDestroy>().StartDestroyThis(3.0f);

            //破壊時にそれぞれの破片が飛ばされる方向をvector3で生成。
            var collapseDirection = new Vector3(random.Next(min, max), random.Next(min, max), random.Next(-8, 0));
            r.AddForce(collapseDirection, ForceMode.Impulse);
            r.AddTorque(collapseDirection, ForceMode.Impulse);
        });

        Destroy(gameObject);
    }
}
