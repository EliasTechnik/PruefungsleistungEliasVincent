using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundManager : MonoBehaviour {

    public static RoundManager Instance { get; private set; }



    private int roundNumber;
    private float roundTimer;

    private void Awake() {
    Instance = this;    
    }

    private void Update() {
        roundTimer += Time.deltaTime;
    }

    public void SetRoundTimer(float time) {
        roundTimer = time;
    }

    public int GetRoundNumber() {
        return roundNumber;
    }

}
