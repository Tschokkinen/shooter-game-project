using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    //Singleton
    public static EventManager instance { get; private set; }

    //Event delegate
    public delegate void ChangeEnemyBehavior(bool gameStatus);
    public static event ChangeEnemyBehavior changeEnemyBehavior;
    [SerializeField]public bool changeBehavior = false;

    public delegate void ShakeCameraEvent(string action);
    public static event ShakeCameraEvent shakeCameraEvent;

    void Start()
    {
        instance = this;
    }

    //Function used for test purposes
    public void UpdateEnemyBehavior()
    {
        changeBehavior = true;
        ChangeEnemyBehaviorStatus(changeBehavior);
    }

    //Send out event message when called
    void ChangeEnemyBehaviorStatus(bool value)
    {
        if(changeEnemyBehavior != null)
        {
            changeEnemyBehavior(value);
        }
    }

    //Trigger to check if player is close enough to the boss character
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            changeBehavior = true;
            ChangeEnemyBehaviorStatus(changeBehavior);
            //UpdateEnemyBehavior();
            Debug.Log("Update enemy behavior event triggered");
        }
    }

    //Trigger ShakeCamera event
    public void ShakeCamera()
    {
        if(shakeCameraEvent != null)
        {
            shakeCameraEvent("Shake");
        }
    }
}
