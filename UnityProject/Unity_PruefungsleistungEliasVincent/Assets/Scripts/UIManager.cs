using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour {

    [SerializeField] private PlayerObject Player;
    [SerializeField] private TargetObject Target;

    private Camera mainCamera;
    private float roundTimer;
    private float averageTime;

    private RectTransform TargetPositionIndicator;
    private TextMeshProUGUI roundNumberText;
    private TextMeshProUGUI roundTimerText;
    private TextMeshProUGUI averageTimeText;

    private void Awake() {
        TargetPositionIndicator = transform.Find("TargetPositionIndicator").GetComponent<RectTransform>();
        roundNumberText = transform.Find("RoundNumberText").GetComponent<TextMeshProUGUI>();
        roundTimerText = transform.Find("RoundTimerText").GetComponent<TextMeshProUGUI>();
        averageTimeText = transform.Find("AverageTimeText").GetComponent<TextMeshProUGUI>();
    }

    private void Start() {
        mainCamera = Camera.main;
        Target.OnRoundNumberChanged += Target_OnRoundNumberChanged;
    }

    private void Target_OnRoundNumberChanged(object sender, System.EventArgs e) {
        SetRoundNumberText("Round "+ Target.GetRoundNumber());
        Target.SetRoundTimer(0);
    }

    private void HandleRoundTimer() {
        roundTimer = Target.GetRoundTimer();
        SetRoundTimerText("Current Round: "+ roundTimer.ToString("F2"));
    }

    private void HandleAverageTime() {
        averageTime = Target.GetAverageTime();
        SetAverageTimeText("Average Time: " + averageTime.ToString("F2"));
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
        HandleRoundTimer();
        HandleAverageTime();
    }

    private void SetRoundNumberText(string text) {
        roundNumberText.SetText(text);
    }

    private void SetRoundTimerText(string text) {
        roundTimerText.SetText(text);
    }

    private void SetAverageTimeText(string text) {
        averageTimeText.SetText(text);
    }

}
