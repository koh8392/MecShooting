using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class BeamManager : MonoBehaviour {

    private Transform beamManagerTransform;
    private Transform playerTransform;
    private GameObject beamBullet;

	// Use this for initialization
	void Start () {
        beamManagerTransform = GetComponent<Transform>();

        //弾丸のプレハブをロード
        beamBullet = (GameObject)Resources.Load("Prefabs/General/beam") as GameObject;

        GameObject.FindGameObjectsWithTag("Wingmate").ToList().ForEach(wingmates =>
        {
            Transform beamTransform = wingmates.GetComponent<Transform>();
            //Random.Range(-10.0f, 10.0f);

            //Debug.Log("ビームを呼び出します");
            //Debug.Log(wingmates.transform.position);

            GameObject beamInstance = Instantiate(beamBullet,
                new Vector3(wingmates.transform.position.x + Random.Range(-2.5f, 2.5f),
                wingmates.transform.position.y + Random.Range(-2.5f, 2.5f),
                beamManagerTransform.position.z)
                , beamManagerTransform.rotation) as GameObject;



        });

        playerTransform = GameObject.Find("Player").GetComponent<Transform>();

        GameObject beamPlayerInstance = Instantiate(beamBullet,
                new Vector3(playerTransform.transform.position.x + 8 + Random.Range(-1f, 1f),
                playerTransform.transform.position.y + 7 + Random.Range(-1f, 1f),
                beamManagerTransform.position.z), 
                beamManagerTransform.rotation) as GameObject;


    }
	
    /*
     ビームマネージャーの処理
     1.FindObjectwithtagでwingmateを取得し配列に格納しforeach文を実行
     2.wingmateのトランスフォーム(位置)を取得
     3.ランダムを用いて、ビームを呼び出す座標を生成。
     4.場所は、XYはwingmateの位置 +- ビームオブジェクトのサイズ/2=半径→必ず命中する位置 zは始点=ビームマネージャの方向

     5.instantiateでビームのオブジェクトを読み込み。場所は4から、回転はビームマネージャーの方向
     6.tweenさせたいのでDOmoveで4のzまで移動

     */


	// Update is called once per frame
	void Update () {
		
	}
}
