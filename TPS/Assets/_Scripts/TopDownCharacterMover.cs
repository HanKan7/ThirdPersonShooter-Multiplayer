using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Cinemachine;

public class TopDownCharacterMover : MonoBehaviourPunCallbacks
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

    [SerializeField]
    LayerMask ground;

    [Header("Material Change Attributes")]
    [SerializeField]
    SkinnedMeshRenderer playerMesh;
    [SerializeField]
    GameObject materialRaycast;
    [SerializeField]
    float rayDistance = 100f;
    [SerializeField] Material[] playerMaterials;

    [Header("Cinecmachine")]
    [SerializeField]
    GameObject vcam;
    

    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
        {
            input = GetComponent<InputHandler>();
            cam = Camera.main;
            vcam = GameObject.Find("VCam");
            vcam.GetComponent<CinemachineVirtualCamera>().enabled = true;
            Transform playerTransform = this.gameObject.transform;
            vcam.GetComponent<CinemachineVirtualCamera>().Follow = playerTransform;
            vcam.GetComponent<CinemachineVirtualCamera>().LookAt = playerTransform;
        }
    }

    // Update is called once per frame;
    void Update()
    {
        //var targetVector = new Vector3(input.InputVector.x, 0, input.InputVector.y);
        if (photonView.IsMine)
        {
            RotateTowardsMouse();
            RaycastTowardsPlayer();
        }

        ////Move in the direction we are aiming

        //var movementVector = MoveTowardTarget(targetVector);
        //if (!rotateTowardsMouse) RotateTowardMovementVector(movementVector);

        //else 

        //RotateCharacter();

    }

    void RotateTowardsMouse()
    {
        
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray , out RaycastHit hitInfo , maxDistance: 300f , ground))
        {
            Debug.Log("Rotating Player");
            var target = hitInfo.point;
            target.y = transform.position.y;
            transform.LookAt(target);
        }

        
    }

    void RaycastTowardsPlayer()
    {
        Ray ray = new Ray();
        ray.origin = cam.transform.position;
        ray.direction = materialRaycast.transform.position - cam.transform.position;
        Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.red );
        if(Physics.Raycast(ray, out RaycastHit hit, rayDistance))
        {
            //Debug.Log(hit.collider.name);
            if (!hit.collider.CompareTag("Player"))
            {
                playerMesh.material = playerMaterials[1];
            }
            else
            {
                playerMesh.material = playerMaterials[0];
            }
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

    //private void LateUpdate()
    //{
    //    cam.transform.position = cameraPos.transform.position;
    //    cam.transform.position = Vector3.Lerp(cam.transform.position, cameraPos.transform.position, PosLerpAmt);
    //    cam.transform.rotation = cameraPos.transform.rotation;
    //    cam.transform.rotation = Quaternion.Lerp(cam.transform.rotation, cameraPos.transform.rotation, RotLerpAmt);
    //    //cameraPos.transform.LookAt(player.transform);
    //}
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
