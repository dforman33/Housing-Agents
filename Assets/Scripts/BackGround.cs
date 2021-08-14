using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGround : MonoBehaviour
{
    public Material planeMaterial; 

    // Start is called before the first frame update
    void Start()
    {
        var plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.transform.position = new Vector3(15,5,4);
        plane.transform.rotation = Quaternion.Euler(0,0,90);
        plane.transform.localScale = new Vector3(10,10,10);
        plane.GetComponent<MeshRenderer>().material = planeMaterial;
    }
}
