using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour {

    public float scrollSpeed;
    public float lengthOfCamera;

	// Use this for initialization
	void Start () {
        Destroy(gameObject.GetComponent<EdgeCollider2D>(), lengthOfCamera / scrollSpeed);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
