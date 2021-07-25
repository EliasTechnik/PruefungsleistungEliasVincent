using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;

public class PlayerObject : MonoBehaviour {

    private float maxSpeed = .05f;
    private float gain = 0.002f;
    private float friction = 0.995f;
    private Vector3 currentPosition;
    private Vector3 inertia;
    private Vector3 rayposition;
    private Vector3 targetdistance;
    private double currenttargetdistance;
    private double forwarddistance;
    private double backdistance;
    private double leftdistance;
    private double rightdistance;
    private GameObject playerobject;
    public bool shouldRespawn;

    public void HandleMovement(KeyCode input) {
        switch(input) {
            case KeyCode.W: 
                if((inertia.z+gain)<maxSpeed){
                inertia=new Vector3(inertia.x,inertia.y,inertia.z+gain);
                } break;
            case KeyCode.A:
                if((inertia.x-gain)>(maxSpeed*-1)){
                inertia=new Vector3(inertia.x-gain,inertia.y,inertia.z);
                } break;
            case KeyCode.S:
                if((inertia.z-gain)>(maxSpeed*-1)){
                inertia=new Vector3(inertia.x,inertia.y,inertia.z-gain);
                } break;
            case KeyCode.D:
                if((inertia.x+gain)<maxSpeed){
                inertia=new Vector3(inertia.x+gain,inertia.y,inertia.z);
                } break;
            }
    }
    public void UpdateMove() {
        currentPosition= currentPosition - inertia;
        /*
        if(Input.GetKey(KeyCode.W)){
            HandleMovement(KeyCode.W);
        }
        if(Input.GetKey(KeyCode.A)){
            HandleMovement(KeyCode.A);
        }
        if(Input.GetKey(KeyCode.S)){
            HandleMovement(KeyCode.S);
        }
        if(Input.GetKey(KeyCode.D)){
            HandleMovement(KeyCode.D);
        } */

        //zu Testzwecken genutzt
        if(Input.GetKey(KeyCode.R)) {
            RespawnPlayer();
        }

        inertia=new Vector3(inertia.x*friction,inertia.y*friction,inertia.z*friction);
        playerobject.transform.SetPositionAndRotation(currentPosition,new Quaternion(0,0.5f,0,0));
        rayposition = currentPosition; 

        TargetDistance();
    }

    public void RespawnPlayer() {
        currentPosition = new Vector3(UnityEngine.Random.Range(-100, 100),0,UnityEngine.Random.Range(-100,100));
        inertia=Vector3.zero;
    }

    private void Update() {
        if (shouldRespawn) {
            GameManager.Instance.respawnbool=true;
            shouldRespawn=false;
        }
        TargetDistance();
    }

    private void LateUpdate() {
        CheckDistance();
        Debug.Log(forwarddistance);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.name == "target_obj") {
            TargetObject.Instance.RespawnTarget();
        }
        if (other.tag == "obstacle" || other.tag == "border") {
            shouldRespawn=true;
        }
    }    
    public PlayerObject(GameObject _object) {
        playerobject = _object;
        currentPosition = playerobject.transform.position;
        inertia = new Vector3(0,0,0);
    }


    // Raycasting ermöglicht eine Distanzmessung vom Spielerobjekt bis zum nächsten Collider.
    private void CheckDistance() {
        int LayerMask = 1 << 6;

        RaycastHit hitforward;
        RaycastHit hitleft;
        RaycastHit hitright;
        RaycastHit hitback;

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hitforward, 15,LayerMask)) {
            forwarddistance = hitforward.distance;
            //Debug.Log(hitforward.distance);
        }

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.left), out hitleft, 15,LayerMask)) {
            rightdistance = hitleft.distance;
            //Debug.Log(hitleft.distance);
        }

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out hitright, 15,LayerMask)) {
            leftdistance = hitleft.distance;
            //Debug.Log(hitright.distance);
        }

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.back), out hitback, 15,LayerMask)) {
            backdistance = hitback.distance;
            //Debug.Log(hitback.distance);
        }    
    }

    //Liefert absolute Distanz von Target zu Spieler
    public void TargetDistance() { 
        targetdistance = TargetObject.Instance.transform.position - currentPosition;
        currenttargetdistance = Math.Abs(targetdistance.x) + Math.Abs(targetdistance.z);
        //Debug.Log(currenttargetdistance);
    }


    
}
