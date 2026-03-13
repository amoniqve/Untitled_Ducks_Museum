using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 10f;
    public float sprintSpeed = 20f;
    private CharacterController controller;

    private const KeyCode XboxLB = KeyCode.JoystickButton4;

    [Header("Audio")]
    public AudioClip walkSound;
    public AudioClip sprintStartSound;
    public float stepInterval = 0.5f;

    [Header("Gravity")]
    public float gravity = -9.81f; 
    private float verticalVelocity = 0f;

    private bool isSprinting = false;
    private float stepTimer = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        controller.stepOffset = 0f; 
    }

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        bool wantsToSprint = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(XboxLB);
        bool hasStamina = HUDController.Instance == null || HUDController.Instance.currentStamina > 0f;
        float currentSpeed = (wantsToSprint && hasStamina) ? sprintSpeed : walkSpeed;

     
        bool isGrounded = controller.isGrounded;
        if (isGrounded && verticalVelocity < 0)
            verticalVelocity = -2f; 

        verticalVelocity += gravity * Time.deltaTime;

        
        Vector3 move = transform.right * x + transform.forward * z;
        move.y = verticalVelocity;
        controller.Move(move * currentSpeed * Time.deltaTime);

        
        if (wantsToSprint && !isSprinting)
        {
            isSprinting = true;
            if (sprintStartSound != null)
                AudioSource.PlayClipAtPoint(sprintStartSound, transform.position);
        }
        else if (!wantsToSprint && isSprinting)
        {
            isSprinting = false;
        }

        if (new Vector3(x, 0, z).magnitude > 0.1f)
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0f && walkSound != null)
            {
                AudioSource.PlayClipAtPoint(walkSound, transform.position);
                stepTimer = stepInterval / (isSprinting ? 1.5f : 1f);
            }
        }
        else
        {
            stepTimer = 0f;
        }
    }
}