using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pillbox : MonoBehaviour
{
    [Header("Firing related")]
    [SerializeField]private AudioSource gunfire;
    [SerializeField]private Rigidbody2D enemyBullet;
    [SerializeField]private Transform[] guns;
    [SerializeField]private float bulletForce = 10.0f;    
    [SerializeField]private float fireRate = 2.5f;
    [SerializeField]private bool nearPlayer = false;

    //Pillbox firing coroutine
    IEnumerator Fire()
    {
        while(nearPlayer)
        {
            foreach(Transform transform in guns)
            {
                Rigidbody2D bulletInstance = Instantiate(enemyBullet, transform.position, transform.rotation) as Rigidbody2D;
                bulletInstance.velocity = bulletForce * transform.up;
            }
            gunfire.Play();
            yield return new WaitForSeconds(fireRate);
        }
    }

    //Check when player is within enemy vicinity
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            nearPlayer = true;
            StartCoroutine(Fire());
            //Debug.Log("Fire");
        }
    }

    //Check when player is not within enemy vicinity
    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            nearPlayer = false;
            //Debug.Log("Don't fire");
        }
    }
}
