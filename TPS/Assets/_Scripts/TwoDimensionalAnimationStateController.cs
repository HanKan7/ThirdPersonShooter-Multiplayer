using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TwoDimensionalAnimationStateController : MonoBehaviourPunCallbacks
{
    [SerializeField]
    Animator anim;
    public CharacterController cc;

    float velocityZ = 0f, velocityX = 0;
    public float acceleration = 2f, deceleration = 2f;
    public float maximumWalkVelocity = 0.5f, maximumRunVelocity = 2f;
    public float speed = 5f , runfactor = 1.5f;


    //increase performance
    int velocityZHash, velocityXHash;

    // Start is called before the first frame update
    void Start()
    {
        //anim = GetComponent<Animator>();
        cc = GetComponent<CharacterController>();
        
        velocityZHash = Animator.StringToHash("Velocity Z");
        velocityXHash = Animator.StringToHash("Velocity X");
    }


    //handles acceleration and decelaration
    void changeVelocity(bool forwardPressed, bool leftPressed, bool rightPressed, bool backPressed, bool runPressed, float currentMaxVelocity)
    {
        if (forwardPressed && velocityZ < currentMaxVelocity)
        {
            velocityZ += Time.deltaTime * acceleration;
        }
        if (leftPressed && velocityX > -currentMaxVelocity)
        {
            velocityX -= Time.deltaTime * acceleration;
        }
        if (rightPressed && velocityX < currentMaxVelocity)
        {
            velocityX += Time.deltaTime * acceleration;
        }
        if (backPressed && velocityZ > -currentMaxVelocity)
        {
            velocityZ -= Time.deltaTime * acceleration;
        }

        if (!forwardPressed && velocityZ > 0)
        {
            velocityZ -= deceleration * Time.deltaTime;
        }

        if (!leftPressed && velocityX < 0)
        {
            velocityX += deceleration * Time.deltaTime;
        }

        if (!rightPressed && velocityX > 0)
        {
            velocityX -= deceleration * Time.deltaTime;
        }

        if (!backPressed && velocityZ < 0)
        {
            velocityZ += deceleration * Time.deltaTime;
        }
    }

    void lockOrResetVelocity(bool forwardPressed, bool leftPressed, bool rightPressed, bool backPressed, bool runPressed, float currentMaxVelocity)
    {
        //if (!forwardPressed && velocityZ < 0)
        //{
        //    velocityZ = 0;
        //}

        if(!forwardPressed && !backPressed && !rightPressed && !leftPressed)
        {
            velocityZ = 0;
            velocityX = 0;
        }


        //if (!forwardPressed && !backPressed && (velocityZ == -0.1 || velocityZ == 0.1))
        //{
        //    velocityZ = 0;
        //}
        //if (!leftPressed && !rightPressed && (velocityX == -0.1 || velocityX == 0.1))
        //{
        //    velocityX = 0;
        //}


        if (!forwardPressed && !backPressed && velocityZ != 0 && (velocityZ > -0.05f && velocityZ < 0.05f))
        {
            velocityZ = 0;
        }



        if (!leftPressed && !rightPressed && velocityX != 0 && (velocityX > -0.05f && velocityX < 0.05f))
        {
            velocityX = 0;
        }

        //if(!backPressed && velocityZ > 0)
        //{
        //    velocityZ = 0;
        //}

        ////////////////////////////////////////////////////////

        //lock Forward
        if (forwardPressed && runPressed && velocityZ > currentMaxVelocity)
        {
            velocityZ = currentMaxVelocity;
        }
        //decelerate to the max walk velocity
        else if (forwardPressed && velocityZ > currentMaxVelocity)
        {
            velocityZ -= Time.deltaTime * deceleration;
            //round to the currentMaxVelocity if within offset
            if (velocityZ > currentMaxVelocity && velocityZ < (currentMaxVelocity + 0.05f))
            {
                velocityZ = currentMaxVelocity;
            }
        }

        //round to the currentMaxVelocity if within offset
        else if (forwardPressed && velocityZ < currentMaxVelocity && velocityZ > (currentMaxVelocity - 0.05f))
        {
            velocityZ = currentMaxVelocity;

        }

        ////////////////////////////////////////////////////////

        ////lock Backward
        if (backPressed && runPressed && velocityZ < -currentMaxVelocity)
        {
            velocityZ = -currentMaxVelocity;
        }
        //decelerate to the max walk velocity
        else if (backPressed && velocityZ < -currentMaxVelocity)
        {
            velocityZ += Time.deltaTime * deceleration;
            //round to the currentMaxVelocity if within offset
            if (velocityZ < -currentMaxVelocity && velocityZ > (-currentMaxVelocity - 0.05f))
            {
                velocityZ = -currentMaxVelocity;
            }
        }

        //round to the currentMaxVelocity if within offset
        else if (backPressed && velocityZ > -currentMaxVelocity && velocityZ < (-currentMaxVelocity + 0.05f))
        {
            velocityZ = -currentMaxVelocity;

        }
        ///////////////////////////////////////////////////////////

        //lock Left Sprint
        if (leftPressed && runPressed && velocityX < -currentMaxVelocity)
        {
            velocityX = -currentMaxVelocity;
        }

        //decelerate to the max walk velocity 
        else if (leftPressed && velocityX < -currentMaxVelocity)
        {
            velocityX += Time.deltaTime * deceleration;
            //round to the currentMaxVelocity if within offset
            if (velocityX < -currentMaxVelocity && velocityX > (-currentMaxVelocity - 0.05f))
            {
                velocityX = -currentMaxVelocity;
            }
        }
        //round to the currentMaxVelocity if within offset
        else if (leftPressed && velocityX > -currentMaxVelocity && velocityX < (-currentMaxVelocity + 0.05f))
        {
            velocityX = -currentMaxVelocity;
        }
        ////////////////////////////////////////////////////////////////
        //lock Right
        if (rightPressed && runPressed && velocityX > currentMaxVelocity)
        {
            velocityX = currentMaxVelocity;
        }

        //decelerate to the max walk velocity
        else if (rightPressed && velocityX > currentMaxVelocity)
        {
            velocityX -= Time.deltaTime * deceleration;
            //round to the currentMaxVelocity if within offset
            if (velocityX > currentMaxVelocity && velocityX < (currentMaxVelocity + 0.05f))
            {
                velocityX = currentMaxVelocity;
            }
        }
        //round to the currentMaxVelocity if within offset
        else if (rightPressed && velocityX < currentMaxVelocity && velocityX > (currentMaxVelocity - 0.05f))
        {
            velocityX = currentMaxVelocity;
        }
    }


    void handleGravity()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {

            bool forwardPressed = Input.GetKey(KeyCode.W);
            bool leftPressed = Input.GetKey(KeyCode.A);
            bool rightPressed = Input.GetKey(KeyCode.D);
            bool backPressed = Input.GetKey(KeyCode.S);
            bool runPressed = Input.GetKey(KeyCode.LeftShift);

            //Debug.Log("Forward Pressed " + forwardPressed + " " + " Backward Pressed " + backPressed);
            float currentMaxVelocity = runPressed ? maximumRunVelocity : maximumWalkVelocity;

            changeVelocity( forwardPressed,  leftPressed,  rightPressed, backPressed,  runPressed,  currentMaxVelocity);
            lockOrResetVelocity( forwardPressed,  leftPressed,  rightPressed, backPressed, runPressed,  currentMaxVelocity);

            anim.SetFloat(velocityZHash, velocityZ);
            anim.SetFloat(velocityXHash, velocityX);

            Vector3 movementVector = new Vector3();
            movementVector += transform.forward * velocityZ;
            movementVector += transform.right * velocityX;
            //Debug.Log("Magnitude of Mvector " + movementVector.magnitude + " moveVector = " + movementVector);
            //Debug.Log("Magnitude of normalised Mvector " + movementVector.normalized.magnitude + " moveVectorNormal = " + movementVector.normalized);


            //Gravity
            if (cc.isGrounded)
            {
                float groundedGravity = -.05f;
                movementVector.y = groundedGravity;
            }
            else
            {
                float gravity = -9.8f;
                movementVector.y += gravity;
            }
            if (runPressed)
            {
                cc.Move(movementVector.normalized * runfactor * speed * Time.deltaTime);
            }
            else
            {
                cc.Move(movementVector.normalized * speed * Time.deltaTime);
            }



        }
        
    }
}
