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
    [SerializeField] private GameObject Arrow;
    public bool shouldRespawn;

    private double lowerRef;
    private double higherRef;
    private double distanceGain=0;

    //Angel 
    float targetArrowRotation=0;

    private void Start() {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        TargetDistance();
        lowerRef = currenttargetdistance-1;
        higherRef = currenttargetdistance+1;

        GameManager.Instance.ai_input.getFeatureByName("Wall_0").CurrentRawInput=forwarddistance;
        GameManager.Instance.ai_input.getFeatureByName("Wall_45").CurrentRawInput=forwardleftdistance;
        GameManager.Instance.ai_input.getFeatureByName("Wall_90").CurrentRawInput=leftdistance;
        GameManager.Instance.ai_input.getFeatureByName("Wall_135").CurrentRawInput=backleftdistance;
        GameManager.Instance.ai_input.getFeatureByName("Wall_180").CurrentRawInput=backdistance;
        GameManager.Instance.ai_input.getFeatureByName("Wall_225").CurrentRawInput=backrightdistance;
        GameManager.Instance.ai_input.getFeatureByName("Wall_270").CurrentRawInput=rightdistance;
        GameManager.Instance.ai_input.getFeatureByName("Wall_315").CurrentRawInput=forwardrightdistance;
        //GameManager.Instance.ai_input.getFeatureByName("Target").CurrentRawInput=distanceGain;
        GameManager.Instance.ai_input.getFeatureByName("Angle").CurrentRawInput=targetArrowRotation;
        if(GameManager.Instance.ai_input.getFeatureByName("Angle")==null){
            Debug.Log("NULL OBJECT");
        }

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
    }

    private void Update() {
        if (shouldRespawn) {
            GameManager.Instance.respawnbool=true;
            shouldRespawn=false;
        } 
        TargetDistance();
        HandleRigidbodyMovement();
        UpdateArrows();

        targetdistance = TargetObject.Instance.transform.position - transform.position;
        double testtargetdistance = Math.Abs(targetdistance.x) + Math.Abs(targetdistance.z);


        if (testtargetdistance < lowerRef) {
            distanceGain=1;
            //Debug.Log(distanceGain + "positiv gain");
            lowerRef = testtargetdistance-1;
            higherRef = testtargetdistance+1;
        }
        if (testtargetdistance > higherRef) {
            distanceGain=3;
            //Debug.Log(distanceGain + "negativ gain");
            higherRef = testtargetdistance+1;
            lowerRef = testtargetdistance-1;
        }

        //Debug.Log(testtargetdistance + " targetdist");
        //Debug.Log(lowerRef + "low");
        //Debug.Log(higherRef + "high");

        GameManager.Instance.ai_input.getFeatureByName("Wall_0").CurrentRawInput=forwarddistance;
        GameManager.Instance.ai_input.getFeatureByName("Wall_45").CurrentRawInput=forwardleftdistance;
        GameManager.Instance.ai_input.getFeatureByName("Wall_90").CurrentRawInput=leftdistance;
        GameManager.Instance.ai_input.getFeatureByName("Wall_135").CurrentRawInput=backleftdistance;
        GameManager.Instance.ai_input.getFeatureByName("Wall_180").CurrentRawInput=backdistance;
        GameManager.Instance.ai_input.getFeatureByName("Wall_225").CurrentRawInput=backrightdistance;
        GameManager.Instance.ai_input.getFeatureByName("Wall_270").CurrentRawInput=rightdistance;
        GameManager.Instance.ai_input.getFeatureByName("Wall_315").CurrentRawInput=forwardrightdistance;
        //GameManager.Instance.ai_input.getFeatureByName("Target").CurrentRawInput=distanceGain;
        GameManager.Instance.ai_input.getFeatureByName("Angle").CurrentRawInput=targetArrowRotation;

        GameManager.Instance.agent.Reward(GameManager.Instance.ai_input);
        AIAction a = GameManager.Instance.agent.PredictAndTrain(GameManager.Instance.ai_input);
        
        switch(a.ActionID) {
            case 0: 
            AIForward();
            break;
            case 1:
            AILeft();
            break;
            case 2:
            AIBack();
            break;
            case 3:
            AIRight();
            break;
        } 
        
        
        //Debug.Log(distanceGain);
        distanceGain=0; 
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
            //Debug.Log(forwarddistance);
            Debug.DrawRay(transform.position, Vector3.forward * 100, Color.green);
        }

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.left), out hitleft, 500,LayerMask)) {
            leftdistance = hitleft.distance;
            //Debug.Log(leftdistance);
            Debug.DrawRay(transform.position, Vector3.left * 100, Color.green);
        }

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out hitright, 500,LayerMask)) {
            rightdistance = hitleft.distance;
            //Debug.Log(rightdistance);
            Debug.DrawRay(transform.position, Vector3.right * 100, Color.green);
        }

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.back), out hitback, 500,LayerMask)) {
            backdistance = hitback.distance;
            //Debug.Log(backdistance);
            Debug.DrawRay(transform.position, Vector3.back * 100, Color.green);
        }

        if (Physics.Raycast(transform.position, transform.TransformDirection(new Vector3(-1,0,1)), out hitforwardleft, 500,LayerMask)) {
            forwardleftdistance = hitforwardleft.distance;
            //Debug.Log(forwardleftdistance);
            Debug.DrawRay(transform.position, new Vector3(-1,0,1) * 100, Color.green);
        }

        if (Physics.Raycast(transform.position, transform.TransformDirection(new Vector3(1,0,1)), out hitforwardright, 500,LayerMask)) {
            forwardrightdistance = hitforwardright.distance;
            //Debug.Log(forwardrightdistance);
            Debug.DrawRay(transform.position, new Vector3(1,0,1) * 100, Color.green);
        }

        if (Physics.Raycast(transform.position, transform.TransformDirection(new Vector3(-1,0,-1)), out hitbackleft, 500,LayerMask)) {
            backleftdistance = hitbackleft.distance;
            //Debug.Log(backleftdistance);
            Debug.DrawRay(transform.position, new Vector3(-1,0,-1) * 100, Color.green);
        }

        if (Physics.Raycast(transform.position, transform.TransformDirection(new Vector3(1,0,-1)), out hitbackright, 500,LayerMask)) {
            backrightdistance = hitbackright.distance;
            //Debug.Log(backrightdistance);
            Debug.DrawRay(transform.position, new Vector3(1,0,-1) * 100, Color.green);
        }
    }

    //Liefert absolute Distanz von Target zu Spieler
    public void TargetDistance() { 
        targetdistance = TargetObject.Instance.transform.position - currentPosition;
        currenttargetdistance = Math.Abs(targetdistance.x) + Math.Abs(targetdistance.z);
    }

    public void UpdateArrows(){
        Vector3 tp=TargetObject.Instance.transform.position;
        tp.y+=0.6f;
        Vector3 direction=(tp-Arrow.transform.position).normalized;
        Quaternion lookRotation=Quaternion.LookRotation(-direction);
        Arrow.transform.rotation=Quaternion.Slerp(Arrow.transform.rotation,lookRotation,1);
        targetArrowRotation=180-lookRotation.eulerAngles.y;
    }


}