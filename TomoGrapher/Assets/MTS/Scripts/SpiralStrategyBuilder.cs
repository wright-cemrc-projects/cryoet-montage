using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiralStrategyBuilder : MonoBehaviour
{
    // Assign to parent Transform with children describing shifted positions.
    // TODO: deprecate this and provide shifts via SimulationParameters
    // public Transform imageShiftPattern;

    // Serialize to view this private member in the Unity Editor
    // [SerializeField]
    // private List<Exposure> Exposures = new List<Exposure>();

    private List<TiltQueueItem> Tilts = new List<TiltQueueItem>();

    public LineRenderer SpiralPositive;
    public LineRenderer SpiralNegative;

    /// Describe a pattern of image shifts at a tilt.
    /// CenterX, CenterY describe a central offset of the pattern.
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

    /// We will call this method when we start a new simulation run (or change parameters)
    public void BuildStrategy(SimulationParameters parameters) {
        // 1. Clear the Tilts if needed.
        Tilts.Clear();

        // Protect against 0 tilt increment division errors.
        if (parameters.TiltIncrement == 0) return;

        // 2. BuildSpiral in each direction of the tilt series
        int positiveIncrements = (int) Mathf.Round( Mathf.Abs( (parameters.TiltTo - parameters.TiltStart) / parameters.TiltIncrement) );
        BuildSpiral(0, positiveIncrements, 1.0, parameters);

        int negativeIncrements = (int) Mathf.Round( Mathf.Abs( (parameters.TiltEnd - parameters.TiltStart) / parameters.TiltIncrement) );
        BuildSpiral(1, negativeIncrements, -1.0, parameters);
    }

    /// Update a visual representation of the spiral.
    public void UpdateSpiral(SimulationParameters parameters) {
        // Debug.Log("UpdateSpiral called.");

        // Protect against 0 tilt increment division errors.
        if (parameters.TiltIncrement == 0) {
            SpiralPositive.positionCount = 0;
            SpiralNegative.positionCount = 0;
            return;
        }

        int positiveIncrements = (int) Mathf.Round( Mathf.Abs( (parameters.TiltTo - parameters.TiltStart) / parameters.TiltIncrement) );
        List<TiltQueueItem> positiveList = BuildSpiralTranslations(0, positiveIncrements, 1.0f, parameters);        
        Vector3[] ppositions = new Vector3[positiveList.Count];
        for (int i = 0; i < positiveList.Count; i++) {
            ppositions[i] = new Vector3(positiveList[i].CenterX, 0f, positiveList[i].CenterY);
        }
        // Debug.Log("Updated a total of " + positiveList.Count);
        SpiralPositive.positionCount = positiveList.Count;
        SpiralPositive.SetPositions(ppositions);

        int negativeIncrements = (int) Mathf.Round( Mathf.Abs( (parameters.TiltEnd - parameters.TiltStart) / parameters.TiltIncrement) );
        List<TiltQueueItem> negativeList = BuildSpiralTranslations(1, negativeIncrements, -1.0f, parameters);
        Vector3[] npositions = new Vector3[negativeList.Count];
        for (int i = 0; i < negativeList.Count; i++) {
            npositions[i] = new Vector3(negativeList[i].CenterX, 0f, negativeList[i].CenterY);
        }
        SpiralNegative.positionCount = negativeList.Count;
        SpiralNegative.SetPositions(npositions);

    }

    /// Get a list of TiltQueueItem(s)
    private List<TiltQueueItem> BuildSpiralTranslations(int start, int end, float Sign, SimulationParameters parameters)
    {
        float B_growth = parameters.GetBGrowth();

        // Debug.Log("B_growth : " + B_growth);
        // Debug.Log("Number of tilts : " + end);

        List<TiltQueueItem> translations = new List<TiltQueueItem>();

        for (int i = start; i <= end; i++)
        {

            // Describe the rotation of the spiral
            float Rotation_deg = 0f;
            if (end != 0) {
                Rotation_deg = i * 360 * parameters.Period / (float) end;
            }
            float Rotation_rad = Rotation_deg * Mathf.PI / 180.0f;
            float S = Mathf.Sin((float)Rotation_rad);
            float C = Mathf.Cos((float)Rotation_rad);
            float IS_rad= (Rotation_deg * B_growth + parameters.AmpInitial)* Sign;

            float center_x = C * IS_rad;
            float center_y = S * IS_rad;
            Vector3 values = new Vector3((float)Rotation_deg, (float)center_x, (float)center_y);
            // Debug.Log(i + " point: " + values);

            float angle = parameters.TiltStart + (float) (i * parameters.TiltIncrement * Sign);
            string text = i.ToString();

            // build a translation center point for a tilt angle and a label.
            TiltQueueItem item = new TiltQueueItem();
            // This is inverse from the tilt series, because it happens at 0 tilt we are simulating the distortion of the translations
            // that *would* occur at higher tilt angles by dividing by cos(angle).  See also QueueTilts
            if (parameters.ProjectOnPerpendicularAxis) {
                item.CenterX = center_x;
            } else {
                item.CenterX = center_x / Mathf.Cos(Mathf.PI * angle / 180.0f);
            }
            item.CenterY = center_y;
            item.TiltAngle = angle;
            item.Label = text;
            
            translations.Add(item);
        }

        return translations;
    }

    public void BuildSpiral(int start, int end, double Sign, SimulationParameters parameters)
    {
        float B_growth = parameters.GetBGrowth();

        Debug.Log("B_growth : " + B_growth);
        Debug.Log("Number of tilts : " + end);

        for (int i = start; i <= end; i++)
        {

            // double Rotation_deg = ((double)i / TiltPerRevolution * 360 * 3;
            float Rotation_deg = 0f;
            if (end != 0) {
                Rotation_deg = i * 360 * parameters.Period / (float) end;
            }
            double Rotation_rad = Rotation_deg * Mathf.PI / 180.0;
            double S = Mathf.Sin((float)Rotation_rad);
            double C = Mathf.Cos((float)Rotation_rad);
            double IS_rad= (Rotation_deg * B_growth + parameters.AmpInitial)* Sign;

            double center_x = C * IS_rad;
            double center_y = S * IS_rad;
            Vector3 values = new Vector3((float)Rotation_deg, (float)center_x, (float)center_y);
            Debug.Log(i + " point: " + values);

            float angle = parameters.TiltStart + (float) (i * parameters.TiltIncrement * Sign);
            string text = i.ToString();

            QueueTilts((float) center_x, (float) center_y, angle, text, parameters.ProjectOnPerpendicularAxis, parameters);
        }
    }

    private List<Vector3> GetPattern(SimulationParameters parameters) {
        // SimulationParameters to determine pattern spacing.
        // parameters has the number of shifts, and the overall 

        float spacing_x = parameters.GetSpacingMicronsX();
        float spacing_y = parameters.GetSpacingMicronsY();

        // We want to center the pattern, so we need to find the max X and max Y, and subtract by 1/2 max.
        float max_x = spacing_x * parameters.ImageShiftsX;
        float max_y = spacing_y * parameters.ImageShiftsY;

        Vector3 offset = new Vector3( (spacing_x - max_x) / 2.0f, (spacing_y - max_y) / 2.0f, 0.0f);

        List<Vector3> rv = new List<Vector3>();

        for (int i = 0; i < parameters.ImageShiftsX; i++) {
            for (int j = 0; j < parameters.ImageShiftsY; j++) {
                Vector3 pos = new Vector3(i * spacing_x + offset.x, j * spacing_y + offset.y, 0.0f);
                rv.Add(pos);
            }
        }

        return rv;
    }

    public void QueueTilts(float center_x, float center_y, float tiltAngle, string text, bool ProjectOnPerpendicularAxis, SimulationParameters parameters)
    {
        // Build an instruction for a TextMeshPro
        TiltQueueItem item = new TiltQueueItem();
        if (ProjectOnPerpendicularAxis) {
            item.CenterX = center_x * Mathf.Cos(Mathf.PI * tiltAngle / 180.0f);
        } else {
            item.CenterX = center_x;
        }
        item.CenterY = center_y;
        item.TiltAngle = tiltAngle;
        item.Label = text;

        float dose = 1.0f;
        if (parameters.DoseWeighting == false) {
            dose = parameters.DosePerTilt;
        } else {
            if ((parameters.TiltIncrement > 0) && (parameters.TiltTo > parameters.TiltEnd)) {
                int TotalTilts = parameters.TotalTilts();

                // Calculate the total expected dose
                float TotalDose = TotalTilts * parameters.DosePerTilt;

                // Calculate the sum of (tilt angle * dose weighting (1/cos(tilt angle)))
                float tiltDose = 0f;
                for (float tilt = parameters.TiltEnd; tilt < parameters.TiltTo; tilt += parameters.TiltIncrement) {
                    tiltDose += Mathf.Pow(1.0f / Mathf.Cos(Mathf.PI * tilt / 180.0f), 1.0f / parameters.DoseWeightingPower) * parameters.DosePerTilt;
                }

                // Calculate the constant needed to have Dose = C * (1/cos(tilt angle))), so that still adds to the total expected dose
                // Then can calculate the individual adjusted dose at this tilt.
                float doseConstant = TotalDose / tiltDose;

                dose = doseConstant * Mathf.Pow(1.0f / Mathf.Cos(Mathf.PI * tiltAngle / 180.0f), 1.0f / parameters.DoseWeightingPower) * parameters.DosePerTilt;

            } else {
                dose = parameters.DosePerTilt;
            }
        }

        // Apply the image shifting pattern
        List<Vector3> pattern = GetPattern(parameters);
        foreach (Vector3 point in pattern)
        {
            // Each of the pattern items is used for the x, y offsets.
            // NOTE: I use (x, z) since the Y is up axis.
            Exposure e = new Exposure();
            if (ProjectOnPerpendicularAxis) {
                e.x = (center_x + point.x) * Mathf.Cos(Mathf.PI * tiltAngle / 180.0f);
            } else {
                e.x= center_x + point.x;
            }
            e.y = center_y + point.y;
            e.tiltDegrees = tiltAngle;
            e.dose = dose;

            // Add each of the imageshifts of the beam.
            item.ImageShifts.Add(e);
        }

        Tilts.Add(item);
    }

    /// Returns a list of exposures (image shifts at tilts)
    public List<Exposure> GetExposures() {
        List<Exposure> rv = new List<Exposure>();

        foreach (TiltQueueItem item in Tilts) {
            foreach (Exposure exp in item.ImageShifts) {
                rv.Add(exp);
            }
        }

        return rv;
    }
}