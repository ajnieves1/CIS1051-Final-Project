using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class CameraRelativeMovement : MonoBehaviour
{
    float _horizontalInput;
    float _verticalInput;
    Vector3 _playerInput;
    [SerializeField] CharacterController characterController;



    // Start is called before the first frame update
    void Start()
    {       
        // get character controller component attached to our character
        characterController = GetComponent<CharacterController>();

    }

    // Update is called once per frame
    void Update()
    {
        // store player input on every frame
        _horizontalInput = Input.GetAxis("Horizontal");
        _verticalInput = Input.GetAxis("Vertical");

        // set values to X and Z values of the vector3
        _playerInput.x = _horizontalInput;
        _playerInput.z = _verticalInput;

        // rotate player input vector to camera space
        Vector3 cameraRelativeMovement = ConvertToCameraSpace(_playerInput);
        characterController.Move(cameraRelativeMovement * Time.deltaTime);

        // transform position using move and player input Vector3
         
    }

    Vector3 ConvertToCameraSpace(Vector3 vectorToRotate)
    {
        // store Y value of original vector to rotate
        float currentYValue = vectorToRotate.y;

        // get forward and right directional vectors of camera
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;

        // remove Y values to ignore awkard camera angles
        cameraForward.y = 0;
        cameraRight.y = 0;

        // re-normalize both vectors to a magnitude of 1
        cameraForward = cameraForward.normalized;
        cameraRight = cameraRight.normalized;

        // rotate the X and Z VectorToRotate values to camera space
        Vector3 cameraForwardZProduct = vectorToRotate.z * cameraForward;
        Vector3 cameraRightXProduct = vectorToRotate.x * cameraRight;

        // sum of both products is the vector3 in camera space
        Vector3 vectorRotatedToCameraSpace = cameraForwardZProduct + cameraRightXProduct;
        vectorRotatedToCameraSpace.y = currentYValue;
        return vectorRotatedToCameraSpace;


    }



}
