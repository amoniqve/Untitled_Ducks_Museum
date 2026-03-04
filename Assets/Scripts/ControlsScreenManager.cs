using UnityEngine;
using UnityEngine.UI;

public class ControlsScreenManager : MonoBehaviour
{
    [Header("Button References")]
    public Button backButton;

    private void Start()
    {
        if (backButton != null)
            backButton.onClick.AddListener(() => UIManager.Instance.ShowMainMenu());
    }
}
