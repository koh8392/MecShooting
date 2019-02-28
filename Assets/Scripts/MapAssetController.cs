using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapAssetController : MonoBehaviour {
    [SerializeField] private float rotateAngleX;
    [SerializeField] private float rotateAngleY;
    [SerializeField] private float rotateAngleZ;


    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(new Vector3(rotateAngleX, rotateAngleY, rotateAngleZ) * Time.deltaTime, Space.Self);
	}
}
