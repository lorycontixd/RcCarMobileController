using SVSBluetooth;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Simple_Bluetooth_for_android.Demos.Game_Example.Scripts
{
    public class MenuInterface : MonoBehaviour {

        public string MY_UUID; // UUID string that is set through the inspector
        string role;
        public GameObject MultiplayerPanel;
        public GameObject WaitPanel;
        public GameObject turnOnBluetoothText;
        public GameObject failConnectionText;
        public GameObject waitConnectionImage;
        public GameObject waitConnectionCancelButton;

        // comments for operations of the same type will not be repeated
        // event subscription
        // for information on the appointment of events, see the Documentation.pdf
        private void OnEnable() {
            BluetoothForAndroid.BtAdapterEnabled += HideTurnOnBluetoothText;
            BluetoothForAndroid.DeviceConnected += LoadGameScene;
            BluetoothForAndroid.ServerStarted += OpenWaitConnectionWindow;
            BluetoothForAndroid.AttemptConnectToServer += OpenWaitConnectionWindow;
            BluetoothForAndroid.FailConnectToServer += FailConnect;
        }
        // unsubscribe from events
        private void OnDisable() {
            BluetoothForAndroid.BtAdapterEnabled -= HideTurnOnBluetoothText;
            BluetoothForAndroid.DeviceConnected -= LoadGameScene;
            BluetoothForAndroid.ServerStarted -= OpenWaitConnectionWindow;
            BluetoothForAndroid.AttemptConnectToServer -= OpenWaitConnectionWindow;
            BluetoothForAndroid.FailConnectToServer -= FailConnect;
        }

        private void Start() {
            Screen.sleepTimeout = SleepTimeout.NeverSleep; // so that the screen does not go out
            BluetoothForAndroid.Initialize(); // plugin initialization
        }

        public void ExitGame() {
            Application.Quit(); // close application
        }
        public void OpenMultiplayerWindow() {
            if (!BluetoothForAndroid.IsBTEnabled()) BluetoothForAndroid.EnableBT(); // command to turn on the bluetooth if the bluetooth adapter is not activated
            MultiplayerPanel.SetActive(true); // opening the connection window
        }
        public void CloseMultiplayerWindow() {
            MultiplayerPanel.SetActive(false);// close the connection window
        }
        public void StarServer() {
            role = "server"; // a flag that activates or deactivates the cancel button on the connection standby screen
            if (BluetoothForAndroid.IsBTEnabled()) {
                turnOnBluetoothText.SetActive(false);
                BluetoothForAndroid.CreateServer(MY_UUID); // create a server with the specified UUID
            }
            else turnOnBluetoothText.SetActive(true); // if bluetooth is off, a message is displayed asking you to turn on bluetooth
        }
        public void ConnectToServer() {
            role = "client";
            if (BluetoothForAndroid.IsBTEnabled()) {
                turnOnBluetoothText.SetActive(false);
                BluetoothForAndroid.ConnectToServer(MY_UUID); // connect to server with specified UUID
            }
            else turnOnBluetoothText.SetActive(true);
        }
        private void HideTurnOnBluetoothText() {
            turnOnBluetoothText.SetActive(false);
        }
        private void OpenWaitConnectionWindow() {
            waitConnectionImage.SetActive(true); // turning on animation
            WaitPanel.SetActive(true);
            failConnectionText.SetActive(false);
            if (role == "server") waitConnectionCancelButton.SetActive(true);        
            else waitConnectionCancelButton.SetActive(false);
        }
        private void FailConnect() {
            waitConnectionImage.SetActive(false);
            waitConnectionCancelButton.SetActive(true);
            failConnectionText.SetActive(true);
        }
        public void CloseWaitConnectionWindow() {
            if (role == "server") BluetoothForAndroid.StopServer(); // server shutdown
            WaitPanel.SetActive(false);
        }

        void LoadGameScene() {
            SceneManager.LoadScene(1); // loading the game scene
        }
    }
}
