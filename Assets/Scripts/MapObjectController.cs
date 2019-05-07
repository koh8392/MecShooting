using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MapObjectController : MonoBehaviour {
    //ゲームエリアの座標
    float gameareaPositionZ;

    // Use this for initialization
    void Start() {

        //生成時にゲームエリアの座標を取得
        gameareaPositionZ = GameObject.Find("GameArea").GetComponent<Transform>().position.z;

        //randomは整数しか取り出せない
        var random = new System.Random();
        int min = -2;
        int max = 2;

        //GetCompornentで子オブジェクトを取得。配列に格納。
        //配列に入ってる分すべてに対して以下の処理を行う。
        Rigidbody r = gameObject.GetComponent<Rigidbody>();

        //破壊時にそれぞれの破片が飛ばされる方向をvector3で生成。
        var collapseDirection = new Vector3(random.Next(min, max), random.Next(min, max), random.Next(-50, 0));
        r.AddForce(collapseDirection, ForceMode.Impulse);
        r.AddTorque(collapseDirection, ForceMode.Impulse);

        }

    // Update is called once per frame
    void Update()
    {

        //最初に取得したゲームエリアの座標と現在の座標を比較し、ゲームエリアより後ろなら実行
        if (transform.position.z < gameareaPositionZ)
        {
            //ゲームオブジェクトの破棄
            Destroy(gameObject);
        }


    }

}
