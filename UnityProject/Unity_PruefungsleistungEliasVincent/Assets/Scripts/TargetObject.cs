using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetObject : MonoBehaviour {

    [SerializeField] private TargetObject Target;

    public void RespawnTarget() {
        Target.transform.position = new Vector3 (Random.Range(-20, 20), 0, Random.Range(-20, 20));
    }


}
