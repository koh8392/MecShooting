using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class EventTestEnter : MonoBehaviour {
    /*
    public void Sample()
    {
        EventTest eventTest = new EventTest();

        EventTest.DelegateType sampleDelegate = TestDelegateA;

        sampleDelegate += TestDelegateB;
        sampleDelegate += TestDelegateC;

        //Delegateを使用することで、他のイベントにメソッド自体を渡すことができる。
        //向こう側で取得せずともこちらからそのまま実行できる。
        eventTest.Sample(sampleDelegate);
    }

    public void TestDelegateA()
    {
        Debug.Log("処理完了");
    }

    public void TestDelegateB()
    {
        Debug.Log("処理完了");
    }

    public void TestDelegateC()
    {
        Debug.Log("処理完了");
    }

    // Use this for initialization
    void Start () {
        Sample();
	}
	
    */

/*
    public void Sample()
    {
        EventTest eventTest = new EventTest();

        UnityAction SampleAction = () => Debug.Log("処理完了");
        SampleAction += () => Debug.Log("処理完了B");
        SampleAction += () => Debug.Log("処理完了C");

        //Delegateを使用することで、他のイベントにメソッド自体を渡すことができる。
        //向こう側で取得せずともこちらからそのまま実行できる。
        eventTest.Sample(SampleAction);
    }

    // Use this for initialization
    void Start()
    {
        Sample();
    }
*/

    public void Sample()
    {
        EventTest eventTest = new EventTest();

        UnityAction SampleAction = () => Debug.Log("処理完了");
        SampleAction += () => Debug.Log("処理完了B");
        SampleAction += () => Debug.Log("処理完了C");

        //Delegateを使用することで、他のイベントにメソッド自体を渡すことができる。
        //向こう側で取得せずともこちらからそのまま実行できる。
        eventTest.Sample(SampleAction);
    }

    // Use this for initialization
    void Start()
    {
        Sample();
    }

    // Update is called once per frame
    void Update () {
		
	}
}
