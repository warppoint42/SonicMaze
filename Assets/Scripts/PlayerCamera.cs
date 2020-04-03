using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public GameObject limelight;

    public float speedH = 2.0f;
    public float speedV = 2.0f;

    private float yaw = 0.0f;
    private float pitch = 0.0f;

    bool disable = true;

    void Start()
    {
        limelight = GameObject.Find("TopLight");
    }

    // Update is called once per frame
    void Update()
    {

        yaw += speedH * Input.GetAxis("Mouse X");
        pitch -= speedV * Input.GetAxis("Mouse Y");

        yaw = Mathf.Clamp(yaw, -70, 70);
        pitch = Mathf.Clamp(pitch, -45, 45);

        transform.localRotation = Quaternion.Euler(pitch, yaw, 0.0f);
        if (Input.GetKeyDown(KeyCode.O))
        {
            disable = !disable;
        }


    }

    void OnPreCull()
    {
        if (!disable) return;
        if (limelight != null)
            limelight.GetComponent<Light>().enabled = false;
    }

    void OnPreRender()
    {
        if (!disable) return;
        if (limelight != null)
            limelight.GetComponent<Light>().enabled = false;
    }
    void OnPostRender()
    {
        if (!disable) return;
        if (limelight != null)
            limelight.GetComponent<Light>().enabled = true;
    }
}
