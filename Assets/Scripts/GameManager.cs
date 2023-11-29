using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityAndroidBluetooth;

public class GameManager : MonoBehaviour
{
    #region Singleton
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    #endregion

    #region PowerState
    public enum PowerState
    {
        ON,
        OFF
    }
    #endregion

    public PowerState controllerState { get; private set; }
    public string connectedDevice { get; private set; } = "";
    public List<BluetoothDevice> currentlyAvailableBluetoothDevices { get; private set; } = new List<BluetoothDevice>();

    [SerializeField] private FixedJoystick leftAnalog; // Speed controller
    [SerializeField] private FixedJoystick rightAnalog; // direction controller

    public Action onDeviceConnected;
    public Action onDeviceDisconnected;
    public Action<PowerState> onControllerStateChanged;

    private BluetoothServer server;

    private void Start()
    {
        server = new BluetoothServer();
        server.ClientConnected += OnClientConnected;
        StartCoroutine(StartSearchDelayed(2f));
    }
    private void Update()
    {
        if (server != null)
            Debug.Log($"Found devices: {server.foundDevices.Count}");
    }
    private IEnumerator StartSearchDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        Bluetooth.SearchDevices();
    }

    private void OnClientConnected(object sender, DeviceInfoEventArgs e)
    {
        Debug.Log($"CLIENT CONNECTED: {sender}, ({e.Device.name}, {e.Device.address})");
    }

    public void TurnOnController()
    {
        controllerState = PowerState.ON;
        onControllerStateChanged?.Invoke(PowerState.ON);
    }
    public void TurnOffController()
    {
        controllerState = PowerState.OFF;
        onControllerStateChanged?.Invoke(PowerState.OFF);
    }

    public void StartBluetoothDeviceSearch()
    {
        Bluetooth.SearchDevices();

    }
}
