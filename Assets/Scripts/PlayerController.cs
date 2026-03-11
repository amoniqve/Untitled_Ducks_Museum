using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float walkSpeed   = 10f;
    public float sprintSpeed = 20f;
    private CharacterController controller;

    // KeyCode for Xbox LB (joystick button 4 in Unity's legacy input)
    private const KeyCode XboxLB = KeyCode.JoystickButton4;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Horizontal / Vertical cover both keyboard (WASD/arrows) and left stick via Input Manager
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Sprint: keyboard LeftShift or Xbox LB
        bool wantsToSprint = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(XboxLB);
        bool hasStamina    = HUDController.Instance == null || HUDController.Instance.currentStamina > 0f;
        float currentSpeed = (wantsToSprint && hasStamina) ? sprintSpeed : walkSpeed;

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * currentSpeed * Time.deltaTime);
    }
}