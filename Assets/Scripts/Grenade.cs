using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField]private AudioSource explosion;
    [SerializeField]Animator anim; //Animator
    [SerializeField]Rigidbody2D rb;
    [Header("Explosion radius")]
    [SerializeField]private float explosionRadius = 2.0f;

    [Header("Time before explosion")]
    [SerializeField]private float grenadeExplosionTime = 0.5f;

    [Header("Layers")]
    [SerializeField]private LayerMask enemyLayer = 0; //Enemy layer required by grenade explosion radius check
    [SerializeField]private LayerMask playerLayer = 3; //Player layer required by grenade explosion radius check
    
    [Header("Grenade collider")]
    [SerializeField]private Collider2D grenadeCollider; //If bullet is a grenade, get grenade collider
    
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        grenadeCollider = GetComponent<Collider2D>();
        StartCoroutine(GrenadeDelay());
    }

    //Explode grenade after time
    IEnumerator GrenadeDelay()
    {
        yield return new WaitForSeconds(grenadeExplosionTime);
        GrenadeExplosion();
        //Debug.Log("Coroutine done.");
    }

    /*
    //Used to visually test grenade explosion area
    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 2.0f);
    }
    */

    //Grenade explosion logic (if isGrenade == true)
    private void GrenadeExplosion()
    {
        grenadeCollider.isTrigger = true;
        DestroyAnimation();
        Debug.Log("Grenade explosion!");

        //Get nearby enemy colliders on grenade explosion
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius, enemyLayer);

        //Get player collider (if within explosion radius) on grenade explosion
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, explosionRadius, playerLayer);

        //Go through hit colliders
        if(colliders != null)
        {
            for(int i = 0; i < colliders.Length; i++)
            {
                Rigidbody2D hit = colliders[i].GetComponent<Rigidbody2D>();
                Enemy enemy = colliders[i].GetComponent<Enemy>();

                try
                {
                    enemy.Dead(); //Grenade could instantiate the blood spatter here instead of having enemy script do it.
                    //Destroy(hit.gameObject);
                }
                catch
                {
                    Debug.Log("No enemy");
                }
            }
        }

        if(playerCollider != null)
        {
            Debug.Log("Player collider hit by grenade!");
            PlayerController playerController = playerCollider.GetComponent<PlayerController>();
            playerController.LooseHealth(0.2f);
        }

        explosion.Play();
        EventManager.instance.ShakeCamera(); //Trigger cameraShake animation on Main Camera
        
        rb.velocity = new Vector2(0.0f, 0.0f);
        Destroy(gameObject, 0.3f);
    }

    //Bullet and grenade explosions
    private void DestroyAnimation()
    {
        anim.SetTrigger("GrenadeExplosion");
    }
}
