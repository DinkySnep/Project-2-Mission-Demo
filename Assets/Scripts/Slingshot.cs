using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slingshot : MonoBehaviour
{
    [Header("Inscribed")]
    public GameObject projectilePrefab;
    public float velocityMult = 10f;
    public GameObject projLinePrefab;

    [Header("Dynamic")]
    public GameObject launchPoint;
    public Vector3 launchPos;
    public GameObject projectile;
    public bool aimingMode;

    [Header("Rubber Band")]
    private LineRenderer bandLine;

    void Awake()
    {
        Transform launchPointTrans = transform.Find("LaunchPoint");
        launchPoint = launchPointTrans.gameObject;
        launchPoint.SetActive(false);
        launchPos = launchPointTrans.position;

        bandLine = GetComponent<LineRenderer>();
        bandLine.positionCount = 2;
        bandLine.enabled = false; // hidden until aiming
    }
    void OnMouseEnter()
    {
        // print("Slingshot:OnMouseEnter()");
        launchPoint.SetActive(true);
    }
    void OnMouseExit()
    {
        // print("Slingshot:OnMouseExit()");
        launchPoint.SetActive(false);
    }

    void OnMouseDown()
    {
        // the player has pressed the mouse button while over Slingshot
        aimingMode = true;
        // instance a projectile
        projectile = Instantiate(projectilePrefab) as GameObject;
        // start it at launch point
        projectile.transform.position = launchPos;
        // set it to isKinematic for now
        projectile.GetComponent<Rigidbody>().isKinematic = true;


        bandLine.enabled = true;
    }

    void Update()
    {
        // If slingshot is not in aming mode do not run this code
        if (!aimingMode) return;

        // get the current mouse position in 2d screen spaace
        Vector3 mousePos2D = Input.mousePosition;
        mousePos2D.z = -Camera.main.transform.position.z;
        Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(mousePos2D);

        // find the delta from the launchpos to the moustpos3d
        Vector3 mouseDelta = mousePos3D - launchPos;
        // Limit mouseDelta to the radius of the slingshot sphere collider
        float maxMagnitude = this.GetComponent<SphereCollider>().radius;
        if (mouseDelta.magnitude > maxMagnitude)
        {
            mouseDelta.Normalize();
            mouseDelta *= maxMagnitude;
        }
        // Move the projectile to this new positon
        Vector3 projPos = launchPos + mouseDelta;
        projectile.transform.position = projPos;

        bandLine.SetPosition(0, launchPos);
        bandLine.SetPosition(1, projPos);

        if (Input.GetMouseButtonUp(0))
        {
            // the mouse has been released
            aimingMode = false;

            bandLine.enabled = false;

            Rigidbody projRB = projectile.GetComponent<Rigidbody>();
            projRB.isKinematic = false;
            projRB.collisionDetectionMode = CollisionDetectionMode.Continuous;
            projRB.velocity = -mouseDelta * velocityMult;

            // switch to slingshot view immediatly before setting POI
            FollowCam.SWITCH_VIEW(FollowCam.eView.slingshot);

            FollowCam.POI = projectile;     // set the main camera poi

            // add a projectile line to the projectile
            Instantiate<GameObject>(projLinePrefab, projectile.transform);

            projectile = null;
            MissionDemolition.SHOT_FIRED();
        }

    }



}
