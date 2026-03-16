using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RubberBand : MonoBehaviour
{
    private LineRenderer lr;

    public Transform bandAnchor;   // where the band attaches on the slingshot
    public Transform projectile;   // current projectile

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = 2;
    }

    void Update()
    {
        // always keep the first point at the slingshot
        lr.SetPosition(0, bandAnchor.position);

        if (projectile != null)
        {
            // stretch band to the ball
            lr.SetPosition(1, projectile.position);
        }
        else
        {
            // snap back to the slingshot
            lr.SetPosition(1, bandAnchor.position);
        }
    }

    public void SetProjectile(Transform proj)
    {
        projectile = proj;
    }

    public void ReleaseBand()
    {
        projectile = null;
    }
}