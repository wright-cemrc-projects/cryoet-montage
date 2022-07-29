using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is a controller for a GameObject that will display previews of the size of the beam and size of the camera box on the stage
/// when at 0-degree tilt. 
/// 
/// After the simulation starts this view can be hidden and only be reshown after a Reset of the view.
/// </summary>
public class CameraBeamPreviewController : MonoBehaviour
{
    // Visual Representation of Beam GameObject
    public LineRenderer BeamPreview;

    // Visual Representation of Camera FOV Rectangle
    public LineRenderer CameraBox;

    // Start is called before the first frame update
    void Start()
    {}

    // Update is called once per frame
    void Update()
    {}

    public void SetBeamDiameter(float microns)
    {
        // Take the illuminated area diameter
        // change the visual representation of a dotted circle.

        int points = 101;

        Vector3[] positions = new Vector3[points];

        for (int i = 0; i < points; i++)
        {
            float angle = 2.0f * Mathf.PI * ((float)i) / ((float)points - 1);
            float x = Mathf.Cos(angle) * microns / 2.0f;
            float y = Mathf.Sin(angle) * microns / 2.0f;

            positions[i] = new Vector3(x, 0, y);
        }
        
        if (BeamPreview)
        {
            BeamPreview.SetPositions(positions);
        }
    }

    public void SetCameraSize(float micronX, float micronY)
    {
        // Take the dimensions of the camera FOV in microns
        // change the visual representation of a camera.
        float ULx = (-1.0f) * micronX / 2.0f;
        float ULy = (-1.0f) * micronY / 2.0f;
        float LRx = (1.0f) * micronX / 2.0f;
        float LRy = (1.0f) * micronY / 2.0f;

        if (CameraBox)
        {
            Vector3[] positions = new Vector3[5];
            positions[0] = new Vector3(ULx, 0, ULy);
            positions[1] = new Vector3(LRx, 0, ULy);
            positions[2] = new Vector3(LRx, 0, LRy);
            positions[3] = new Vector3(ULx, 0, LRy);
            positions[4] = new Vector3(ULx, 0, ULy);
            CameraBox.SetPositions(positions);
        }
    }
}
