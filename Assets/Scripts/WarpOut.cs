using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WarpOut : MonoBehaviour {
    [SerializeField] private GameObject PlayerUI;

    // Use this for initialization
    void Start () {
        transform.localScale = new Vector3(0.1f, 0.1f, 5.0f);
        transform.DOScale(new Vector3(1,1,1), 0.3f);
        PlayerUI.GetComponent<EnemyUIController>().ActivateUI(gameObject);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
