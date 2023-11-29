using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Simple_Bluetooth_for_android.Demos.Game_Example.Scripts
{
    public class GameScene : MonoBehaviour {
        public static GameScene instance;
        // Objects
        public GameObject tankPlayer;
        public GameObject tankEnemy;
        public GameObject bullet;
        // Text fields
        public Text playerHealthText;
        public Text enemyHealthText;
        public Text scoreText;
        // Game variables
        int playerHealth = 100;
        int enemyHealth = 100;
        int playerDeath;
        int enemyDeath;
        // Input
        bool goUp;
        bool goDown;
        // Fields for interpolating the movement of an enemy
        Queue<Vector2> posEnemyBuffer = new Queue<Vector2>();
        bool bufferReady;
        int frame;
        int stepToNewPoint;
        Vector2 step;

        void Start() {
            Application.targetFrameRate = 30;
            if (instance == null) instance = this; // creating singleton
            UpdateVievHealth();
            UpdateScore();
            float timeRepeat = 1f / 30f;
            InvokeRepeating("MovePlayer", 0f, timeRepeat);
            InvokeRepeating("MoveEnemy", timeRepeat, timeRepeat);
            InvokeRepeating("SendPosition", timeRepeat, timeRepeat);
        }


        ////////////////////////
        // Methods for the tank of this device
        ////////////////////////

        // player tank movement when pushing buttons
        void MovePlayer() {
            if (playerHealth > 0) {
                if (goUp || goDown) {
                    int direction = goUp ? 1 : -1;
                    Vector2 vector = tankPlayer.transform.position;
                    vector.y = Mathf.Clamp(vector.y + 10 * Time.deltaTime * direction, -2.8f, 2.8f);
                    tankPlayer.transform.position = vector;
                }
            }
        }

        // wrapping a player’s tank position into an array of bytes and transferring this array to an enemy device
        void SendPosition() {
            byte[] position = new byte[9];
            byte[] posX = new byte[4];
            byte[] posY = new byte[4];
            posX = NetworkManager.instance.FloatToBytes(tankPlayer.transform.position.x);
            posY = NetworkManager.instance.FloatToBytes(tankPlayer.transform.position.y);
            position[0] = 0;
            position[1] = posX[0];
            position[2] = posX[1];
            position[3] = posX[2];
            position[4] = posX[3];
            position[5] = posY[0];
            position[6] = posY[1];
            position[7] = posY[2];
            position[8] = posY[3];
            NetworkManager.instance.WriteMessage(position); // message transfer
        }

        // delay between shots
        IEnumerator ShotPlayer() {
            while (true) {
                if (playerHealth > 0) {
                    Vector2 vector = tankPlayer.transform.position;
                    vector.x += 1;
                    GameObject newBullet = Instantiate(bullet, vector, Quaternion.identity);
                    newBullet.GetComponent<Rigidbody2D>().AddForce(Vector2.right * 100);
                    NetworkManager.instance.WriteMessage(new byte[1] { (byte)1 }); // message transfer
                }
                yield return new WaitForSeconds(0.3f);
            }
        }

        // the reduction of the HP of the enemy tank and the destruction of the tank when the HP is equal to zero
        public void HitByEnemy() {
            if (enemyHealth > 20) {
                enemyHealth -= 20;
                NetworkManager.instance.WriteMessage(new byte[2] { (byte)2, (byte)enemyHealth }); // message transfer
                UpdateVievHealth();
            }
            else if (enemyHealth == 20) {
                enemyHealth = 0;
                NetworkManager.instance.WriteMessage(new byte[1] { (byte)3 }); // message transfer
                UpdateVievHealth();
                tankEnemy.transform.position = new Vector2(3.256f, 20);
                Invoke("ReturnEnemyToStartingPosion", 1f);
                enemyDeath++;
                UpdateScore();
            }
        }

        // the return of the enemy’s tank to the starting position a second after its destruction
        void ReturnEnemyToStartingPosion() {
            tankEnemy.transform.position = new Vector2(3.256f, 0);
            enemyHealth = 100;
            NetworkManager.instance.WriteMessage(new byte[1] { (byte)4 }); // message transfer
            posEnemyBuffer.Clear();
            UpdateVievHealth();
        }


        /////////////////////////////
        // Commands from another device
        /////////////////////////////

        // If the buffer is not empty, then the movement of the enemy tank begins.
        // tank movement
        void MoveEnemy() {
            if (enemyHealth > 0) {
                if (posEnemyBuffer.Count > 0) tankEnemy.transform.position = posEnemyBuffer.Dequeue();
            }
        }

        // The resulting positions are converted from an array of bytes to coordinates of type Vector2 and added to the buffer.
        public void PutInBufferPosition(byte[] position) {
            Vector2 currentPosition;
            byte[] posX = new byte[4];
            byte[] posY = new byte[4];
            posX[0] = position[1];
            posX[1] = position[2];
            posX[2] = position[3];
            posX[3] = position[4];
            posY[0] = position[5];
            posY[1] = position[6];
            posY[2] = position[7];
            posY[3] = position[8];
            currentPosition.x = -NetworkManager.instance.BytesToFloat(posX);
            currentPosition.y = NetworkManager.instance.BytesToFloat(posY);
            posEnemyBuffer.Enqueue(currentPosition);
        }

        // shot of an enemy tank
        public void ShotEnemy() {
            Vector2 vector = tankEnemy.transform.position;
            vector.x -= 1;
            GameObject newBullet = Instantiate(bullet, vector, Quaternion.identity);
            newBullet.GetComponent<Rigidbody2D>().AddForce(-Vector2.right * 100);
        }

        // hitting the player's tank
        public void HitByPlayer(byte health) {
            playerHealth = (int)health;
            UpdateVievHealth();
        }

        // player tank destruction
        public void DestroyTankPlayer() {
            tankPlayer.transform.position = new Vector2(-3.256f, 20);
            playerHealth = 0;
            playerDeath++;
            UpdateVievHealth();
            UpdateScore();
        }

        // the return of the player’s tank to the starting position
        public void ReturnPlayerToStartingPosion() {
            tankPlayer.transform.position = new Vector2(-3.256f, 0);
            frame = 0;
            playerHealth = 100;
            UpdateVievHealth();
        }


        // View
        void UpdateVievHealth() {
            playerHealthText.text = playerHealth + "%";
            enemyHealthText.text = enemyHealth + "%";
        }
        void UpdateScore() {
            scoreText.text = playerDeath + ":" + enemyDeath;
        }
        // Input
        public void UpButtonPointerDown() {
            goUp = true;
        }
        public void UpButtonPointerUp() {
            goUp = false;
        }
        public void DownButtonPointerDown() {
            goDown = true;
        }
        public void DownButtonPointerUp() {
            goDown = false;
        }
        public void ShotButtonPointerDown() {
            StartCoroutine("ShotPlayer");
        }
        public void ShotButtonPointerUp() {
            StopCoroutine("ShotPlayer");
        }
        public void ExitScene() {
            NetworkManager.instance.ExitGameScene();
        }
    }
}
