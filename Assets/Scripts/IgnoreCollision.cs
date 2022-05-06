using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreCollision : MonoBehaviour
{
    [SerializeField]private int layerOne;
    [SerializeField]private int layerTwo;

    // Start is called before the first frame update
    void Start()
    {
        //Ignore collision between layers
        Physics2D.IgnoreLayerCollision(layerOne, layerTwo, true);
    }
}
