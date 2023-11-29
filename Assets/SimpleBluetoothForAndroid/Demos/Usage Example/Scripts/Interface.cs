using SVSBluetooth;
using UnityEngine;
using UnityEngine.UI;

namespace Simple_Bluetooth_for_android.Demos.Usage_Example.Scripts
{
    public class Interface : MonoBehaviour {
        public Image image; // a picture that displays the status of the bluetooth adapter upon request
        public Text textField; // field for displaying messages and events
        const string MY_UUID = "0b7062bc-67cb-492b-9879-09bf2c7012b2"; // UUID constant which is set via script
        //00001101-0000-1000-8000-00805F9B34FB for arduino

        BluetoothForAndroid.BTDevice[] devices;
        string lastConnectedDeviceAddress;

        // subscription and unsubscribe from events. You can read more about events in Documentation.pdf
        private void OnEnable() {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            BluetoothForAndroid.ReceivedIntMessage += PrintVal1;
            BluetoothForAndroid.ReceivedFloatMessage += PrintVal2;
            BluetoothForAndroid.ReceivedStringMessage += PrintVal3;
            BluetoothForAndroid.ReceivedByteMessage += PrintVal4;

            BluetoothForAndroid.BtAdapterEnabled += PrintEvent1;
            BluetoothForAndroid.BtAdapterDisabled += PrintEvent2;
            BluetoothForAndroid.DeviceConnected += PrintEvent3;
            BluetoothForAndroid.DeviceDisconnected += PrintEvent4;
            BluetoothForAndroid.ServerStarted += PrintEvent5;
            BluetoothForAndroid.ServerStopped += PrintEvent6;
            BluetoothForAndroid.AttemptConnectToServer += PrintEvent7;
            BluetoothForAndroid.FailConnectToServer += PrintEvent8;

            BluetoothForAndroid.DeviceSelected += PrintDeviceData;
        }
        private void OnDisable() {
            BluetoothForAndroid.ReceivedIntMessage -= PrintVal1;
            BluetoothForAndroid.ReceivedFloatMessage -= PrintVal2;
            BluetoothForAndroid.ReceivedStringMessage -= PrintVal3;
            BluetoothForAndroid.ReceivedByteMessage -= PrintVal4;

            BluetoothForAndroid.BtAdapterEnabled -= PrintEvent1;
            BluetoothForAndroid.BtAdapterDisabled -= PrintEvent2;
            BluetoothForAndroid.DeviceConnected -= PrintEvent3;
            BluetoothForAndroid.DeviceDisconnected -= PrintEvent4;
            BluetoothForAndroid.ServerStarted -= PrintEvent5;
            BluetoothForAndroid.ServerStopped -= PrintEvent6;
            BluetoothForAndroid.AttemptConnectToServer -= PrintEvent7;
            BluetoothForAndroid.FailConnectToServer -= PrintEvent8;

            BluetoothForAndroid.DeviceSelected -= PrintDeviceData;
        }

        // Initially, always initialize the plugin.
        public void Initialize() {
            BluetoothForAndroid.Initialize();
        }

        // methods for controlling the bluetooth and getting its state
        public void GetBluetoothStatus() {
            if (BluetoothForAndroid.IsBTEnabled()) image.color = Color.green;
            else image.color = Color.red;
        }
        public void EnableBT() {
            BluetoothForAndroid.EnableBT();
        }
        public void DisableBT() {
            BluetoothForAndroid.DisableBT();
        }

        // methods for creating and stopping the server, connecting to the server and disconnecting
        public void CreateServer() {
            BluetoothForAndroid.CreateServer(MY_UUID);
        }
        public void StopServer() {
            BluetoothForAndroid.StopServer();
        }
        public void ConnectToServer() {
            BluetoothForAndroid.ConnectToServer(MY_UUID);
        }
        public void Disconnect() {
            BluetoothForAndroid.Disconnect();
        }
        public void ConnectToServerByAddress() {
            if (devices != null) {
                if (devices[0].address != "none") BluetoothForAndroid.ConnectToServerByAddress(MY_UUID, devices[0].address);
            } 
        }
        public void ConnectToLastServer() {
            if (lastConnectedDeviceAddress != null) BluetoothForAndroid.ConnectToServerByAddress(MY_UUID, lastConnectedDeviceAddress);
        }

        // methods for sending messages of various types
        public void WriteMessage1() {
            BluetoothForAndroid.WriteMessage(15);
            textField.text += BluetoothForAndroid.GetConnectedDeviceName() + "\n";
        }
        public void WriteMessage2() {
            BluetoothForAndroid.WriteMessage(100.69f);
        }
        public void WriteMessage3() {
            BluetoothForAndroid.WriteMessage("Hi !!!");
        }
        public void WriteMessage4() {
            BluetoothForAndroid.WriteMessage(new byte[] { 0, 1, 2, 3, 4, 5 });
        }

        // methods for displaying received messages on the screen
        void PrintVal1(int val) {
            textField.text += val.ToString() + "\n";
        }
        void PrintVal2(float val) {
            textField.text += val.ToString() + "\n";
        }
        void PrintVal3(string val) {
            textField.text += val + "\n";
        }
        void PrintVal4(byte[] val) {
            foreach (var item in val) {
                textField.text += item;
            }
            textField.text += "\n";
        }
        public void GetBondedDevices() {
            devices = BluetoothForAndroid.GetBondedDevices();
            if (devices != null) {
                for (int i = 0; i < devices.Length; i++) {
                    textField.text += devices[i].name + "   ";
                    textField.text += devices[i].address;
                    textField.text += "\n";
                }
            }
        }

        // methods for displaying events on the screen
        void PrintEvent1() {
            textField.text += "Adapter enabled" + "\n";
        }
        void PrintEvent2() {
            textField.text += "Adapter disabled" + "\n";
        }
        void PrintEvent3() {
            textField.text += "The device is connected" + "\n";
        }
        void PrintEvent4() {
            textField.text += "Device lost connection" + "\n";
        }
        void PrintEvent5() {
            textField.text += "Server is running" + "\n";
        }
        void PrintEvent6() {
            textField.text += "Server stopped" + "\n";
        }
        void PrintEvent7() {
            textField.text += "Attempt to connect to server" + "\n";
        }
        void PrintEvent8() {
            textField.text += "Connection to the server failed" + "\n";
        }
        void PrintDeviceData(string deviceData) {
            string[] btDevice = deviceData.Split(new char[] { ',' });
            textField.text += btDevice[0] + "   ";
            textField.text += btDevice[1] + "\n";
            lastConnectedDeviceAddress = btDevice[1];
        }

        // method for cleaning the log
        public void ClearLog() {
            textField.text = "";
        }
    }
}
