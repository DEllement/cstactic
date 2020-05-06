using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameCamera : MonoBehaviour
{
    private const float Y_ANGLE_MIN = 0.0f;
    private const float Y_ANGLE_MAX = 89.0f;
    
    public Transform lookAt;
    private Camera cam;
    
    private float zoomSpeed = 1.0f;
    private float distance = 10.0f;
    private float currentX = 0.0f;
    private float currentY = 100.0f;
    private float sensivityX = 4.0f;
    private float sensivityY = 4.0f;
    
    private Quaternion lookAtTargetRotation = Quaternion.identity;
    
    private Vector2 input;
    
    // Start is called before the first frame update
    void Start()
    {
                
    }
    
    bool isZooming = false;
    // Update is called once per frame
    void Update()
    {
        //Zoom
        isZooming = Input.mouseScrollDelta.y != 0.0f;
        if(isZooming){
            distance += (Input.mouseScrollDelta.y * -zoomSpeed) * Time.deltaTime * 50;
            distance = Math.Max(5.0f, Math.Min( 50.0f , distance));
        }
        
        input = new Vector2(Input.GetAxis("Horizontal"),Input.GetAxis("Vertical"));
        input = Vector2.ClampMagnitude(input, 1);

        var camF = this.GetComponent<Transform>().forward;
        var camR = this.GetComponent<Transform>().right;
        camF.y = 0;
        camR.y = 0;
        camF = camF.normalized;
        camR = camR.normalized;
        
        lookAt.transform.position += (camF * input.y + camR * input.x ) * Time.deltaTime * 5;        
        
        //Rotation
        if(Input.GetKey(KeyCode.Mouse2)){
            currentX += Input.GetAxis("Mouse X");
            currentY += Input.GetAxis("Mouse Y");
        }
        currentY = Mathf.Clamp(currentY, Y_ANGLE_MIN, Y_ANGLE_MAX);
     
        var dir = new Vector3(0,0,-distance);
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        this.GetComponent<Transform>().position = lookAt.position + rotation * dir;
        /*this.GetComponent<Transform>().position = Vector3.Lerp(
            this.GetComponent<Transform>().position,
            lookAt.position+ rotation * dir,
            0.125f);*/
        this.GetComponent<Transform>().LookAt(lookAt.position);
    }
}
