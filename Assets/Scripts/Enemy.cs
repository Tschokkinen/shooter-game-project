using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [SerializeField]private AudioSource gunfire;
    [SerializeField]private bool inBossZone;
    [Header("ScribtableObjects")]
    [SerializeField]private ScriptableBool ExitGateActive;
    [SerializeField]private ScriptableFloat level;
    [SerializeField]private ScriptableInt bossKills;
    [Header("Player healthbar")]
    [SerializeField]private Healthbar healthBar;
    [SerializeField]private BossIndicator bossIndicator;
    [Header("Enemy health")]
    [SerializeField]private float enemyHealth;
    [SerializeField]private float enemyBaseSpeed;
    [SerializeField]private ScriptableInt kills;
    [SerializeField]private ScriptableInt wowKills;
    [SerializeField]private SpriteRenderer spriteRenderer;
    [Header("Enemy bloodspatter on death")]
    [SerializeField]private GameObject bloodSpatter;
    [Header("Enemy speed after changeEnemyBehavior")]
    [SerializeField]private float upgradedSpeed = 10.0f;
    [Header("Firing related")]
    [SerializeField]private Rigidbody2D enemyBullet;
    [SerializeField]private Transform gun;
    [SerializeField]private float bulletForce = 10.0f;    
    [SerializeField]private float fireRate = 2.5f;
    [SerializeField]private bool nearPlayer = false;
    //[SerializeField]private float nextFire;
    [Header("Check if enemy is boss")]
    [SerializeField]private bool isBoss = false;
    [SerializeField]private Rigidbody2D rb;
    [Header("A* related")]
    [SerializeField]private AIPath aIPath;
    [Header("For drops")]
    [SerializeField]private GameObject ShotgunPU;
    [SerializeField]private GameObject SMGPU;
    [SerializeField]private GameObject GrenadePU;


    private Transform target;

    //List for spawned pick ups
    List<GameObject> spawnedPickUps = new List<GameObject>();

    //Subscribe to EventManager on enable
    void OnEnable()
    {
        EventManager.changeEnemyBehavior += MovementSpeed;
    }

    //Unsubscribe from EventManager on enable
    void OnDisable()
    {
        EventManager.changeEnemyBehavior -= MovementSpeed;
    }

    void Start()
    {
        //Get nessesary Components
        target = GameObject.Find("Player").GetComponent<Transform>();
        healthBar = GameObject.Find("Healthbar").GetComponent<Healthbar>();
        bossIndicator = GameObject.Find("BossIndicator").GetComponent<BossIndicator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        inBossZone = false;
        
        aIPath = GetComponent<AIPath>();

        //Reset boss zone
        inBossZone = false;

        //Set boss as inactive
        if (isBoss)
        {
            aIPath.enabled = false;
            rb = GetComponent<Rigidbody2D>();
            rb.isKinematic = true;
        }

        //Sets enemy stats according to level
        UpdateEnemyToLevel();

        //Coroutine to check range of player
        StartCoroutine(AggroRange());
    }

    // Update is called once per frame
    void Update()
    {
        //Lassi's own basic tracking
        /*transform.position = Vector2.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
        transform.right = target.position - transform.position;*/
    }

    //Collision detector to detect when enemy reaches player
    /*
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log(this + " hit " + other);
            //Destroy(this.gameObject);
            
            //healthBar.ReduceHealth(0.1f);
        }
    }
    */

    /*
    //Non coroutine solution
    private void Fire()
    {
        if(Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            Fire();
        }
    }
    */

    //Limits the range of A* Pathfinding
    IEnumerator AggroRange()
    {
        for(; ; )
        {
            if (target != null)
            {
                float distance = Vector2.Distance(this.transform.position, target.transform.position);
                if (distance > 35)
                {
                    aIPath.enabled = false;
                }

                else if (!isBoss)
                {
                    aIPath.enabled = true;
                }
            }
            //Checks range every 0.1 seconds
            yield return new WaitForSeconds(0.1f);
        }
    }

    //Fire at the player on set intervals
    IEnumerator Fire()
    {
        while(nearPlayer)
        {
            Rigidbody2D bulletInstance = Instantiate(enemyBullet, gun.position, gun.rotation) as Rigidbody2D;
            bulletInstance.velocity = bulletForce * gun.up;
            gunfire.Play();
            yield return new WaitForSeconds(fireRate);
        }
    }

    //Controll enemies speed based on base stats and the level player is on
    public void UpdateEnemyToLevel()
    {
        if (isBoss == false)
        {
            enemyHealth += level.value * 0.2f;
            aIPath.maxSpeed = enemyBaseSpeed + level.value;
            Debug.Log("Healt: " + enemyHealth * 10f + ", Speed: " + aIPath.maxSpeed);
        }
    }

    //Control how enemies take damage
    public void Health(float damage)
    {
        if(isBoss && inBossZone)
        {
            enemyHealth -= damage;
        }
        else if(!isBoss)
        {
            enemyHealth -= damage;
        }

        StartCoroutine(Hit());

        if(enemyHealth < 0)
        {
            Dead();
        }
    }

    //Change enemy color momentarily when hit
    IEnumerator Hit()
    {
        if(isBoss && inBossZone)
        {
            spriteRenderer.color = Color.black;
        }
        else
        {
            spriteRenderer.color = Color.red;
        }
        
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = Color.white;
    }

    //Check when player is within enemy vicinity or if in boss zone
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            nearPlayer = true;
            StartCoroutine(Fire());
            Debug.Log("Fire");
        } else if (other.gameObject.CompareTag("BossZone"))
        {
            inBossZone = true;
        }
    }

    //If player is not withing enemy vicinityor if in boss zone
    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            nearPlayer = false;
            Debug.Log("Don't fire");
        }
        if (other.gameObject.CompareTag("BossZone"))
        {
            inBossZone = false;
        }
    }

    //Enemy dead
    public void Dead()
    {
        if (isBoss && inBossZone) //If enemy is a boss and in boss zone
        {
            ExitGateActive.value = true;
            bossKills.value++;
            bossIndicator.Disable();
            Instantiate(bloodSpatter, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
        else if(!isBoss) //If enemy is not a boss
        {
            Instantiate(bloodSpatter, transform.position, Quaternion.identity);
            RandomDrops();
            kills.value++;
            wowKills.value++;
            Destroy(gameObject);
            bossIndicator.Shrink();
        }

    }

    //Change enemy speed according to EventManager bool value
    private void MovementSpeed(bool value)
    {
        if(value)
        {
            Debug.Log("More enemy speed.");
            aIPath.maxSpeed = upgradedSpeed;

            if(isBoss)
            {
                aIPath.enabled = true;
                rb.isKinematic = false;
            }
        }
    }

    //Method to drop items randomly
    private void RandomDrops()
    {
        int number = Random.Range(1, 10);

        if(number == 1)
        {
            spawnedPickUps.Add(Instantiate(ShotgunPU, this.transform.position, Quaternion.Euler(0, 0, 0)) as GameObject);
        }

        else if (number == 2)
        {
            spawnedPickUps.Add(Instantiate(SMGPU, this.transform.position, Quaternion.Euler(0, 0, 0)) as GameObject);
        }
        else if (number == 3)
        {
            spawnedPickUps.Add(Instantiate(GrenadePU, this.transform.position, Quaternion.Euler(0, 0, 0)) as GameObject);
        }
    }
}