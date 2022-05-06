using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wow : MonoBehaviour
{
    private Animator anim;
    [SerializeField]private ScriptableInt wowKills;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(wowKills.value > 20)
        {
            anim.SetTrigger("Wow");
            wowKills.value = 0;
        }
    }
}
