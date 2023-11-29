using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bouncing : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }


    public float bounceRate = 0.1f;
    float currBounce = 0;
    int dir = 1;
    // Update is called once per frame
    void Update()
    {
        if (currBounce < -bounceRate && dir == 1) {
            dir = -1;
        } else if (currBounce > bounceRate && dir == -1) {
            dir = 1;
        }
    }
}
