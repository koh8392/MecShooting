using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUIController : MonoBehaviour {
    //位置合わせをしたいオブジェクト
    private GameObject targetObject;
    //位置合わせを取得したいオブジェクトのtransform
    Transform targetTransform;
    
    //UIが制御可能状態になっているかのフラグ
    private bool isUIActive;

    //UIの表示位置
    private RectTransform thisUIRectTransform;

    //表示位置がズレる際の調整値
    [SerializeField] private Vector3 offset;

    // Use this for initialization
    void Start () {
        //アタッチされたUIのrecttransformを取得
        thisUIRectTransform = GetComponent<RectTransform>();
	}

    //UIを追従状態にする。引数はオブジェクト名
    public void ActivateUI(GameObject targetObject)
    {
        //引数で渡されたgameObjectがnullになっていないか確認
        Debug.Log(targetObject.name);
        //位置合わせを取得したいオブジェクトのtransformを参照しておく
        targetTransform = targetObject.GetComponent<Transform>();
        //位置合わせ状態のフラグをオン
        isUIActive = true;
        //UIを表示する。
        GetComponent<CanvasGroup>().alpha = 1;
    }

    // Update is called once per frame
    void Update() {
        //追従状態であれば、毎フレーム対象オブジェクトとの位置合わせを行う。
        if (isUIActive == true)
        {
            thisUIRectTransform.anchoredPosition = RectTransformUtility.WorldToScreenPoint(Camera.main, targetTransform.position + offset);
        }
	}
}
