using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class Timer : MonoBehaviour
{
    private TextMeshProUGUI text;
    private float time;
    public bool playerAlive;

    //Get nessesary SribtableObjects
    [SerializeField]private ScriptableFloat playTime;
    [SerializeField]private ScriptableFloat level;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        playerAlive = true;

        StartCoroutine(TimePassed());        
    }

    //Timer
    IEnumerator TimePassed()
    {
        while(playerAlive)
        {
            text.text = $"Level: {level.value} // Time: {Math.Round(playTime.value, 2).ToString()}";
            yield return new WaitForSeconds(0.01f);
            playTime.value += 0.01f;
        }
    }

    //Save time to playTime scriptable object OnDisable
    void OnDisable()
    {
        playTime.value = time;
    }
}
