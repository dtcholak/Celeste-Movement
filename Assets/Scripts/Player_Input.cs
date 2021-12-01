using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Player_Input : MonoBehaviour
{

    [Space]
    [Header("Variables")]
        public Vector2 dir;
        public Vector2 dash_dir; 
        public bool jump_start;
        public bool jump_stop;

        public float x;
        private float y;
        public float xRaw;
        private float yRaw;
        private float dash_xRaw;
        private float dash_yRaw;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        x = Input.GetAxis("Horizontal"); //x direction variable input (smoothed)
        y = Input.GetAxis("Vertical"); //y direction variable input (smoothed)
        xRaw = Input.GetAxisRaw("Horizontal"); //raw x direction variable input 
        yRaw = Input.GetAxisRaw("Vertical"); //raw y direction variable input
        jump_start = Input.GetButtonDown("Jump"); //true if jump button press
        jump_stop = Input.GetButtonUp("Jump"); //true if jump button release

        dash_xRaw = Input.GetAxisRaw("Horizontal_Dash"); //raw x direction variable input 
        dash_yRaw = Input.GetAxisRaw("Vertical_Dash"); //raw y direction variable input

        dir.Set(xRaw,yRaw); //vector setting of raw x + y 
        dash_dir.Set(dash_xRaw,dash_yRaw);

    }

    
}
 

