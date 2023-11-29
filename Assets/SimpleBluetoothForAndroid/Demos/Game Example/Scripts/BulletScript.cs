using UnityEngine;

namespace Simple_Bluetooth_for_android.Demos.Game_Example.Scripts
{
    public class BulletScript : MonoBehaviour {

        // collision check. If the projectile collides with a wall or with its own tank, then it is simply destroyed.
        // If the projectile collides with an enemy tank, then it is destroyed and calls the method of reducing HP in the GameScene script.
        private void OnCollisionEnter2D(Collision2D collision) {
            if (collision.gameObject.name == "LeftBorder" || collision.gameObject.name == "RightBorder" || collision.gameObject.name == "PlayerTank") {
                Destroy(gameObject);
            }
            else {
                GameScene.instance.HitByEnemy();
                Destroy(gameObject);
            }
        }
    }
}
