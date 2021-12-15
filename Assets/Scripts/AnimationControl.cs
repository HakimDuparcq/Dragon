using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationControl : MonoBehaviour
{
    Animator Animator;

    public float speed;
    public GameObject sol;

    public float speed_walk=6;
    public float speed_run=10;
    public float speed_start_fly=10;//take off
    public float speed_fly_forward = 12;
    public float speed_fly_glide = 10;//planer
    public float speed_land = 2;//atterrissage

    private void Start()
    {
        Animator = GetComponent<Animator>();
    }
    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        bool maj = (Input.GetKey(KeyCode.LeftShift));
        bool space = (Input.GetKey(KeyCode.Space));
        bool key_e = (Input.GetKey(KeyCode.E));
        bool key_f = (Input.GetKey(KeyCode.F));
        bool key_c = (Input.GetKey(KeyCode.C));

        if (horizontal != 0 || vertical != 0)
        {
            Animator.SetBool("isMoving", true);
            speed = speed_walk;
        }
        else
        {
            Animator.SetBool("isMoving", false);
        }

        if (Animator.GetBool("isMoving") && maj)
        {
            Animator.SetBool("isSprinting" , true);
            speed = speed_run;
        }
        else
        {
            Animator.SetBool("isSprinting", false);
        }

        if ( space )
        {
            Animator.SetBool("isFlying", true);

        }
        
        
        /*
        else
        {
            Animator.SetBool("isFlying", false);
        }
        */

        if (key_e)
        {
            Animator.SetBool("isAttack1", true);
        }
        else
        {
            Animator.SetBool("isAttack1", false);
        }

        if (key_f)
        {
            Animator.SetBool("isAttack2", true);
        }
        else
        {
            Animator.SetBool("isAttack2", false);
        }

        if (key_c)
        {
            Animator.SetBool("isAttack3", true);
        }
        else
        {
            Animator.SetBool("isAttack3", false);
        }














    }

    /*
    private void OnTriggerEnter(Collider other)
    {
        if (other==sol)
        {
            Debug.Log("sol");
        }
    }
    */

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log(other);
        if (other.name== "attérisage_cube")
        {
            Animator.SetBool("isFlying", false);
        }
    }

}

