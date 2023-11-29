using Michsky.MUIP;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityAndroidBluetooth;

public class UIManager : MonoBehaviour
{
    [Header("UI Menus")]
    [SerializeField] private GameObject loadingMenu;
    [SerializeField] private GameObject mainMenu;

    [Header("UI Components")]
    [SerializeField] private FixedJoystick leftAnalog;
    [SerializeField] private FixedJoystick rightAnalog;
    [SerializeField] private ButtonManager powerButton;
    [SerializeField] private GameObject powerLedParent;
    [SerializeField] private Image powerLed;
    [SerializeField] private TextMeshProUGUI powerLedText;
    [SerializeField] private ButtonManager connectButton;
    [SerializeField] private GameObject connectedDeviceParent;
    [SerializeField] private TextMeshProUGUI connectedDeviceText;
    [SerializeField] private ButtonManager deviceInfoButton;
    [SerializeField] private GameObject deviceInfoPanel;
    [SerializeField] private TextMeshProUGUI deviceModelText;
    [SerializeField] private TextMeshProUGUI bluetoothText;

    [Header("Settings")]
    [SerializeField, Range(0f, 4f)] private float initialLoading = 1.5f;

    private bool deviceInfoPanelOpen = false;




    private void Start()
    {
    }

    // General UI Management
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

    // Device connection
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

    // Device Info
    public void OpenDeviceInfoPanel()
    {
        deviceInfoPanel.gameObject.SetActive(true);
        deviceInfoPanelOpen = true;
    }
    public void CloseDeviceInfoPanel()
    {
        deviceInfoPanel.gameObject.SetActive(false);
        deviceInfoPanelOpen = false;
    }
    public void UpdateDeviceInfo()
    {
        this.deviceModelText.text = $"Device model: {SystemInfo.deviceModel}";
    }

    #region UI callbacks
    public void OnPowerButton()
    {
    }
    public void OnDeviceInfoButton()
    {
        if (deviceInfoPanelOpen)
        {
            CloseDeviceInfoPanel();
        }
        else
        {
            OpenDeviceInfoPanel();
        }
    }
    #endregion

}
