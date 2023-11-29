using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public string connectedDevice = "";

    public Action onDeviceConnected;
    public Action onDeviceDisconnected;
    public Action<PowerState> onControllerStateChanged;



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
}
