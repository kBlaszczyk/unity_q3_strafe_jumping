using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerStrafeJumping : MonoBehaviour {

    	#region Variables
	private static float quakeScaleFactor = 1f / 26f;  // 1 m = 26 qunits
	private float maxSpeed = 320f * quakeScaleFactor;
	private float jumpSpeed = 270f * quakeScaleFactor;
	private float gravity = 800f * quakeScaleFactor;
	private float airAcceleration = 320f * quakeScaleFactor;
	private float groundAcceleration = 3200f * quakeScaleFactor;
	private float stopSpeed = 100f * quakeScaleFactor;
	private float friction = 6f;

	private Vector3 velocity = Vector3.zero;
	private Transform cameraTransform;
	private CharacterController characterController;
	#endregion

	void Start() {
		cameraTransform = transform.Find("Camera");
		characterController = GetComponent<CharacterController>();
	}

	void Update() {
		if (characterController.isGrounded || VerifyCharacterGroundedness()) {
			if (Input.GetButton("Jump")) {
				velocity.y = jumpSpeed;
				applyAcceleration(airAcceleration);
			} else {
				velocity.y = 0f;
				applyFriction();
				applyAcceleration(groundAcceleration);
				var targetPosition = transform.position + velocity * Time.deltaTime;
				var targetOnGround = DowncastFromPosition(targetPosition, 1.02f + characterController.skinWidth);
				if (!targetOnGround) {
					var targetCloseToGround = DowncastFromPosition(targetPosition, 1.3f);
					if (targetCloseToGround)
						velocity.y -= 0.3f / Time.deltaTime;
				}
			}
		} else {
			velocity.y -= gravity * Time.deltaTime;
			applyAcceleration(airAcceleration);
		}
		
		characterController.Move(velocity * Time.deltaTime);
	}

	private void applyFriction() {
		var speed = velocity.magnitude;
		if (speed > 0) {
			var control = speed < stopSpeed ? stopSpeed : speed;
			var frictionDrop = control * friction * Time.deltaTime;
			velocity *= Mathf.Max(speed - frictionDrop, 0f) / speed;
		}
	}

	private void applyAcceleration(float accelerationValue) {
		var accelerationDirection = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")).normalized;
		var yRotation = Quaternion.Euler(0f, cameraTransform.eulerAngles.y, 0f);
		accelerationDirection = yRotation * accelerationDirection;

		Debug.DrawLine(transform.position, transform.position + accelerationDirection, Color.red);

		var speedTowardsAcceleration = Vector3.Dot(velocity, accelerationDirection);

		if (speedTowardsAcceleration < maxSpeed) {
			float speedGain = accelerationValue * Time.deltaTime;
			velocity += accelerationDirection * Mathf.Min(maxSpeed - speedTowardsAcceleration, speedGain);
		}
	}

	private bool VerifyCharacterGroundedness() {
		return DowncastFromPosition(transform.position, 1.02f + characterController.skinWidth);
	}

	private bool DowncastFromPosition(Vector3 position, float maxDistance) {
		if (Physics.Raycast(position, Vector3.down, maxDistance))
			return true;
		else
			return false;
	}
}
