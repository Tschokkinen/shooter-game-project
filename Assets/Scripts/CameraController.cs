using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Animator anim;
    private Vector2 playerPosition;
    [SerializeField]private GameObject player;

    //Subscribe Main Camera to shakeCameraEvent
    void OnEnable()
    {
        if(gameObject.name == "Main Camera")
        {
            EventManager.shakeCameraEvent += ShakeCamera;
        }
    }

    //Unsubscribe Main Camera to shakeCameraEvent
    void OnDisable()
    {
        if(gameObject.name == "Main Camera")
        {
            EventManager.shakeCameraEvent -= ShakeCamera;
        }
    }

    private void Start()
    {
        if(gameObject.name == "Main Camera")
        {
            anim  = GetComponent<Animator>();
        }
        
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if(player != null)
        {
            playerPosition = player.transform.position;
        }
        //Debug.Log(playerPosition);
    }

    private void LateUpdate()
    {
        if(player != null)
        {
            transform.position = new Vector3(playerPosition.x, playerPosition.y, -15);
        }
    }

    //Camera action
    private void ShakeCamera(string action)
    {
        Debug.Log("Camera is shaking!");
        anim.SetTrigger("Shake");
    }
}
