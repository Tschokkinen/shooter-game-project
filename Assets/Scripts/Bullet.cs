using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]private float bulletKillTime = 1.0f; //Delay used to kill stray bullets
    [SerializeField]Animator anim; //Animator
    [SerializeField]Rigidbody2D rb;
    [SerializeField]private bool enemyBullet; //Check if bullet prefab is used by enemy

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        AutoDestroy();
    }

    //Destroy bullet after delay if there's no hit
    private void AutoDestroy()
    {
        Destroy(gameObject, bulletKillTime);
    }

    //Check bullet and shotgun shell collision
    private void OnCollisionEnter2D(Collision2D other)
    {
            if(!enemyBullet && other.gameObject.tag == "Enemy")
            {
                Debug.Log("Hit enemy");
                Enemy enemy = other.gameObject.GetComponent<Enemy>(); //Get Enemy script
                enemy.Health(0.1f); //Reduce enemy health
                //Destroy(other.gameObject); //Destroy hit object
                DestroyAnimation(); //Trigger bullet explosion animation
                rb.velocity = new Vector2(0.0f, 0.0f); //Stop bullet when hit detected
                Destroy(gameObject, 0.3f); //Destroy bullet
            }
            else if(enemyBullet && other.gameObject.tag == "Player")
            {
                PlayerController playerController = other.gameObject.GetComponent<PlayerController>(); //Get PlayerController
                playerController.LooseHealth(0.1f); //Reduce player health
                DestroyAnimation(); //Trigger bullet explosion animation
                rb.velocity = new Vector2(0.0f, 0.0f); //Stop bullet when hit detected
                Destroy(gameObject, 0.3f); //Destroy bullet
            }
            else if(other.gameObject.tag == "Untagged")
            {
                DestroyAnimation(); //Trigger bullet explosion animation
                rb.velocity = new Vector2(0.0f, 0.0f);
                Destroy(gameObject, 0.3f); //Destroy bullet
            }     
    }

    //Bullet and grenade explosions
    private void DestroyAnimation()
    {
        anim.SetTrigger("Explode");
    }
}
