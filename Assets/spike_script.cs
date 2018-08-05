using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spike_script : MonoBehaviour {

	public GameObject gamecontroller;

	// Use this for initialization
	void Start () {
		Destroy(gameObject,5);
	}
	
	// Update is called once per frame
	void Update () {
		gameObject.transform.position += Vector3.down * Time.deltaTime * 3f;
	}
}
