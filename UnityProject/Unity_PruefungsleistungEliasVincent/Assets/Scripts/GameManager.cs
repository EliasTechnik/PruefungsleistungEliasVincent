using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    private PlayerObject Player;
    [SerializeField] private TargetObject Target;
    [SerializeField] private GameObject Arrow;

    private void Start() {
        Player = new PlayerObject(GameObject.Find("player_obj"));
    }

    private void Update() {

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

}