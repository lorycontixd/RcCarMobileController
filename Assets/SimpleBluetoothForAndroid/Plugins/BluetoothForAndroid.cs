using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simple_Bluetooth_for_android.Plugins.AndroidRuntimePermissions;
using UnityEngine;

// ATTENTION !!! 
// This is the service class of the plugin for working with the native code of the android system. 
// No need to change anything here, otherwise the plugin will stop working.
// To work with plug-in methods, read Documentation.pdf

namespace SVSBluetooth {
    public class BluetoothForAndroid {
        private static BluetoothForAndroid instance;
        public static AndroidJavaClass javaClass;
        public static AndroidJavaObject javaObject;

        public static event Action BtAdapterEnabled;
        public static event Action BtAdapterDisabled;

        public static event Action ServerStarted;
        public static event Action ServerStopped;

        public static event Action AttemptConnectToServer;
        public static event Action FailConnectToServer;

        public static event Action DeviceConnected;
        public static event Action DeviceDisconnected;

        public static event Action<int> ReceivedIntMessage;
        public static event Action<float> ReceivedFloatMessage;
        public static event Action<string> ReceivedStringMessage;
        public static event Action<byte[]> ReceivedByteMessage;

        public static event Action<string> DeviceSelected;

        const string BT_CLASS_NAME = "com.SVStar.BtForUnityAndroid.BT_Plugin";
        public struct BTDevice {
            public string name;
            public string address;
            public BTDevice(string name, string address) {
                this.name = name;
                this.address = address;
            }
        }

        #region static methods

        public static void Initialize() {
            if (Application.platform == RuntimePlatform.Android) {

                if (instance == null) instance = new BluetoothForAndroid();
                GameObject emptyObject = new GameObject();
                emptyObject.AddComponent<BluetoothBridgeForAndroid>();

                javaClass = new AndroidJavaClass(BT_CLASS_NAME);
                javaClass.CallStatic("Initialize");
                javaObject = javaClass.GetStatic<AndroidJavaObject>("btPlugin");
            }
        }

        public static bool IsBTEnabled() {
            if (javaObject != null) return javaObject.Call<bool>("IsEnabled");
            else return false;
        }
   
        public static void EnableBT() {
            if (javaObject != null) javaObject.Call("EnableBT");
        }
  
        public static void DisableBT() {
            if (javaObject != null) javaObject.Call("DisableBT");
        }

        public static void CreateServer(string UUID)
        {
            instance.CheckPermissions();
            
            if (javaObject != null) {
                javaObject.Call("CreateServer", UUID);
            }
        }
 
        public static void StopServer() {
            if (javaObject != null) {
                javaObject.Call("StopServer");
            }
        }
 
        public static void ConnectToServer(string UUID) {
            instance.CheckPermissions();
            
            if (javaObject != null) {
                javaObject.Call("ConnectToServer", UUID);
            }
        }
     
        public static void Disconnect() {
            if (javaObject != null) javaObject.Call("Disconnect");
        }
  
        public static void WriteMessage(int val) {
            if (javaObject != null) {
                byte[] array = BitConverter.GetBytes(val);
                sbyte[] sBytes = instance.ByteToSByteArray(instance.ModAddArray(array, 0));
                javaObject.Call("WriteMessage", sBytes);
            }
        }
        public static void WriteMessage(float val) {
            if (javaObject != null) {
                byte[] array = BitConverter.GetBytes(val);
                sbyte[] sBytes = instance.ByteToSByteArray(instance.ModAddArray(array, 1));
                javaObject.Call("WriteMessage", sBytes);
            }
        }
        public static void WriteMessage(string val) {
            if (javaObject != null && val.Length > 0) {
                byte[] array = Encoding.UTF8.GetBytes(val);
                sbyte[] sBytes = instance.ByteToSByteArray(instance.ModAddArray(array, 2));
                javaObject.Call("WriteMessage", sBytes);
            }
        }
        public static void WriteMessage(byte[] val) {
            if (javaObject != null && val.Length > 0) {
                sbyte[] sBytes = instance.ByteToSByteArray(instance.ModAddArray(val, 3));
                javaObject.Call("WriteMessage", sBytes);
            }
        }
        public static BTDevice[] GetBondedDevices() {
            if (javaObject != null) {
                return instance.GetBTArray(javaObject.Call<string[]>("GetBondedDevices"));
            }
            else return null;
        }
        public static void ConnectToServerByAddress(string UUID, string address) {
            string[] par = { UUID, address };
            if (javaObject != null) javaObject.Call("ConnectToServerByAddress", par);
        }
        public static string GetConnectedDeviceName()
        {
            string name = "not connected";
            if (javaObject != null)
                name = javaObject.Call<string>("GetConnectedDeviceName");
            return name;
        }
        #endregion


        #region SERVICE METHODS PLUGIN

        public void GetInputData() {
            if (javaObject != null) {
                sbyte[] array = javaObject.Call<sbyte[]>("GetInputData");
                ConvertFromByte(array);
            }
        }

        private void ConvertFromByte(sbyte[] array) {
            byte[] byteArray = SByteToByteArray(array);
            if (array != null) {
                switch (array[0]) {
                    case 0:
                        if (ReceivedIntMessage != null) ReceivedIntMessage(BitConverter.ToInt32(byteArray, 1));
                        break;
                    case 1:
                        if (ReceivedFloatMessage != null) ReceivedFloatMessage(BitConverter.ToSingle(byteArray, 1));
                        break;
                    case 2:
                        if (ReceivedStringMessage != null) ReceivedStringMessage(Encoding.UTF8.GetString(ModRemoveArray(byteArray)));
                        break;
                    case 3:
                        if (ReceivedByteMessage != null) ReceivedByteMessage(ModRemoveArray(byteArray));
                        break;
                }
            }
            else Debug.Log("blue  =  ArrayNull");
        }

        private byte[] ModAddArray(byte[] array, int typeData) {
            byte[] lengthArray = BitConverter.GetBytes(array.Length + 1);
            byte[] newArray = new byte[array.Length + 5];
            newArray[0] = lengthArray[3];
            newArray[1] = lengthArray[2];
            newArray[2] = lengthArray[1];
            newArray[3] = lengthArray[0];
            newArray[4] = (byte)typeData;
            for (int i = 0; i < array.Length; i++) {
                newArray[i + 5] = array[i];
            }
            return newArray;
        }

        private byte[] ModRemoveArray(byte[] array) {
            byte[] newArray = new byte[array.Length - 1];
            for (int i = 0; i < array.Length - 1; i++) {
                newArray[i] = array[i + 1];
            }
            return newArray;
        }

        private sbyte[] ByteToSByteArray(byte[] array)
        {
            sbyte[] sBytes = new sbyte[array.Length];
            Buffer.BlockCopy(array, 0, sBytes, 0, array.Length);
            return sBytes;
        }

        private byte[] SByteToByteArray(sbyte[] array)
        {
            byte[] bytes = new byte[array.Length];
            Buffer.BlockCopy(array, 0, bytes, 0, array.Length);
            return bytes;
        }
        

        public void OnBtAdapterEnabled() {
            if (BtAdapterEnabled != null) BtAdapterEnabled();
        }
        public void OnBtAdapterDisabled() {
            if (BtAdapterDisabled != null) BtAdapterDisabled();
        }
        public void OnServerStarted() {
            if (ServerStarted != null) ServerStarted();
        }
        public void OnServerStopped() {
            if (ServerStopped != null) ServerStopped();
        }
        public void OnAttemptConnectToServer() {
            if (AttemptConnectToServer != null) AttemptConnectToServer();
        }
        public void OnFailConnectToServer() {
            if (FailConnectToServer != null) FailConnectToServer();
        }
        public void OnDeviceConnected() {
            if (DeviceConnected != null) DeviceConnected();
        }
        public void OnDeviceDisconnected() {
            if (DeviceDisconnected != null) DeviceDisconnected();
        }
        public void OnDeviceSelected(string deviceData) {
            if (DeviceSelected != null) DeviceSelected(deviceData);
        }
        private BTDevice[] GetBTArray(string[] devices) {
            BTDevice[] btDevices = new BTDevice[devices.Length / 2];
            int j = 0;
            for (int i = 0; i < devices.Length; i = i + 2) {
                btDevices[j].name = devices[i];
                btDevices[j].address = devices[i + 1];
                j++;
            }
            return btDevices;
        }

        private bool CheckPermissions()
        {
            var permissions = new[]
            {
                "android.permission.BLUETOOTH_SCAN", 
                "android.permission.BLUETOOTH_ADVERTISE", 
                "android.permission.BLUETOOTH_CONNECT"
            };
            var neededPermissions = new List<string>();

            foreach (var permission in permissions)
            {
                if (AndroidRuntimePermissions.CheckPermission(permission) != AndroidRuntimePermissions.Permission.Granted)
                    neededPermissions.Add(permission);
            }

            if (neededPermissions.Any())
            {
                var requestResults = AndroidRuntimePermissions.RequestPermissions(neededPermissions.ToArray());

                if (requestResults.Any(x => x != AndroidRuntimePermissions.Permission.Granted))
                    return false;
            }

            return true;
        }
        #endregion


        public class BluetoothBridgeForAndroid : MonoBehaviour {
            private BluetoothForAndroid btForUnity;

            void Start() {
                btForUnity = instance;
                DontDestroyOnLoad(gameObject);
                gameObject.name = "BluetoothForAndroid";
            }

            public void NewInputData(string message) {
                btForUnity.GetInputData();
            }
            public void BtAdapterEnabled(string message) {
                btForUnity.OnBtAdapterEnabled();
            }
            public void BtAdapterDisabled(string message) {
                btForUnity.OnBtAdapterDisabled();
            }
            public void ServerStarted(string message) {
                btForUnity.OnServerStarted();
            }
            public void ServerStopped(string message) {
                btForUnity.OnServerStopped();
            }
            public void AttemptConnectToServer(string message) {
                btForUnity.OnAttemptConnectToServer();
            }
            public void FailConnectToServer(string message) {
                btForUnity.OnFailConnectToServer();
            }
            public void DeviceConnected(string message) {
                btForUnity.OnDeviceConnected();
            }
            public void DeviceDisconnected(string message) {
                btForUnity.OnDeviceDisconnected();
            }
            public void DeviceSelected(string message) {
                btForUnity.OnDeviceSelected(message);
            }
        }
    }
}



