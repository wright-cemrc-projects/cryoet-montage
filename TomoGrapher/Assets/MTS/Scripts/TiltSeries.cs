using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TiltSeries : MonoBehaviour
{
    public RunStrategy Strategy;
    public GameObject imageShiftPattern;

    public double TiltRange = 60;
    public double TiltIncrement = 3;
    public double StartTilt = -60;

    //No longer needed if we are building an explicit pattern to run as a strategy.
    //public Beam beam;
    //public GameObject stage;
    //private double TimeInterval = 0.25;
    //private double Timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        BuildTiltSeries();
    }

    public void BuildTiltSeries() {
        int TotalTilts = (int) (2.0*TiltRange / TiltIncrement);
        
        // Overall these tilts..
        for (int i = 0; i < TotalTilts; i++) {
            // Apply the image shifting pattern
            foreach (Transform child in imageShiftPattern.transform) {
                // Each of the pattern items is used for the x, y offsets.
                // NOTE: I use (x, z) since the Y is up axis.
                Exposure e = new Exposure();
                e.x = child.position.x;
                e.y = child.position.z; 
                e.tiltDegrees = StartTilt + i * TiltIncrement;

                Strategy.ShiftTiltStrategy.Add(e);
            }
        }
    }

    /*
    // Update is called once per frame
    void Update()
    {
        Timer -= Time.deltaTime;

        if (Timer < 0 && CurrentTilt < TiltRange)
        {
            Quaternion rot = new Quaternion();
            rot.eulerAngles = new Vector3(0, 0, (float)CurrentTilt);
            stage.transform.rotation = rot;

            Timer = TimeInterval;

            // Move to the next tilt and fire.
            beam.RayIntersectVoxels();

            // Next tilt
            CurrentTilt += TiltIncrement;
        }
    }
    */
}
