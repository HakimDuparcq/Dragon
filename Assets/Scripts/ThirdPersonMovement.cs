using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    public CharacterController controller;
    public Transform cam;
    //public float speed = 6f;

    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    public AnimationControl script;



    void Update()
    {
        float speed = script.speed;
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        bool space = (Input.GetKey(KeyCode.Space));
        bool ctrl = (Input.GetKey(KeyCode.LeftControl));


        float isGoUp = Convert.ToSingle(space) - Convert.ToSingle(ctrl);






        Vector3 direction = new Vector3(horizontal, isGoUp , vertical).normalized; //0f

        if (direction.magnitude >= 0.1f)  //direction.magnitude >= 0.1f
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg +cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

           
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward + new Vector3(0, isGoUp, 0);
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }
    }
}
