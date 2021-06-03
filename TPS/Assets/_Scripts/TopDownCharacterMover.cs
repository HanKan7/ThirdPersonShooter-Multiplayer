using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownCharacterMover : MonoBehaviour
{

    InputHandler input;

    [SerializeField]
    private float moveSpeed;

    [SerializeField]
    Camera cam;

    public float rotateSpeed = 15f, PosLerpAmt = 0.1f , RotLerpAmt = 0.01f;

    [SerializeField]
    bool rotateTowardsMouse;

    [SerializeField]
    GameObject cameraPos;

    [SerializeField]
    GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        input = GetComponent<InputHandler>();
        cam = Camera.main;
    }

    // Update is called once per frame;
    void Update()
    {
        //var targetVector = new Vector3(input.InputVector.x, 0, input.InputVector.y);
        //Move in the direction we are aiming

        //var movementVector =  MoveTowardTarget(targetVector);
        //if(!rotateTowardsMouse) RotateTowardMovementVector(movementVector);

        //elseRotateTowardsMouse();
        RotateCharacter();
        
    }

    void RotateTowardsMouse()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray , out RaycastHit hitInfo , maxDistance: 300f))
        {
            var target = hitInfo.point;
            target.y = transform.position.y;
            player.transform.LookAt(target);
        }

        
    }

    void RotateCharacter()
    {
        
        if (Input.GetKey(KeyCode.K))
        {
            transform.Rotate(Vector3.up * -rotateSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.L))
        {
            transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
        }
    }

    private void LateUpdate()
    {
        cam.transform.position = cameraPos.transform.position;
    //    cam.transform.position = Vector3.Lerp(cam.transform.position, cameraPos.transform.position,PosLerpAmt);
        cam.transform.rotation = cameraPos.transform.rotation;
    //    cam.transform.rotation = Quaternion.Lerp(cam.transform.rotation, cameraPos.transform.rotation, RotLerpAmt);
    //    cameraPos.transform.LookAt(player.transform);
    }
    private void RotateTowardMovementVector( Vector3 movementVector )
    {
        if (movementVector.magnitude == 0) return;
        var rotation = Quaternion.LookRotation(movementVector);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, rotateSpeed);
    }

    private Vector3 MoveTowardTarget(Vector3 targetVector)
    {
        var speed = moveSpeed * Time.deltaTime;
        targetVector = Quaternion.Euler(0, cam.gameObject.transform.eulerAngles.y, 0) * targetVector;
        var targetPosition = transform.position + targetVector * speed;
        transform.position = targetPosition;
        return targetVector;
    }
}
