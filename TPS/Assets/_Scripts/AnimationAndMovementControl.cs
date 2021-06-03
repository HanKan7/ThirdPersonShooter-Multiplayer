using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class AnimationAndMovementControl : MonoBehaviour
{
    public PlayerInput playerInput;
    CharacterController cc;
    Animator anim;


    Vector2 currentMovementInput;
    Vector3 currentMovement;
    Vector3 currentRunMovement;
    bool isMovementPressed;
    bool isRunPressed;
    public float rotationFactorPerFrame = 15f;
    public float runMultiplier = 3f;

    int isWalkingHash, isRunningHash;

    private void Awake()
    {
        playerInput = new PlayerInput();
        cc = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();

        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");

        playerInput.CharacterControls.Move.started += onMovementInput;

        playerInput.CharacterControls.Move.canceled += onMovementInput;

        playerInput.CharacterControls.Move.performed += onMovementInput;

        playerInput.CharacterControls.Run.started += onRun;

        playerInput.CharacterControls.Run.canceled += onRun;
    }

    void onRun(InputAction.CallbackContext context)
    {
        isRunPressed = context.ReadValueAsButton();
    }

    void onMovementInput(InputAction.CallbackContext context)
    {
        currentMovementInput = context.ReadValue<Vector2>();
        currentMovement.x = currentMovementInput.x;
        currentMovement.z = currentMovementInput.y;
        currentRunMovement.x = currentMovement.x * runMultiplier;
        currentRunMovement.z = currentMovement.z * runMultiplier;
        isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
    }

    void handleRotation()
    {
        Vector3 positionToLookAt;
        positionToLookAt.x = currentMovement.x;
        positionToLookAt.y = 0;
        positionToLookAt.z = currentMovement.z;

        Quaternion currentRotation = transform.rotation;

        if (isMovementPressed)
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation =  Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame * Time.deltaTime);
        }
    }
        

    void handleAnimation()
    {
        bool isWalking = anim.GetBool(isWalkingHash);
        bool isRunning = anim.GetBool(isRunningHash);

        if(isMovementPressed && !isWalking)
        {
            anim.SetBool(isWalkingHash, true);
        }
        else if(!isMovementPressed && isWalking)
        {
            anim.SetBool(isWalkingHash, false);
        }
        if((isMovementPressed && isRunPressed) && !isRunning)
        {
            anim.SetBool(isRunningHash, true);
        }

        else if((!isMovementPressed || !isRunPressed) && isRunning)
        {
            anim.SetBool(isRunningHash, false);
        }
    }

    private void Update()
    {
        handleAnimation();
        handleRotation();

        if (isRunPressed)
        {
            cc.Move(currentRunMovement * Time.deltaTime);
        }
        else
        {
            cc.Move(currentMovement * Time.deltaTime);
        }
        
    }

    private void OnEnable()
    {
        playerInput.CharacterControls.Enable();
    }

    private void OnDisable()
    {
        playerInput.CharacterControls.Disable();
    }
}
