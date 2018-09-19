using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptUserInput : MonoBehaviour {
	Vector3 inst_mouse_position = Vector3.zero;

	private const float cam_scope_speed = 100f;
	private const float cam_pan_speed = 20f;
	private const float cam_rotate_speed = 30f;

	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButton (1)) {
			float dx = (Input.mousePosition.x - inst_mouse_position.x) / Screen.width;
			float dy = (Input.mousePosition.y - inst_mouse_position.y) / Screen.height;
			transform.eulerAngles += new Vector3(Mathf.Clamp(dy, -2f, 2f) * -200f * Time.deltaTime, Mathf.Clamp(dx, -2f, 2f) * 200f * Time.deltaTime, 0f) * cam_rotate_speed;
		}

		inst_mouse_position = Input.mousePosition;

		Vector3 cam_pos = transform.position;
		if (Input.GetAxis ("Mouse ScrollWheel") != 0f)
			cam_pos.y -= Time.deltaTime * cam_scope_speed * Mathf.Sign(Input.GetAxis("Mouse ScrollWheel"));
		if (Input.GetKey (KeyCode.LeftArrow))
			cam_pos -= Vector3.ProjectOnPlane (transform.right, Vector3.up).normalized * Time.deltaTime * cam_pan_speed;
		if (Input.GetKey (KeyCode.RightArrow))
			cam_pos += Vector3.ProjectOnPlane (transform.right, Vector3.up).normalized * Time.deltaTime * cam_pan_speed;
		if (Input.GetKey (KeyCode.UpArrow))
			cam_pos += Vector3.ProjectOnPlane (transform.forward, Vector3.up).normalized * Time.deltaTime * cam_pan_speed;
		if (Input.GetKey (KeyCode.DownArrow))
			cam_pos -= Vector3.ProjectOnPlane (transform.forward, Vector3.up).normalized * Time.deltaTime * cam_pan_speed;
		transform.position = cam_pos;
	}
}