using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class PlayerObject : MonoBehaviour {

    private float maxSpeed = .05f;
    private float gain = 0.002f;
    private float friction = 0.995f;
    private Vector3 currentPosition;
    private Vector3 inertia;
    private Vector3 rayposition;
    private GameObject playerobject;
    [SerializeField] private TargetObject Target;

    [SerializeField] private GameManager GM;
    public bool shouldRespawn;
    private bool collided;

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
        }

        //zu Testzwecken genutzt
        if(Input.GetKey(KeyCode.R)) {
            RespawnPlayer();
        }

        inertia=new Vector3(inertia.x*friction,inertia.y*friction,inertia.z*friction);
        playerobject.transform.SetPositionAndRotation(currentPosition,new Quaternion(0,0.5f,0,0));
        rayposition = currentPosition; 
    }

    public void RespawnPlayer() {
        currentPosition = new Vector3(Random.Range(-100, 100),0,Random.Range(-100,100));
        inertia=Vector3.zero;
    }

    private void Update() {
        if (shouldRespawn) {
            GM.respawnbool=true;
            shouldRespawn=false;
        }
    }

    private void LateUpdate() {
        CheckDistance();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.name == "target_obj") {
            Target.RespawnTarget();
        }
        if (other.tag == "obstacle" || other.tag == "border") {
            //Debug.Log("Hit!");
            shouldRespawn=true;
        }
    }    
    public PlayerObject(GameObject _object) {
        playerobject = _object;
        currentPosition = playerobject.transform.position;
        inertia = new Vector3(0,0,0);
    }

    //Raycasting

    private void CheckDistance() {

    RaycastHit hitforward;
    RaycastHit hitleft;
    RaycastHit hitright;
    RaycastHit hitback;

    if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hitforward, 50)) {
        //Debug.Log(hitforward.distance);
    }

    if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.left), out hitleft, 50)) {
        //Debug.Log(hitleft.distance);
    }

    if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out hitright, 50)) {
        //Debug.Log(hitright.distance);
    }

    if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.back), out hitback, 50)) {
        //Debug.Log(hitback.distance);
    }    
    }


    
}
