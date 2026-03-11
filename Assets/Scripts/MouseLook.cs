using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public float controllerSensitivity = 100f;
    public Transform playerBody;

    float xRotation = 0f;

    void Update()
    {
        // Mouse 
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Controller  (Right Stick)
        float stickX = Input.GetAxis("RightStickX") * controllerSensitivity * Time.deltaTime;
        float stickY = Input.GetAxis("RightStickY") * controllerSensitivity * Time.deltaTime;

        //  mouse and controller
        float finalX = mouseX + stickX;
        float finalY = mouseY + stickY;

        // Vert look
        xRotation -= finalY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Hor look (rotate player body)
        playerBody.Rotate(Vector3.up * finalX);
    }
}