using UnityEngine;

public class DetectionTester : MonoBehaviour
{
    private DetectionMeter detectionMeter;
    private HUDController hudController;

    private void Start()
    {
        detectionMeter = FindObjectOfType<DetectionMeter>();
        hudController = FindObjectOfType<HUDController>();
    }

    private void Update()
    {
        if (detectionMeter == null) return;

        if (Input.GetKey(KeyCode.Q))
        {
            detectionMeter.IncreaseDetection(Time.deltaTime * 0.3f);
        }

        if (Input.GetKey(KeyCode.Z))
        {
            detectionMeter.DecreaseDetection(Time.deltaTime * 0.5f);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            detectionMeter.ResetDetection();
        }

        if (Input.GetKeyDown(KeyCode.T) && hudController != null)
        {
            hudController.ShowInteractionPrompt("Press [E] to interact");
        }

        if (Input.GetKeyDown(KeyCode.Y) && hudController != null)
        {
            hudController.HideInteractionPrompt();
        }

        if (detectionMeter.detectionLevel >= 1f)
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowGameOver();
            }
        }
    }
}
