using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DelayScript : MonoBehaviour {

    //引数に遅延時間と実行したい処理をAction型で渡すことで遅延が実装できる。
    public IEnumerator Delay(float waitTime, UnityAction Action)
    {
        yield return new WaitForSeconds(waitTime);
        Action();
    }
}
