using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controls : MonoBehaviour {
	BoardManager manager;
	Camera cam;
	int camSpeed = 30;
	int scrollSpeed_ = 10;

	// Use this for initialization
	void Start () {
		cam = Camera.main;
		manager = BoardManager.getInstance();
	}
	
	// Update is called once per frame
	void Update () {
		ControlCamera ();
		MouseControl ();
	}

	void ControlCamera()
	{
		if (Input.GetKey (KeyCode.UpArrow))
			cam.transform.Translate(0, Time.deltaTime*camSpeed, 0);
		if (Input.GetKey (KeyCode.DownArrow))
			cam.transform.Translate(0, Time.deltaTime*camSpeed*-1, 0);
		if (Input.GetKey (KeyCode.LeftArrow))
			cam.transform.Translate(Time.deltaTime*camSpeed*-1, 0, 0);
		if (Input.GetKey (KeyCode.RightArrow))
			cam.transform.Translate(Time.deltaTime*camSpeed, 0, 0);

		cam.transform.Translate (0, 0, Input.GetAxis("Mouse ScrollWheel")*scrollSpeed_);
	}

	void MouseControl()
	{
		if (Input.GetMouseButtonDown (0)) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;

			if (Physics.Raycast (ray, out hit, 100)) {
				TileIdentification tileIdHit = hit.collider.GetComponentInParent<TileIdentification>();
				Debug.Log (tileIdHit.x_ + " " + tileIdHit.y_);
				manager.Cleeck(tileIdHit.x_, tileIdHit.y_);
			}
		}
	}
}
