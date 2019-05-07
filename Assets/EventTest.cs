using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventTest : MonoBehaviour {
    /*
    //デリゲートの型を設定
    public delegate void DelegateType();

    //Delegateは特定の変数への参照を取っている。

    //デリゲートを渡されて呼ばれる関数
    public void Sample(DelegateType SampleDelegate)
    {
        SampleDelegate(); //SampleDelegate内に入っている関数は引数で渡されたデリゲートによって変わる。
    }
    */


        
    //デリゲートを渡されて呼ばれる関数
    public void Sample(UnityAction SampleAction)
    {
        SampleAction(); //SampleDelegate内に入っている関数は引数で渡されたデリゲートによって変わる。
    }


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
