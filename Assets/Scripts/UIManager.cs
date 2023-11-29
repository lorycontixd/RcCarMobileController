using Michsky.MUIP;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI Panels")]

    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private ButtonManager powerButton;
    [SerializeField] private GameObject powerLedParent;
    [SerializeField] private Image powerLed;
    [SerializeField] private TextMeshProUGUI powerLedText;
    [SerializeField] private GameObject connectedDeviceParent;
    [SerializeField] private TextMeshProUGUI connectedDeviceText;

    [Header("Settings")]
    [SerializeField, Range(0f, 4f)] private float initialLoading = 1.5f;



    private void Start()
    {
    }

    public void DeactivateAllUI()
    {
        powerButton.gameObject.SetActive(false);
        powerLedParent.gameObject.SetActive(false);
    }
    public void ActivateAllUI()
    {
        powerButton.gameObject.SetActive(true);
        powerLedParent.gameObject.SetActive(true);
    }

    public void SetConnectedDeviceText(string deviceName)
    {
        if (connectedDeviceText != null)
        {
            connectedDeviceText.text = deviceName;
            connectedDeviceParent.SetActive(true);
            connectedDeviceText.gameObject.SetActive(true);
        }
        else
        {
            connectedDeviceParent.SetActive(false);
            connectedDeviceText.text = string.Empty;
        }
    }
    public void SetNoDeviceText()
    {
        connectedDeviceParent.SetActive(false);
        connectedDeviceText.text = string.Empty;
    }
}
