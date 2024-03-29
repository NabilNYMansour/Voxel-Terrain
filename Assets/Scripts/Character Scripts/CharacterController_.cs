﻿/* 
 * author : jiankaiwang
 * description : The script provides you with basic operations of first personal control.
 * platform : Unity
 * date : 2017/12
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController_ : MonoBehaviour
{

    public float speed = 10.0f;
    private float translation;
    private float straffe;
    // Use this for initialization
    void Start()
    {
        // turn off the cursor
        Cursor.lockState = CursorLockMode.Locked;
    }


    // Update is called once per frame
    void Update()
    {
        // Input.GetAxis() is used to get the user's input
        // You can furthor set it on Unity. (Edit, Project Settings, Input)
        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed = 50;
        }
        else
        {
            speed = 10;
        }
        translation = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        straffe = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        // Changing the original script:
        transform.position += gameObject.transform.Find("Eyes").GetComponent<Camera>().transform.rotation * new Vector3(straffe, 0, translation);
        if (Input.GetKey(KeyCode.Space))
        {
            transform.position += Vector3.up * 0.1f;
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            transform.position += Vector3.down * 0.1f;
        }

        if (Input.GetKeyDown("escape"))
        {
            // turn on the cursor
            Cursor.lockState = CursorLockMode.None;
        }
    }
}