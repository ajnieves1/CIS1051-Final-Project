using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimationAndMovementController : MonoBehaviour
{
    public ParticleSystem dust;
    PlayerInput playerInput; // reference variable declared
    CharacterController characterController;
    Animator animator;
    
    

    int isWalkingHash; // Variables to store optimized setter/getter parameters
    int isRunningHash;

    Vector2 currentMovementInput; // Variables to store player input values
    Vector3 currentMovement;
    Vector3 currentRunMovement;
    bool isMovementPressed;
    bool isRunPressed;

    //constants
    float rotationFactorPerFrame = 0.5f;
    float runMultiplier = 1.5f;
    int zero = 0;

    // gravity variables
    float gravity = -9.8f;
    float groundedGravity = -.05f;

    // jump variables
    bool isJumpPressed = false;
    float initialJumpVelocity;
    float maxJumpHeight = 6.0f;
    float maxJumpTime = 0.75f;
    bool isJumping = false;
    int isJumpingHash;
    bool isJumpAnimating = false;

    

    


    void Awake() // Awake is called earlier than start
    {
        playerInput =  new PlayerInput(); // Set reference variables
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");
        isJumpingHash = Animator.StringToHash("isJumping");


        playerInput.CharacterControls.Move.started += onMovementInput; // These are all player input callbacks
        playerInput.CharacterControls.Move.canceled += onMovementInput;
        playerInput.CharacterControls.Move.performed += onMovementInput;
        playerInput.CharacterControls.Run.started += onRun;
        playerInput.CharacterControls.Run.canceled += onRun;
        playerInput.CharacterControls.Jump.started += onJump;
        playerInput.CharacterControls.Jump.canceled += onJump;

        setupJumpVariables();
    }

    void setupJumpVariables()
    {
        float timeToApex = maxJumpTime / 2;
        gravity =(-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        initialJumpVelocity = (2 * maxJumpHeight) / timeToApex;
    }

    void handleJump()
    {
        if (!isJumping && characterController.isGrounded && isJumpPressed) {
            animator.SetBool(isJumpingHash, true);
            isJumpAnimating = true;
            isJumping = true;
            currentMovement.y = initialJumpVelocity * .5f;
            currentRunMovement.y = initialJumpVelocity * .5f;
            
        } else if (!isJumpPressed && isJumping && characterController.isGrounded) {
            isJumping = false;
        }

    }

    void onJump (InputAction.CallbackContext context)
    {
        isJumpPressed = context.ReadValueAsButton();
    }



    void onRun(InputAction.CallbackContext context)
    {
        isRunPressed = context.ReadValueAsButton();
        CreateDust();
    }

    void handleRotation()
    {
        Vector3 positionToLookAt;
        // Change in poisition the character points to
        positionToLookAt.x = currentMovementInput.x;
        positionToLookAt.y = 0.0f;
        positionToLookAt.z = currentMovementInput.y;

        // rotation of character
        Quaternion currentRotation = transform.rotation;
        if (isMovementPressed) // Creates new rotation based on where player is pressing
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame * Time.deltaTime);
        }
        
    }

    void onMovementInput(InputAction.CallbackContext context)
    {
            currentMovementInput = context.ReadValue<Vector2>();
            currentMovement.x = currentMovementInput.x;
            currentMovement.z = currentMovementInput.y;
            currentRunMovement.x = currentMovementInput.x * runMultiplier;
            currentRunMovement.z = currentMovementInput.y * runMultiplier;
            isMovementPressed = currentMovementInput.x != zero || currentMovementInput.y != zero;        
    }

    void handleAnimation()
    {
        bool isWalking = animator.GetBool(isWalkingHash);
        bool isRunning = animator.GetBool(isRunningHash);

        if (isMovementPressed && !isWalking) { // start walking if movement pressed is true and not already walking
            animator.SetBool(isWalkingHash, true);
        }

        else if (!isMovementPressed && isWalking) { // stop walking if isMovementPressed is false and not already walking
            animator.SetBool(isWalkingHash, false);
        }

        if ((isMovementPressed && isRunPressed) && !isRunning) // run if movement and run are true
        {
            animator.SetBool(isRunningHash, true);
        }

        else if ((!isMovementPressed || !isRunPressed) && isRunning) // stop running if movement or run pressed are false
        {
            animator.SetBool(isRunningHash, false);
        }
    }

    void handleGravity()
    {
        bool isFalling = currentMovement.y <= 0.0f || !isJumpPressed; // Allows short jumps to be made by tapping the jump key, high jump if held down
        float fallMultiplier = 2.0f;

        if (characterController.isGrounded) {
            if (isJumpAnimating) {
                animator.SetBool(isJumpingHash, false);
                isJumpAnimating = false;
            }
            currentMovement.y = groundedGravity;
            currentRunMovement.y = groundedGravity;
        } else if (isFalling) {
            float previousYVelocity = currentMovement.y;
            float newYVelocity = currentMovement.y + (gravity * fallMultiplier * Time.deltaTime);
            float nextYVelocity = Mathf.Max((previousYVelocity + newYVelocity) * .5f, -20.0f);
            currentMovement.y = nextYVelocity;
            currentRunMovement.y = nextYVelocity;
        } else {
            float previousYVelocity = currentMovement.y;
            float newYVelocity = currentMovement.y + (gravity * Time.deltaTime);
            float nextYVelocity = (previousYVelocity + newYVelocity) * .5f;
            currentMovement.y = nextYVelocity; 
            currentRunMovement.y = nextYVelocity; // old velocity + acceleration = new velocity (something like that the physics part of jumping is confusing)
        }
    }




    // Update is called once per frame, if 60 fps update runs 60 times
    void Update()
    {
        

        handleRotation();
        handleAnimation();


        if (isRunPressed)
        {
            characterController.Move(currentRunMovement * Time.deltaTime);
        }
        else 
        {
            characterController.Move(currentMovement * Time.deltaTime);
        }
        
        handleGravity();
        handleJump();

    }


    void OnEnable()
    {
        playerInput.CharacterControls.Enable(); // Enables character action map
    }

    void OnDisable()
    {
        playerInput.CharacterControls.Disable(); // Disables character action map
    }

    void CreateDust()
    {
        dust.Play();
    }

}
