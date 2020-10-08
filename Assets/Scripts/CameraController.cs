using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    private Vector2 movement;
    private Vector2 rotation;
    private float altitude;
    private Rigidbody rb;
    private Camera cam;

    private float moveSpeed = 20f; //units/s
    //private float rotateSpeed = 2.5f; //degrees/s

    public void Start()
    {
        rb = GetComponent<Rigidbody>();
        cam = GetComponent<Camera>();
    }

    public void FixedUpdate()
    {
        /*
        Mouse mouse = Mouse.current;
        if(mouse == null)
            return;
        
        //Set rotation
        Quaternion currRot = rb.rotation;
        Quaternion nextRot = new Quaternion();
        Ray ray = cam.ScreenPointToRay(mouse.position.ReadValue());
        nextRot = Quaternion.LookRotation(ray.direction);
        rb.MoveRotation(Quaternion.Lerp(currRot, nextRot, Time.fixedDeltaTime * rotateSpeed));
        */

        //Set velocity
        Vector3 vel = new Vector3();
        vel = transform.right * movement.x * moveSpeed;
        vel += transform.forward * movement.y * moveSpeed;
        vel += transform.up * altitude * moveSpeed;
        rb.velocity = vel;
    }

    //Move controlled by the Input Controller
    public void Move(InputAction.CallbackContext context)
    {
        movement = context.ReadValue<Vector2>();
    }

    //Altitude controlled by the input controller
    public void Altitude(InputAction.CallbackContext context)
    {
        altitude = context.ReadValue<float>();
    }

    public void Rotate(InputAction.CallbackContext context)
    {
        rotation = context.ReadValue<Vector2>();
    }

}
