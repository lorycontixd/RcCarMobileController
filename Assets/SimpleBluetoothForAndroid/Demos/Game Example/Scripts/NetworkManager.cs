using System;
using SVSBluetooth;
using UnityEngine;
using UnityEngine.SceneManagement;

// this script serves as a bridge between the game script and the bluetooth plugin

namespace Simple_Bluetooth_for_android.Demos.Game_Example.Scripts
{
    public class NetworkManager : MonoBehaviour {
        public static NetworkManager instance; // singleton

        // subscription and unsubscribe to events
        private void OnEnable() {
            BluetoothForAndroid.DeviceDisconnected += ExitGameScene;
            BluetoothForAndroid.ReceivedByteMessage += GetMessage;
        }
        private void OnDisable() {
            BluetoothForAndroid.DeviceDisconnected -= ExitGameScene;
            BluetoothForAndroid.ReceivedByteMessage -= GetMessage;
        }

        void Start() {
            if (instance == null) instance = this; // creating singleton
        }

        // data transfer protocol
        // you can come up with any protocol

        // message[0] == 0 - change position
        // message[0] == 1 - shot
        // message[0] == 2 - hit
        // message[0] == 3 - destroy tank
        // message[0] == 4 - return to starting posion
        // when a message is received, methods from the GameScene script are called
        // Information about which method to call is contained in the first byte of the array
        void GetMessage(byte[] message) {
            switch ((int)message[0]) {
                case 0:
                    GameScene.instance.PutInBufferPosition(message);
                    break;
                case 1:
                    GameScene.instance.ShotEnemy();
                    break;
                case 2:
                    GameScene.instance.HitByPlayer(message[1]);
                    break;
                case 3:
                    GameScene.instance.DestroyTankPlayer();
                    break;
                case 4:
                    GameScene.instance.ReturnPlayerToStartingPosion();
                    break;
                default:
                    break;
            }
        }
        // message transfer
        public void WriteMessage(byte[] message) {
            BluetoothForAndroid.WriteMessage(message);
        }
        // go to menu and disconnect
        public void ExitGameScene() {
            BluetoothForAndroid.Disconnect();
            SceneManager.LoadScene(0);
        }
        // converting float to byte array and back from byte array to float
        public byte[] FloatToBytes(float f) {
            byte[] bytes = BitConverter.GetBytes(f);
            return bytes;
        }
        public float BytesToFloat(byte[] bytes) {
            float f = BitConverter.ToSingle(bytes, 0);
            return f;
        }
    }
}
