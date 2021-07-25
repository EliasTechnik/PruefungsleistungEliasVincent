using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleGenerator : MonoBehaviour {

    // Diese Klasse ist verantwortlich für die Generierung sowie Löschung der Hindernisse.
    public int amount;
    // Die ObstacleList enthält alle möglichen Hindernisse, welche erzeugt werden können.
    public List<GameObject> ObstacleList; 

    private void Start() {
        TargetObject.Instance.OnRoundNumberChanged += Target_OnRoundNumberChanged;
    }

    private void Target_OnRoundNumberChanged(object sender, System.EventArgs e) {
        HandleObstacles();
    }

    // Zur Generierung werden alle Hindernisse der ObstacleList "amount-mal" durchlaufen und an einer zufälligen Position erzeugt. 
    public void GenerateObstacles() {
        foreach(GameObject obs in ObstacleList) {
            for (int i = 0; i < amount; i++) {
            Instantiate(obs, new Vector3(Random.Range(-50, 50),0, Random.Range(-50, 50)), Quaternion.identity);
            }
        }
    }

    // Alle Objekte innerhalb der Szene mit dem Tag "obstacle" werden in einem GameObject Array erfasst und anschließend gelöscht.
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
        // Testen der Generierung
        if (Input.GetKeyDown(KeyCode.G)) {
            DeleteOldObstacles();
            GenerateObstacles();
        }
    }

}
