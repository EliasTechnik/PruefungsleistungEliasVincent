using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;

public class PlayerObject : MonoBehaviour {

    private const float speed = 1500f;
    private Vector3 moveDir;
    private Rigidbody rb;
    private float maxSpeed = .05f;
    private float gain = 0.002f;
    private float friction = 0.995f;
    private Vector3 currentPosition;
    private Vector3 inertia;
    private Vector3 rayposition;
    private Vector3 targetdistance;
    private double currenttargetdistance;
    private double forwarddistance, backdistance, leftdistance, rightdistance;
    private double forwardleftdistance, forwardrightdistance, backleftdistance, backrightdistance;
    private GameObject playerobject;
    public bool shouldRespawn;

    private void Start() {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;

        GameManager.Instance.ai_input.getFeatureByName("Wall_0").CurrentRawInput=forwarddistance;
        GameManager.Instance.ai_input.getFeatureByName("Wall_45").CurrentRawInput=forwardleftdistance;
        GameManager.Instance.ai_input.getFeatureByName("Wall_90").CurrentRawInput=leftdistance;
        GameManager.Instance.ai_input.getFeatureByName("Wall_135").CurrentRawInput=backleftdistance;
        GameManager.Instance.ai_input.getFeatureByName("Wall_180").CurrentRawInput=backdistance;
        GameManager.Instance.ai_input.getFeatureByName("Wall_225").CurrentRawInput=backrightdistance;
        GameManager.Instance.ai_input.getFeatureByName("Wall_270").CurrentRawInput=rightdistance;
        GameManager.Instance.ai_input.getFeatureByName("Wall_315").CurrentRawInput=forwardrightdistance;

        //GameManager.Instance.agent.Reward(GameManager.Instance.ai_input);
        GameManager.Instance.agent.PredictAndTrain(GameManager.Instance.ai_input);
    }

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

    private void HandleRigidbodyMovement() {
        float moveX = 0f;
        float moveZ = 0f;

        if (Input.GetKey(KeyCode.W)) {
            moveZ = .5f;
        }
        if (Input.GetKey(KeyCode.S)) {
            moveZ = -.5f;
        }
        if (Input.GetKey(KeyCode.A)) {
            moveX = -.5f;
        }
        if (Input.GetKey(KeyCode.D)) {
            moveX = .5f;
        }

        moveDir = new Vector3(moveX, 0 ,moveZ);
    }

    private void AIForward() {
        float moveX = 0f;
        float moveZ = 0f;

        moveZ = .5f;

        moveDir = new Vector3(moveX, 0, moveZ);
    }
    private void AILeft() {
        float moveX = 0f;
        float moveZ = 0f;

        moveX = -.5f;

        moveDir = new Vector3(moveX, 0, moveZ);
    }
    private void AIRight() {
        float moveX = 0f;
        float moveZ = 0f;

        moveX = .5f;

        moveDir = new Vector3(moveX, 0, moveZ);
    }
    private void AIBack() {
        float moveX = 0f;
        float moveZ = 0f;

        moveZ = -.5f;

        moveDir = new Vector3(moveX, 0, moveZ);
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

    private void Update() {
        if (shouldRespawn) {
            GameManager.Instance.respawnbool=true;
            shouldRespawn=false;
        } 
        TargetDistance();
        HandleRigidbodyMovement();

        GameManager.Instance.ai_input.getFeatureByName("Wall_0").CurrentRawInput=forwarddistance;
        GameManager.Instance.ai_input.getFeatureByName("Wall_45").CurrentRawInput=forwardleftdistance;
        GameManager.Instance.ai_input.getFeatureByName("Wall_90").CurrentRawInput=leftdistance;
        GameManager.Instance.ai_input.getFeatureByName("Wall_135").CurrentRawInput=backleftdistance;
        GameManager.Instance.ai_input.getFeatureByName("Wall_180").CurrentRawInput=backdistance;
        GameManager.Instance.ai_input.getFeatureByName("Wall_225").CurrentRawInput=backrightdistance;
        GameManager.Instance.ai_input.getFeatureByName("Wall_270").CurrentRawInput=rightdistance;
        GameManager.Instance.ai_input.getFeatureByName("Wall_315").CurrentRawInput=forwardrightdistance;

        GameManager.Instance.agent.Reward(GameManager.Instance.ai_input);
        AIAction a = GameManager.Instance.agent.PredictAndTrain(GameManager.Instance.ai_input);
        /*
        switch(a) {
            case 0: 
            AIForward();
            break;
            case 1:
            AILeft();
            break;
            case 2:
            AIRight();
            break;
            case 3:
            AIBack();
            break;
        } 
        */

    }

    private void LateUpdate() {
        CheckDistance();
        //Debug.Log(forwarddistance);
    }

    private void FixedUpdate() {
        //rb.velocity = (moveDir * speed * Time.fixedDeltaTime);
        rb.AddForce(moveDir * speed);
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

    public void RespawnPlayer() {
        currentPosition = new Vector3(UnityEngine.Random.Range(-100, 100),0.1f,UnityEngine.Random.Range(-100,100));
        inertia=Vector3.zero;
    }

    // Raycasting ermöglicht eine Distanzmessung vom Spielerobjekt bis zum nächsten Collider.
    // Entfernung ist 0 wenn kein Collider erfasst wird.
    private void CheckDistance() {
        int LayerMask = 1 << 6;

        RaycastHit hitforward, hitleft, hitright, hitback, hitforwardleft, hitforwardright, hitbackleft, hitbackright;

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hitforward, 500,LayerMask)) {
            forwarddistance = hitforward.distance;
        }

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.left), out hitleft, 500,LayerMask)) {
            rightdistance = hitleft.distance;
        }

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out hitright, 500,LayerMask)) {
            leftdistance = hitleft.distance;
        }

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.back), out hitback, 500,LayerMask)) {
            backdistance = hitback.distance;
        }

        if (Physics.Raycast(transform.position, transform.TransformDirection(new Vector3(-1,0,1)), out hitforwardleft, 500,LayerMask)) {
            forwardleftdistance = hitforwardleft.distance;
        }

        if (Physics.Raycast(transform.position, transform.TransformDirection(new Vector3(1,0,1)), out hitforwardright, 500,LayerMask)) {
            forwardrightdistance = hitforwardright.distance;
        }

        if (Physics.Raycast(transform.position, transform.TransformDirection(new Vector3(-1,0,-1)), out hitbackleft, 500,LayerMask)) {
            backleftdistance = hitbackleft.distance;
        }

        if (Physics.Raycast(transform.position, transform.TransformDirection(new Vector3(1,0,-1)), out hitbackright, 500,LayerMask)) {
            backrightdistance = hitbackright.distance;
        }
    }

    //Liefert absolute Distanz von Target zu Spieler
    public void TargetDistance() { 
        targetdistance = TargetObject.Instance.transform.position - currentPosition;
        currenttargetdistance = Math.Abs(targetdistance.x) + Math.Abs(targetdistance.z);
        //Debug.Log(currenttargetdistance);
    }
}
