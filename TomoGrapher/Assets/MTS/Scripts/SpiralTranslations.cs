using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiralTranslations : MonoBehaviour
{
    public RunStrategy Strategy;
    public GameObject imageShiftPattern;
    public int TiltPerRevolution = 16;
    public double RadiusFactor = 0.05;

    public double AngleIncrement = 3;
    public int TotalTilts = 21;

    // Start is called before the first frame update
    void Start()
    {
        // 0 tilt to positive 60
        BuildSpiral(0, TotalTilts, AngleIncrement, 1);
        // 1 tilt to negative 60
        BuildSpiral(1, TotalTilts, -1.0 * AngleIncrement, -1);
    }

    public void BuildSpiral(int start, int end, double AngleIncrement, double Sign) {
        for (int i = start; i <= end; i++) {

            double Rotation_deg = (double) i / TiltPerRevolution * 360 * 3;
            double Rotation_rad = Rotation_deg * Mathf.PI / 180.0;
            double S = Mathf.Sin((float) Rotation_rad);
            double C = Mathf.Cos((float) Rotation_rad);
            double Radius_Q = Rotation_deg * RadiusFactor * Sign;

            double center_x = C * Radius_Q;
            double center_y = S * Radius_Q;
            Vector3 values = new Vector3((float) Rotation_deg, (float) center_x, (float) center_y);
            Debug.Log(i + " point: " + values);

            // Apply the image shifting pattern
            foreach (Transform child in imageShiftPattern.transform) {
                // Each of the pattern items is used for the x, y offsets.
                // NOTE: I use (x, z) since the Y is up axis.
                Exposure e = new Exposure();
                e.x = center_x + child.position.x;
                e.y = center_y + child.position.z; 
                e.tiltDegrees = i * AngleIncrement;

                Strategy.ShiftTiltStrategy.Add(e);
            }
        }
    }
}
