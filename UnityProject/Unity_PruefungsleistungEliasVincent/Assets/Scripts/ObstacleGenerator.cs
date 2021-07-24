using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleGenerator : MonoBehaviour {
    public int amount;
    [SerializeField] private TargetObject Target;
    public List<GameObject> ObstacleList;


    private void Start() {
        Target.OnRoundNumberChanged += Target_OnRoundNumberChanged;
    }

    private void Target_OnRoundNumberChanged(object sender, System.EventArgs e) {
        HandleObstacles();
    }

    public void GenerateObstacles() {

        foreach(GameObject obs in ObstacleList) {
            for (int i = 0; i < amount; i++) {
            Instantiate(obs, new Vector3(Random.Range(-50, 50),0, Random.Range(-50, 50)), Quaternion.identity);
            }
        }
    }

    private void DeleteOldObstacles() {
        GameObject[] obs = GameObject.FindGameObjectsWithTag("obstacle");

        for (int i = 0; i < obs.Length; i++) {
            Destroy(obs[i]);
        }

    }

    public void HandleObstacles() {
        DeleteOldObstacles();
        GenerateObstacles();
    }


    private void Update() {
        //For tests
        if (Input.GetKeyDown(KeyCode.G)) {
            DeleteOldObstacles();
            GenerateObstacles();
        }
    }

}
