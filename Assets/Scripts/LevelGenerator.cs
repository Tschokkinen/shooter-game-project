using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private List<GameObject> spawnPoints;
    [SerializeField] private GameObject wall;
    [SerializeField] private ScriptableFloat level;
    [SerializeField] private float tileWidth;
    [SerializeField] private float tileHeight;
    [SerializeField] private int width;
    [SerializeField] private int height;

    private void Awake()
    {
        //Get a random starting block for size reference
        var reference = GetRandomBlock("Start");
        tileWidth = reference.GetComponentInChildren<Tilemap>().size.x;
        tileHeight = reference.GetComponentInChildren<Tilemap>().size.y;

        generateLevel();
    }

    public void generateLevel()
    {
        //First build surrounding wall and add prefab blocks with predefined spawn points
        BuildWall();
        InstantiateBlocks();
        NormalizeSpawnPoints();

        //Player must be added before pathfinding so player is found during scanning
        AddPlayer();
        AddPathfinding();
        AddBoss();
        InstantiateSpawners();
    }

    public void BuildWall()
    {
        Instantiate(wall);
    }

    private void InstantiateBlocks()
    {
        // Offset map starting point
        float startX = (tileWidth/2 * (-width + 1));
        float startY = 0;
        Vector2 spot = new Vector2(startX, startY); // The coordinates to place the tile

        // Lay blocks in a grid
        for (int rivi = 0; rivi < height; rivi++)
        {

            for (int i = 0; i < width; i++)
            {
                if (rivi == 0 && i == width-1)
                {
                    Instantiate(GetRandomBlock("Start"), spot, Quaternion.identity);
                }
                else if (rivi == height / 2 && i == width / 2) // Currently only works for odd sized levels. Might change if we actually need to change level size?
                {
                    Instantiate(GetRandomBlock("Center"), spot, Quaternion.identity);
                }
                else
                {
                    Instantiate(GetRandomBlock("City"), spot, Quaternion.identity);
                }
                // Update x coordinates for next tile
                spot.x += tileWidth/2;
            }
            // Start x from beginning and go to next row on the y-axis
            spot.x = startX;
            spot.y += tileHeight;
        }

    }

    private void NormalizeSpawnPoints()
    {
        //Get list of predefined spawnpoints and set z-axises to zero
        spawnPoints = new List<GameObject>(GameObject.FindGameObjectsWithTag("Spawn"));
        foreach (GameObject spawn in spawnPoints)
        {
            spawn.transform.position = new Vector3(spawn.transform.position.x, spawn.transform.position.y, 0);
        }
    }

    private void InstantiateSpawners()
    {
        //Select random spawn points and instantiate random enemy spawner on it. Amount increases with each level
        // Set cap for spawnpoints
        int maxSpawns = 13;
        float spawns;
        if (level.value < maxSpawns)
        {
            spawns = level.value;
        }
        else
        {
            spawns = maxSpawns;
        }
        for (int i = Mathf.FloorToInt(4 + level.value); i > 0; i--)
        {
            int enemy = Random.Range(0, 2);
            GameObject spawned;
            GameObject spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count-1)];
            if (enemy == 1)
            {
                spawned = Instantiate(Resources.Load<GameObject>("Spawnables/CircleSpawner"), spawnPoint.transform.position, Quaternion.identity);
            } else
            {
                spawned = Instantiate(Resources.Load<GameObject>("Spawnables/StarSpawner"), spawnPoint.transform.position, Quaternion.identity);
            }
            // Get the script of current spawner, enable respawn and set timer
            PrefabSpawner spawnScript = spawned.GetComponent<PrefabSpawner>();
            spawnScript.EnableRespawn();
            spawnScript.SetRespawnTime(5.0f - (level.value/3));
        }

    }

    private void AddPlayer()
    {
        //Find player spawn and instantiate player
        //Maybe use for respawning if we add lives for player?
        GameObject playerSpawn = GameObject.FindGameObjectsWithTag("Respawn")[0];
        GameObject playerInstance = Instantiate(Resources.Load<GameObject>("Spawnables/Player"), playerSpawn.transform.position, Quaternion.identity);

        //Remove "(clone)" from object name
        playerInstance.name = "Player";

    }
    // REFACTOR: ADD NULLCHECKS
    private void AddBoss()
    {
        //Get random spawn point from level
        int spawnPointIndex = Random.Range(0, spawnPoints.Count-1);
        
        Debug.Log(spawnPoints[spawnPointIndex].transform.position);

        //Add boss to spawn point
        GameObject bossInstance = Instantiate(Resources.Load<GameObject>("Spawnables/Boss"), spawnPoints[spawnPointIndex].transform.position, Quaternion.identity);

        //Remove "(clone)" from object name
        bossInstance.name = "Boss";

    }

    private void AddPathfinding()
    {
        Instantiate(Resources.Load<GameObject>("Spawnables/Pathfinding"));
    }

    //Find random block from resources folders
    private GameObject GetRandomBlock(string type)
    {
        int number;
        int range;
        if (type == "Center")
        {
            range = 3;
        } else if (type == "Start")
        {
            range = 1;
        }else
        {
            range = 5;
        }
        number = Random.Range(1, range);
        Debug.Log("CityBlocks/" + type + "/" + type + "Block0" + number);
        GameObject block = Resources.Load<GameObject>("CityBlocks/" + type + "/" + type + "Block0" + number);

        return block;
    }

}
