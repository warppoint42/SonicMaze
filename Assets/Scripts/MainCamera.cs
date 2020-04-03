using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{

    public GameObject limelight;
    // Start is called before the first frame update
    public void findSpot()
    {
        limelight = GameObject.Find("Directional Light");
    }

    public void setL(float val)
    {
        limelight.GetComponent<Light>().intensity = val;
    }

    //void OnPreCull()
    //{
    //    if (limelight != null)
    //        limelight.GetComponent<Light>().enabled = false;
    //}

    //void OnPreRender()
    //{
    //    if (limelight != null)
    //        limelight.GetComponent<Light>().enabled = false;
    //}
    //void OnPostRender()
    //{
    //    if (limelight != null)
    //        limelight.GetComponent<Light>().enabled = true;
    //}

}
