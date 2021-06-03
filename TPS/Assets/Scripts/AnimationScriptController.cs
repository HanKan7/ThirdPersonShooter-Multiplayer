using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationScriptController : MonoBehaviour
{
    Animator animator;
    int isWalkingHash;
    int isRunningHash;
    int veloctiyHash;
    float velocity = 0f;
    float acceleration = 0.1f, deceleration = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");
        veloctiyHash = Animator.StringToHash("Velocity");
    }

    // Update is called once per frame
    void Update()
    {
        bool isWalking = animator.GetBool(isWalkingHash);
        bool isRunning = animator.GetBool(isRunningHash);
        bool forwardPressed = Input.GetKey(KeyCode.W);
        bool runPressed = Input.GetKey(KeyCode.LeftShift);

        /*if (!isWalking && forwardPressed)
        {
            animator.SetBool(isWalkingHash, true);
            velocity += Time.deltaTime * acceleration;
        }
        if (isWalking && !forwardPressed)
        {
            animator.SetBool(isWalkingHash, false);
        }

        if(!isRunning && (forwardPressed && runPressed))
        {
            animator.SetBool(isRunningHash, true);
        }
        if (isRunning && (!forwardPressed || !runPressed))
        {
            animator.SetBool(isRunningHash, false);
        }*/

        if (forwardPressed && velocity < 1)
        {
            velocity += acceleration * Time.deltaTime;
        }

        if (!forwardPressed && velocity > 0)
        {
            velocity -= deceleration * Time.deltaTime;
        }

        if (!forwardPressed && velocity < 0)
        {
            velocity = 0;
        }

        animator.SetFloat(veloctiyHash, velocity);

    }
}
