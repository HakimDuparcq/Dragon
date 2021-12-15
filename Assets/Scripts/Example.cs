using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Example : MonoBehaviour
{
    float horizontal = Input.GetAxis("Horizontal");
    float vertical = Input.GetAxis("Vertical");
    float speed = 5.0f;

    void Update()
    {
        transform.position = new Vector3(horizontal, 0, vertical) * speed * Time.deltaTime;
    }
}