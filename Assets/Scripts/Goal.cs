using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class Goal : MonoBehaviour
{
    // a static field accessible by code anywhere
    static public bool goalMet = false;


    void OnTriggerEnter(Collider other)
    {
        //when the trigger is hit by something
        // check to see if it is a projectile
        Projectile proj = other.GetComponent<Projectile>();
        if (proj != null)
        {
            // if so set goalMet to true
            Goal.goalMet = true;
            //also set the alpha of the color to higher opacity
            Material mat = GetComponent<Renderer>().material;
            Color c = mat.color;
            c.a = 0.75f;
            mat.color = c;
        }
    }
}
