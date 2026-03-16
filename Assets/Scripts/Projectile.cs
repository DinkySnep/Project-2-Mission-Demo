using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class Projectile : MonoBehaviour
{
    const int LOOKBACK_COUNT = 10;
    static List<Projectile> PROJECTILES = new List<Projectile>();

    private AudioSource audioSource;
    private bool whirrPlayed = false;

    [SerializeField]
    private bool _awake = true;
    public bool awake
    {
        get { return _awake; }
        private set { _awake = value; }
    }

    private Vector3 prevPos;
    // This private list stored the history of the projectile's move distance
    private List<float> deltas = new List<float>();
    private Rigidbody rigid;

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        awake = true;
        prevPos = new Vector3(1000, 1000, 0);
        deltas.Add(1000);

        PROJECTILES.Add(this);
    }

    void FixedUpdate()
    {
        if (rigid.isKinematic || !awake) return;

        if (!whirrPlayed && audioSource != null)
        {
            audioSource.Play();
            whirrPlayed = true;  // ensure it only plays once
        }

        Vector3 deltaV3 = transform.position - prevPos;
        deltas.Add(deltaV3.magnitude);
        prevPos = transform.position;

        // limit lookback; one of very few time that we use while
        while (deltas.Count > LOOKBACK_COUNT)
        {
            deltas.RemoveAt(0);
        }

        // Iterate over deltas and find the greatest one
        float maxDelta = 0;
        foreach (float f in deltas)
        {
            if (f > maxDelta) maxDelta = f;
        }

        // if the projectile hasn't moved more than the sleep threshold
        if (maxDelta <= Physics.sleepThreshold)
        {
            // set awake to fale and put rigidbody to sleep
            audioSource.Stop();
            awake = false;
            rigid.Sleep();
        }
    }

    private void OnDestroy()
    {
        PROJECTILES.Remove(this);
    }

    static public void DESTROY_PROJECTILES()
    {
        foreach (Projectile p in PROJECTILES)
        {
            Destroy(p.gameObject);
        }
    }

}
