using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class Ball : MonoBehaviour
{
    private Rigidbody2D rb;
    private GameObject arrowScript;

    private Vector2 bombAct = new Vector2(0, 0);

    private float velX;
    private float velY;
    private bool canShoot;
    private bool canLook;
    private float eulerZAngle;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //canShoot = true;
        //canLook = true;
        //eulerZAngle = -90;
        arrowScript = GameObject.Find("Arrow");
    }

    void Update()
    {
        //GoFire();
        //ChangeLook();
    }

    void ChangeLook()
    {
        if (canLook)
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                eulerZAngle -= 0.5f;
                transform.eulerAngles = new Vector3(0, 0, eulerZAngle);
            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                eulerZAngle += 0.5f;
                transform.eulerAngles = new Vector3(0, 0, eulerZAngle);
            }
        }
    }

    void GoFire()
    {
        if (Input.GetKeyDown(KeyCode.Space) && canShoot)
        {
            canShoot = false;
            canLook = false;
            rb.gravityScale = 75;
            arrowScript.SetActive(false);

            velX = rb.velocity.x;
            velY = rb.velocity.y;
            rb.velocity = transform.right * 1000.0f;
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if(col.gameObject.tag == "Bomb")
        {
            velX = rb.velocity.x;
            velY = rb.velocity.y;
            
            if (Math.Sign(velX) == 1)
            {
                bombAct.x = 1000;
            }
            else if (Math.Sign(velX) == -1)
            {
                bombAct.x = -1000;
            }

            if (Math.Sign(velY) == 1)
            {
                bombAct.y = 1000;
            }
            else if (Math.Sign(velY) == -1)
            {
                bombAct.y = -1000;
            }

            rb.velocity = new Vector2(velX, velY) + bombAct;
            col.gameObject.SetActive(false);
        }

        if(col.gameObject.tag == "Bubble")
        {
            col.gameObject.SetActive(false);
        }
    }
}
