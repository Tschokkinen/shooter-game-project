using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabSpawner : MonoBehaviour
{
    //Modifiers for spawner
    [SerializeField] private GameObject whatPrefabToSpawn;
    [SerializeField] private float minDistance;
    [SerializeField] private float maxDistance;
    [SerializeField] private bool respawning = false;
    [SerializeField] private float timeBetweenRespawns;

    //Define target
    private Transform target;

    //List for spawned prefabs
    List<GameObject> spawnedPrefebs = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        //Find player transform
        target = GameObject.Find("Player").GetComponent<Transform>();

        Spawn();
    }

    //Method for spawning prefabs
    private void Spawn()
    {
        //Checks if target is found to prevent crashes
        if (target != null)
        {
            //Get distance from player to spawner
            float distance = Vector2.Distance(this.transform.position, target.transform.position);

            //Spawns prefab is player is in defined range
            if (distance < maxDistance && distance > minDistance)
            {
                spawnedPrefebs.Add(Instantiate(whatPrefabToSpawn, this.transform.position, Quaternion.Euler(0, 0, 0)) as GameObject);
            }

            //Calls method again in defined time if respawning is set on
            if (timeBetweenRespawns != 0 && respawning == true)
            {
                Invoke("Spawn", timeBetweenRespawns);
            }
        } 
    }

    //Get max distance.         IS THIS USED?
    public float GetMaxDistance()
    {
        return maxDistance;
    }

    //Method that LevelGenerator uses to change setting
    public void EnableRespawn()
    {
        this.respawning = true;
    }

    //Method that LevelGenerator uses to change setting
    public void SetRespawnTime(float value)
    {
        this.timeBetweenRespawns = value;
    }
}
