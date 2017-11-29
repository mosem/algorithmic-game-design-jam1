﻿using UnityEngine;
using System;

namespace Flight {
[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour {

	[Serializable]
    public class ClampRange {
        public float min;
        public bool clampMin;
        public float max;
        public bool clampMax;
        public float clampSoftness = 50f;

        /// <summary>
        /// Soft clamp the value.
        /// </summary>
        public float Clamp(float value) {
            if (clampMin && value < min) {
                var diff = value - min;
                return min + diff / (clampSoftness - diff);
            } else if (clampMax && value > max) {
                var diff = value - max;
                return max + diff / (clampSoftness + diff);
            }
            return value;
        }
    }

	[Tooltip("The target that is moving.")]
    [SerializeField] Transform target;
    [Tooltip("How soft does the camera react to the changing position.\n" +
        "0 means the camera will always be at the target's X and the lookahead's " +
        "Y (not a very comfortable experience).")]
    [SerializeField] float snapLooseness = 0.0f;
    [Tooltip("The offset of the camera from the target (at X) and the lookahead (at Y).\n" +
        "Z will be automatically set to be the initial distance of the target " +
        "from the camera.")]
    [SerializeField] Vector3 offset;
    [Tooltip("How to clamp the camera's position on the X axis.")]
    [SerializeField] ClampRange clampX;
    [Tooltip("How to clamp the camera's position on the Y axis.")]
    [SerializeField] ClampRange clampY;

    // Only used to pass to SmoothDamp.
    private Vector3 velocity = Vector3.zero;

    protected void Awake() {
        var cameraComponent = GetComponent<Camera>();
        offset.z = cameraComponent.transform.position.z - target.position.z;
    }

    protected void LateUpdate() {
        if (target) {
            // Calculate position.
            var targetPosition = new Vector3(target.position.x, target.position.y, 0.0f) + offset;

            // Soft clamp the position.
            targetPosition.x = clampX.Clamp(targetPosition.x);
            targetPosition.y = clampY.Clamp(targetPosition.y);

            // Damp and set position.
            transform.position = Vector3.SmoothDamp(
                transform.position,
                targetPosition,
                ref velocity,
                snapLooseness,
                Mathf.Infinity,
                Time.unscaledDeltaTime);
        }
    }
//    public GameObject player;       //Public variable to store a reference to the player game object
//
//	[SerializeField] Transform target;
//
//    private Vector3 offset;         //Private variable to store the offset distance between the player and camera
//
//    // Use this for initialization
//    void Start () 
//    {
//        //Calculate and store the offset value by getting the distance between the player's position and camera's position.
//        offset = transform.position - player.transform.position;
//    }
//    
//    // LateUpdate is called after Update each frame
//    void LateUpdate () 
//    {
//        // Set the position of the camera's transform to be the same as the player's, but offset by the calculated offset distance.
//        transform.position = player.transform.position + offset;
//    }
}

}
