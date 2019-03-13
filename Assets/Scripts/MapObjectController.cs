using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MapObjectController : MonoBehaviour {

    // Use this for initialization
    void Start() {
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
	void Update ()
        {

        }

}
