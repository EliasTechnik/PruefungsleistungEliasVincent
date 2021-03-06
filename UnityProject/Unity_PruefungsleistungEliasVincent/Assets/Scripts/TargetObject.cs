using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TargetObject : MonoBehaviour {

    public static TargetObject Instance {get; private set;}

    private int roundNumber;
    private float roundTimer;
    private float totalTime;

    public event EventHandler OnRoundNumberChanged;

    private void Awake() {
        Instance = this;
    }

    private void Update() {
        roundTimer += Time.deltaTime;
        totalTime += Time.deltaTime;
    }
    public void RespawnTarget() {
        this.transform.position = new Vector3 (UnityEngine.Random.Range(-20, 20), 0, UnityEngine.Random.Range(-20, 20));
        roundNumber++;
        OnRoundNumberChanged?.Invoke(this, EventArgs.Empty);
    }

    public int GetRoundNumber() {
        return roundNumber;
    }

    public float GetRoundTimer() {
        return roundTimer;
    }

    public void SetRoundTimer(float time) {
        roundTimer = time;
    }

    public float GetAverageTime() {
        return totalTime/(roundNumber+1);
    }
}
