using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCreate : MonoBehaviour
{
    public GameObject obj;
    public float scale = 2; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out hit, 150.0f))
            {
                if (hit.normal.x == 0 || hit.normal.y == 0 || hit.normal.z == 0)
                {
                    Vector3 pos = hit.transform.position + hit.normal * scale;
                    Quaternion rot = Quaternion.identity;
                    GameObject newObj = Instantiate(obj, pos, rot);
                    newObj.transform.position = pos;
                }
            }    
        }

        else if (Physics.Raycast(ray, out hit, 300.0f) && Input.GetMouseButtonDown(1))
        {
            if (hit.collider.gameObject != null)
                Destroy(hit.collider.gameObject);
        }
        
    }
}
