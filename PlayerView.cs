using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerView : MonoBehaviour {

	public float horizontalSpeed = 100f;
	public float verticalSpeed = 100f;
	public bool flipVertical = false;
	public float clampAngle = 80f;

	private float rotationX;
	private float rotationY;
	private Vector3 cameraTargetPosition;

	private float flipMultiplier = 1f;

	void Start() {
		if (flipVertical)
			flipMultiplier = -1;

		rotationX = transform.eulerAngles.x;
		rotationY = transform.eulerAngles.y;

		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	void LateUpdate() {		
		rotationY += horizontalSpeed * Input.GetAxis ("Mouse X") * Time.deltaTime;
		rotationX += flipMultiplier * verticalSpeed * Input.GetAxis ("Mouse Y") * Time.deltaTime;
		rotationX = Mathf.Clamp(rotationX, -clampAngle, clampAngle);

		var parentPosition = transform.InverseTransformPoint(transform.parent.position);
		transform.Translate(parentPosition);
		transform.rotation = Quaternion.Euler(rotationX, rotationY, 0f);
		transform.Translate(-parentPosition);
	}
}
