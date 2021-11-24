using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpiralTranslations2 : MonoBehaviour
{
    public RunStrategy Strategy;
    public GameObject imageShiftPattern;
    public int TiltPerRevolution = 16;
    public double RadiusFactor = 0.05;

    public double AngleIncrement = 3;
    public int TotalTilts = 21;

    public double TimeInterval = 0.25;
    private double Timer;
    private int CurrentTilt = 0;

    // Text Labelling Prefab
    public GameObject Label;
    public GameObject Stage;

    // Draw spiral as lines
    public LineRenderer Line;

    // Options
    public bool DrawLines = false;
    public bool DrawText = false;

    // Operations that will be run.
    private List<TiltQueueItem> TiltQueue = new List<TiltQueueItem>();

    // Complete description of operation at each tilt.
    private class TiltQueueItem
    {
        // Overall description.
        public float TiltAngle;
        public float CenterX;
        public float CenterY;
        public string Label;
        // Image shifts at this tilt
        public List<Exposure> ImageShifts = new List<Exposure>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // 0 tilt to positive 60
        BuildSpiral(0, TotalTilts, AngleIncrement, 1);
        // 1 tilt to negative 60
        BuildSpiral(1, TotalTilts, -1.0 * AngleIncrement, -1);
    }

    // Update is called once per frame
    void Update()
    {
        Timer -= Time.deltaTime;

        if (Timer < 0 && CurrentTilt < TiltQueue.Count)
        {
            Timer = TimeInterval;

            // Draw the text describing the current point.
            AddLabel(TiltQueue[CurrentTilt]);

            // Add the current points to the RunStrategy to draw.
            foreach (Exposure e in TiltQueue[CurrentTilt].ImageShifts)
            {
                Strategy.ShiftTiltStrategy.Add(e);
            }

            CurrentTilt++;
        }
    }

    private void AddLabel(TiltQueueItem item)
    {
        // Need to ray trace to find the intersection of the CenterX, CenterY down to the stage.
        // Then get that point, to be able to add the TMP text as a number. 

        Vector3 start = new Vector3(item.CenterX, 20, item.CenterY); // center point
        Vector3 direction = new Vector3(0, -1, 0); // down

        RaycastHit[] hits;
        hits = Physics.RaycastAll(start, direction, 100.0F);

        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];
            GameObject voxel = hit.collider.gameObject;
            if (voxel.GetComponent<VoxelBehaviour>() != null)
            {
                // Add the label
                Vector3 location = new Vector3(hit.point.x, hit.point.y + 0.5f, hit.point.z);

                if (DrawText)
                {
                    GameObject label = Instantiate(Label);
                    label.transform.position = location;
                    // Vector3 euler = Stage.transform.rotation.eulerAngles;
                    // label.transform.Rotate(euler);
                    label.transform.parent = Stage.transform;
                    label.GetComponent<TMP_Text>().text = item.Label;

                    Debug.Log("Would add label: " + item.Label + " for " + item.CenterX + ", " + item.CenterY);
                }

                if (DrawLines)
                {
                    Line.positionCount++;
                    Vector3 localPos = Stage.transform.InverseTransformPoint(location);
                    Line.SetPosition(Line.positionCount - 1, localPos);
                }

                break;
            }
        }
    }

    public void BuildSpiral(int start, int end, double AngleIncrement, double Sign)
    {
        for (int i = start; i <= end; i++)
        {

            double Rotation_deg = (double)i / TiltPerRevolution * 360 * 3;
            double Rotation_rad = Rotation_deg * Mathf.PI / 180.0;
            double S = Mathf.Sin((float)Rotation_rad);
            double C = Mathf.Cos((float)Rotation_rad);
            double Radius_Q = Rotation_deg * RadiusFactor * Sign;

            double center_x = C * Radius_Q;
            double center_y = S * Radius_Q;
            Vector3 values = new Vector3((float)Rotation_deg, (float)center_x, (float)center_y);
            Debug.Log(i + " point: " + values);

            float angle = (float) (i * AngleIncrement);
            string text = i.ToString();

            QueueTilts((float) center_x, (float) center_y, angle, text);
        }
    }

    public void QueueTilts(float center_x, float center_y, float tiltAngle, string text)
    {
        // Build an instruction for a TextMeshPro
        TiltQueueItem item = new TiltQueueItem();
        item.CenterX = center_x;
        item.CenterY = center_y;
        item.TiltAngle = tiltAngle;
        item.Label = text;

        // Apply the image shifting pattern
        foreach (Transform child in imageShiftPattern.transform)
        {
            // Each of the pattern items is used for the x, y offsets.
            // NOTE: I use (x, z) since the Y is up axis.
            Exposure e = new Exposure();
            e.x = center_x + child.position.x;
            e.y = center_y + child.position.z;
            e.tiltDegrees = tiltAngle;

            // Add each of the imageshifts of the beam.
            item.ImageShifts.Add(e);
        }

        TiltQueue.Add(item);
    }
}
