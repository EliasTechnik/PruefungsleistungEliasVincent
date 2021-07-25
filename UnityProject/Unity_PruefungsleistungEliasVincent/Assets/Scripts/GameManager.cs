using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager Instance {get; private set;}

    private PlayerObject Player;
    private bool inputai=false;
    [SerializeField] private TargetObject Target;
    [SerializeField] private GameObject Arrow;
    [SerializeField] private ObstacleGenerator obstacleGenerator;

    public bool respawnbool;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        Player = new PlayerObject(GameObject.Find("player_obj"));
        Player.RespawnPlayer();
        obstacleGenerator.HandleObstacles();
    }

    private void Update() {

        if (inputai) {
            //switch(id) {}
        } else {
            if (Input.GetKey(KeyCode.W)) {
            Player.HandleMovement(KeyCode.W);
            }
            if (Input.GetKey(KeyCode.S)) {
            Player.HandleMovement(KeyCode.S);
            }
            if (Input.GetKey(KeyCode.A)) {
            Player.HandleMovement(KeyCode.A);
            }
            if (Input.GetKey(KeyCode.D)) {
            Player.HandleMovement(KeyCode.D);
            }
        }
        

        if (respawnbool) {
            obstacleGenerator.HandleObstacles();
            Player.RespawnPlayer();
            Player.shouldRespawn=false;
            respawnbool = false;
        }
        Player.UpdateMove();
        UpdateArrows();
    }

        private void UpdateArrows(){
        Vector3 tp=Target.transform.position;
        tp.y+=0.6f;
        Vector3 direction=(tp-Arrow.transform.position).normalized;
        Quaternion lookRotation=Quaternion.LookRotation(-direction);
        Arrow.transform.rotation=Quaternion.Slerp(Arrow.transform.rotation,lookRotation,1);
    }

    public void toggleAI() {
        switch(inputai) {
            case true:
            inputai = false;
            break;
            case false:
            inputai = true;
            break;
        }
    }
}
