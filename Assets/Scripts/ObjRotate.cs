using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ObjRotate : MonoBehaviour
{
    public float rotSpeed = 200;

    float rotX;
    float rotY;

    public bool useRotX;
    public bool useRotY;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");

        rotX = Mathf.Clamp(rotX, -80, 80);

        if(useRotX) rotX += my * rotSpeed * Time.deltaTime;
        if(useRotY) rotY += mx * rotSpeed * Time.deltaTime;

        transform.localEulerAngles = new Vector3(-rotX, rotY, 0);

    }
}
