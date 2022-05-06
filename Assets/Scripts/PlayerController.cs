using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Animators")]
    [SerializeField] private Animator anim;
    [SerializeField]private Animator niceTohave;
    [Header("Various")]
    [SerializeField]private ScriptableBool isPaused;
    [Header("Audio")]
    [SerializeField]private AudioSource gunfire;
    [Header("Healthbar")]
    [SerializeField]private Healthbar healthBar;
    [Header("Movement")]
    private Vector2 movement;
    [SerializeField] private Camera aimCam;
    private Vector2 mouse;
    [SerializeField] private float movementSpeed = 12f;
    [Header("Guns and firing")]
    [SerializeField]private Rigidbody2D bullet;
    [SerializeField]private Rigidbody2D grenade;
    //[SerializeField]private Rigidbody2D shotgunShell;
    [SerializeField]private Transform gun;
    [SerializeField]private List<Transform> shells = new List<Transform>();
    [SerializeField]private float shotgunRecoil = 30.0f;
    [SerializeField]private float smgRecoil = 500.0f;
    [SerializeField]private float bulletForce = 10.0f;
    [SerializeField]private float timeBetweenShots;
    [SerializeField]private ParticleSystem ps; //Particle system component on child object "Gun"
    [Header("Grenade")]
    [SerializeField]private float grenadeForce = 4.0f;
    [SerializeField]private float grenadeThrowInterval = 2.0f;
    [SerializeField]private Image grenadeDelayIndicator;
    private bool canThrow = true; //Bool to check if player can throw grenade
    [Header("Player UI")]
    [SerializeField]private GameObject PistolUI;
    [SerializeField]private GameObject SMGUI;
    [SerializeField]private GameObject ShotgunUI;
    [SerializeField]private TextMeshProUGUI AmmoCounter;
    [SerializeField]private GameObject Grenades1UI;
    [SerializeField]private GameObject Grenades2UI;
    [SerializeField]private GameObject Grenades3UI;

    //CLEANUP: Maybe we could refactor UI elements to separate controller script and use methods in this class to call the UIController methods?

    private bool readyToFire = true;
    private int grenades = 1;
    private int ammo = 0;
    private string activeWeapon = "Pistol";
    private Rigidbody2D rb;

    TextMeshProUGUI nadesFull;

    // Start is called before the first frame update
    void Start()
    { 
        //Get all components (back up if inspector assignment fails or is missing)
        niceTohave = GameObject.Find("NiceToHave").GetComponent<Animator>();
        healthBar = GameObject.Find("Healthbar").GetComponent<Healthbar>();
        rb = GetComponent<Rigidbody2D>();
        aimCam = GameObject.Find("Aim Camera").GetComponent<Camera>();
        PistolUI = GameObject.Find("PistolUI");
        ShotgunUI = GameObject.Find("ShotgunUI");
        SMGUI = GameObject.Find("SMGUI");
        AmmoCounter = GameObject.Find("AmmoCountUI").GetComponent<TextMeshProUGUI>();
        Grenades1UI = GameObject.Find("Grenades1UI");
        Grenades2UI = GameObject.Find("Grenades2UI");
        Grenades3UI = GameObject.Find("Grenades3UI");
        nadesFull = GameObject.Find("NadesFull").GetComponent<TextMeshProUGUI>();

        grenadeDelayIndicator = transform.Find("Canvas").GetComponentInChildren<Image>();
        grenadeDelayIndicator.gameObject.SetActive(false);

        nadesFull.gameObject.SetActive(false);

        anim = GetComponent<Animator>();

        UpdateGrenades();
    }

    // Update is called once per frame
    void Update()
    {
        //Get directional inputs
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        //Get mouse position
        mouse = aimCam.ScreenToWorldPoint(Input.mousePosition);

        Fire();
        CheckAmmo();
    }

    private void FixedUpdate()
    {
        //Move and animate player
        movement.Normalize();
        rb.MovePosition(rb.position + movement * movementSpeed * Time.fixedDeltaTime);
        anim.SetFloat("Speed", movement.magnitude);

        //Set Rotation based on mouse position
        Vector2 direction = mouse - rb.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        rb.rotation = angle;
    }

    //Gun firing mechanism
    private void Fire()
    {
        if(isPaused.value == false)
        {
            //Firing mechanism for pistol
            if ((Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Space)) && activeWeapon == "Pistol")
            {
                Rigidbody2D bulletInstance = Instantiate(bullet, gun.position, gun.rotation) as Rigidbody2D;
                bulletInstance.velocity = bulletForce * gun.up;
                ps.Play(); //Play particle system
                gunfire.Play();
            }
            //Shotgun firing mechanism
            else if ((Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Space)) && activeWeapon == "Shotgun")
            {
                ammo--;

                foreach (Transform transform in shells)
                {
                    Rigidbody2D bulletInstance = Instantiate(bullet, transform.position, transform.rotation) as Rigidbody2D;
                    bulletInstance.velocity = bulletForce * transform.up;
                    ps.Play(); //Play particle system
                    SetAmmoText();
                }

                rb.AddForce(-transform.up * shotgunRecoil);
                gunfire.Play();

            }
            //Automatic firing mechanism for SMG when in hand
            else if ((Input.GetKey(KeyCode.Mouse0) || Input.GetKey(KeyCode.Space)) && activeWeapon == "SMG" && readyToFire == true)
            {
                Rigidbody2D bulletInstance = Instantiate(bullet, gun.position, gun.rotation) as Rigidbody2D;
                bulletInstance.velocity = bulletForce * gun.up;
                ps.Play();  //Play particle system
                ammo--;
                SetAmmoText();
                readyToFire = false;
                Invoke("FireRate", timeBetweenShots);
                rb.AddForce(-transform.up * smgRecoil);
                gunfire.Play();
            }
            //Trown grenade with mouse1
            else if ((Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.E)) && canThrow && grenades > 0)
            {
                Rigidbody2D bulletInstance = Instantiate(grenade, gun.position, gun.rotation) as Rigidbody2D;
                bulletInstance.velocity = grenadeForce * gun.up;
                StartCoroutine(GrenadeThrowDelay());
                grenades--;
                UpdateGrenades();
            }
        } 
    }

    //Controls grenade throw interval
    IEnumerator GrenadeThrowDelay()
    {
        canThrow = false;
        grenadeDelayIndicator.gameObject.SetActive(true);

        while(grenadeThrowInterval > 0)
        {
            yield return new WaitForSeconds(0.1f);
            grenadeThrowInterval -= 0.1f;
            grenadeDelayIndicator.fillAmount -= 0.05f;
        }
        
        grenadeDelayIndicator.fillAmount = 1.0f;
        grenadeDelayIndicator.gameObject.SetActive(false);
        grenadeThrowInterval = 2.0f;
        canThrow = true;
    }

    //Method to limit fire rate when called with Invoke
    private void FireRate()
    {
        readyToFire = true;
    }

    //WARNING! Healthbar is sometimes accessed by enemies directly through Healthbar game object. 
    //Should everything go through playerController instead?
    public void LooseHealth(float value)
    {
        healthBar.ReduceHealth(value);
    }

    //Pick up for weapons
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("SMGPU"))
        {
            Destroy(other.gameObject);
            PistolUI.gameObject.SetActive(false);
            ShotgunUI.gameObject.SetActive(false);
            SMGUI.gameObject.SetActive(true);
            activeWeapon = "SMG";
            ammo = 30;
            SetAmmoText();
        }
        else if (other.gameObject.CompareTag("ShotgunPU"))
        {
            Destroy(other.gameObject);
            PistolUI.gameObject.SetActive(false);
            ShotgunUI.gameObject.SetActive(true);
            SMGUI.gameObject.SetActive(false);
            activeWeapon = "Shotgun";
            ammo = 6;
            SetAmmoText();
        }
        else if (other.gameObject.CompareTag("Grenade") && grenades < 3)
        {
            Destroy(other.gameObject);
            grenades++;
            UpdateGrenades();
            niceTohave.SetTrigger("Slide");
        }
        else if(other.gameObject.CompareTag("Grenade") && grenades > 2)
        {
            StartCoroutine(FadeInText(1f, nadesFull));
        }
    }

    //Updates UI according to amount of grenades player is holding
    private void UpdateGrenades()
    {
        if (grenades == 0)
        {
            Grenades1UI.gameObject.SetActive(false);
            Grenades2UI.gameObject.SetActive(false);
            Grenades3UI.gameObject.SetActive(false);
        }
        else if (grenades == 1)
        {
            Grenades1UI.gameObject.SetActive(true);
            Grenades2UI.gameObject.SetActive(false);
            Grenades3UI.gameObject.SetActive(false);
        }
        else if (grenades == 2)
        {
            Grenades1UI.gameObject.SetActive(false);
            Grenades2UI.gameObject.SetActive(true);
            Grenades3UI.gameObject.SetActive(false);
        }
        else if (grenades == 3)
        {
            Grenades1UI.gameObject.SetActive(false);
            Grenades2UI.gameObject.SetActive(false);
            Grenades3UI.gameObject.SetActive(true);
        }
    }

    //Updates ammo counter
    private void SetAmmoText()
    {
        AmmoCounter.text = ammo.ToString();
    }

    //Checks if ammo is 0 and switches to pistol
    private void CheckAmmo()
    {
        if (ammo <= 0)
        {
            PistolUI.gameObject.SetActive(true);
            ShotgunUI.gameObject.SetActive(false);
            SMGUI.gameObject.SetActive(false);
            activeWeapon = "Pistol";
            AmmoCounter.text = "\u221E";
        }
    }

    //Mehod for fading in text to screen
    private IEnumerator FadeInText(float t, TextMeshProUGUI i)
    {
        nadesFull.gameObject.SetActive(true);

        i.color = new Color(i.color.r, i.color.g, i.color.b, 0);
        while (i.color.a < 1.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a + (Time.deltaTime / t));
            yield return null;
        }
        //When text is completely faded in, starts method to fade it out
        StartCoroutine(FadeOutText(1f, nadesFull));
    }

    //Method for fading out text from screen
    private IEnumerator FadeOutText(float t, TextMeshProUGUI i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 1);
        while (i.color.a > 0.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / t));
            yield return null;
        }
        nadesFull.gameObject.SetActive(false);
    }
}
