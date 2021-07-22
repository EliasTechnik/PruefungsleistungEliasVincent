using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {

    [SerializeField] private PlayerObject Player;
    [SerializeField] private TargetObject Target;

    private Camera mainCamera;

    private RectTransform TargetPositionIndicator;

    private void Awake() {
        TargetPositionIndicator = transform.Find("TargetPositionIndicator").GetComponent<RectTransform>();
    }

    private void Start() {
        mainCamera = Camera.main;
    }

    private void TargetIndicator() {
        Vector3 dirToTargetPosition = -(Target.transform.position - mainCamera.transform.position).normalized;

        TargetPositionIndicator.eulerAngles = new Vector3(0,0,GetAngleFromVector(dirToTargetPosition));
    }


    //Converts vector coordinates to angle
    public static float GetAngleFromVector(Vector3 vector) {
        float radians = Mathf.Atan2(vector.y, vector.x);
        float degrees = radians * Mathf.Rad2Deg;
        return degrees; 
    }

    private void Update() {
        TargetIndicator();
    }

}
