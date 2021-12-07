using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///
/// This class is intended to be an instance the simulation parameters.
///
public class SimulationParameters : MonoBehaviour
{

    // Pixel Spacing
    [SerializeField]
    public float PixelSpacing = 1.0f;

    // Illuminated Area
    [SerializeField]
    public float IlluminatedArea = 3.2f;

    // Camera Dimensions X, Y
    [SerializeField]
    public int CameraPixelsX = 5760;
    [SerializeField]
    public int CameraPixelsY = 4092;

    // Image Shifts
    [SerializeField]
    public int ImageShiftsX = 3;
    [SerializeField]
    public int ImageShiftsY = 3;

    // Overlaps of the camera fields
    [SerializeField]
    public float PercentOverlapX = 15.0f;
    [SerializeField]
    public float PercentOverlapY = 10.0f; 

    // Electron per tilt
    [SerializeField]
    public float DosePerTilt = 1.0f;

    // Referenced by cycles term.
    [SerializeField]
    public float Turns = 50f;

    // Referenced by the IS_Angle term.
    [SerializeField]
    public float Period = 3f;

    // How far do the tilts extend.
    [SerializeField]
    public float TiltTo = 60f;

    [SerializeField]
    public float TiltEnd = -60f;

    [SerializeField]
    public float TiltStart = 0f;

    // Incremental change of a tilt series, or step size.
    [SerializeField]
    public float TiltIncrement = 3f;

    [SerializeField]
    public float TiltsPerRevolution = 16f;

    // Affects the start and end of the spiral diameter.
    [SerializeField]
    public float AmpFinal = 0.8f;
    [SerializeField]
    public float AmpInitial = 0f;

    // Apply a correction based on the tilt angle so that a projected spiral pattern doesn't
    // distort in the axis perpendicular to the tilt axis.
    [SerializeField]
    public bool ProjectOnPerpendicularAxis = true;

    // Radius Growth R (B_growth) determines the distance of a point from origin, rate of expansion of the spiral.
    private float Bgrowth;  // Should be calculated as (A_final - A_initial) / Cycles

    [SerializeField]
    public bool ShowTarget = true;

    [SerializeField]
    public float MaxIntensity = 256f;

    public void Awake() {
        UpdateValues();
    }

    private void UpdateValues() {
        UpdateGrowth();

        // Debug.Log("Period : " + Period);
        // Debug.Log("Cycles : " + Cycles);
        // Debug.Log("TiltTo : " + TiltTo);
        // Debug.Log("TiltEnd : " + TiltEnd);
        // Debug.Log("TiltStart : " + TiltStart);
        // Debug.Log("TiltIncrement : " + TiltIncrement);
        // Debug.Log("AmpInitial : " + AmpInitial);
        // Debug.Log("AmpFinal : " + AmpFinal);
        // Debug.Log("Period : " + Period);
    }

    public float GetBGrowth() {
        return UpdateGrowth();
    }

    private float UpdateGrowth() {
        if (Turns != 0f) {
            Bgrowth = (AmpInitial - AmpFinal) / (47 * Turns);
        } else {
            Bgrowth = 0f;
        }
        return Bgrowth;
    }
 
    /// Get the X dimension of the camera in microns, derived from pixel spacing.
    public float GetCameraMicronsX() {
        // Pixels * Angstrom / pixel * 10 nm / 1 A * 1 um / 1000 nm.
        // Convert the answer back to microns (Pixels )
        return (CameraPixelsX * PixelSpacing) / (10.0f * 1000.0f);
    }

    /// Get the Y dimension of the camera in microns, derived from pixel spacing.
    public float GetCameraMicronsY() {
        // Pixels * Angstrom / pixel * 10 nm / 1 A * 1 um / 1000 nm.
        // Convert the answer back to microns
        return (CameraPixelsY * PixelSpacing) / (10.0f * 1000.0f);
    }

    /// Get the overlap in microns on X edge
    public float GetOverlapMicronsX() {
        return GetCameraMicronsX() * (PercentOverlapX / 100.0f);
    }

    /// Get the overlap in microns on Y edge
    public float GetOverlapMicronsY() {
        return GetCameraMicronsY() * (PercentOverlapY / 100.0f);
    }

    /// Get the X spacing in microns of the imaging/beam center
    public float GetSpacingMicronsX() {
        return GetCameraMicronsX() - GetOverlapMicronsX();
    }

    /// Get the Y spacing in microns of the imaging/beam center
    public float GetSpacingMicronsY() {
        return GetCameraMicronsY() - GetOverlapMicronsY();
    }

    public void DebugImageShifts() {
        Debug.Log("Pattern is " + ImageShiftsX + " by " + ImageShiftsY);
        Debug.Log("Camera is " + GetCameraMicronsX() + " by " + GetCameraMicronsY() + " microns.");
        Debug.Log("Spacing is " + GetSpacingMicronsX() + " x " + GetSpacingMicronsY() + " microns.");
        Debug.Log("Beam diameter is " + IlluminatedArea + " microns");
    }

}
