using System.Collections;
using System.Collections.Generic;
using System.IO;
//using UnityEditor;
//using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class ToolkitController : MonoBehaviour
{
    public SpiralStrategyBuilder builder;
    public Beam beam;
    public RunStrategy2 runner;
    public CameraController cameras;
    public VoxelHolder VoxelMap;
    public SimulationParameters Parameters;
    public GameObject Target;

    // Start is called before the first frame update
    void Start()
    {
        UIDocument document = GetComponent<UIDocument>();

        // --- Simulation Buttons ---
        // Button is called RunSimulation
        Button RunSimulation = document.rootVisualElement.Q<Button>("RunSimulation");
        RunSimulation.clicked += RunSimulationClicked;

        // Button is called StopSimulation
        Button PauseSimulation = document.rootVisualElement.Q<Button>("PauseSimulation");
        PauseSimulation.clicked += PauseSimulationClicked;

        // Button is called ResetSimulation
        Button ResetSimulation = document.rootVisualElement.Q<Button>("ResetSimulation");
        ResetSimulation.clicked += ResetSimulationClicked;

        // --- Camera Buttons ---
        Button TopCamera = document.rootVisualElement.Q<Button>("TopCamera");
        TopCamera.clicked += TopCameraClicked;

        Button OrthoCamera = document.rootVisualElement.Q<Button>("OrthoCamera");
        OrthoCamera.clicked += OrthoCameraClicked;

        Button WorldCamera = document.rootVisualElement.Q<Button>("WorldCamera");
        WorldCamera.clicked += WorldCameraClicked;

        Button ExportButton = document.rootVisualElement.Q<Button>("Export");
        ExportButton.clicked += ExportClicked;

        // Bindings between variables and the UI
        SetupBindings(document);

        // Initial displays.
        UpdateAllVisible();
    }

    void SetupBindings(UIDocument document) {
        // Bindings for the Image Shift Pattern Panel

        TextField PixelSpacing = document.rootVisualElement.Q<TextField>("PixelSpacing");
        PixelSpacing.SetValueWithoutNotify(string.Format("{0:N2}", Parameters.PixelSpacing));
        PixelSpacing.RegisterValueChangedCallback( e => OnPixelSpacingChanged(e));

        TextField IlluminatedArea= document.rootVisualElement.Q<TextField>("BeamDiameter");
        IlluminatedArea.SetValueWithoutNotify(string.Format("{0:N2}", Parameters.IlluminatedArea));
        IlluminatedArea.RegisterValueChangedCallback( e => OnIlluminatedAreaChanged(e));

        TextField CameraPixelsX = document.rootVisualElement.Q<TextField>("CameraPixelsX");
        CameraPixelsX.SetValueWithoutNotify(string.Format("{0}", Parameters.CameraPixelsX));
        CameraPixelsX.RegisterValueChangedCallback( e => OnCameraPixelsXChanged(e));

        TextField CameraPixelsY = document.rootVisualElement.Q<TextField>("CameraPixelsY");
        CameraPixelsY.SetValueWithoutNotify(string.Format("{0}", Parameters.CameraPixelsY));
        CameraPixelsY.RegisterValueChangedCallback( e => OnCameraPixelsYChanged(e));

        TextField ImageShiftsX = document.rootVisualElement.Q<TextField>("ImageShiftsX");
        ImageShiftsX.SetValueWithoutNotify(string.Format("{0}", Parameters.ImageShiftsX));
        ImageShiftsX.RegisterValueChangedCallback( e => OnImageShiftsXChanged(e));

        TextField ImageShiftsY = document.rootVisualElement.Q<TextField>("ImageShiftsY");
        ImageShiftsY.SetValueWithoutNotify(string.Format("{0}", Parameters.ImageShiftsY));
        ImageShiftsY.RegisterValueChangedCallback( e => OnImageShiftsYChanged(e));

        TextField PercentOverlapX = document.rootVisualElement.Q<TextField>("OverlapX");
        PercentOverlapX.SetValueWithoutNotify(string.Format("{0:N2}", Parameters.PercentOverlapX));
        PercentOverlapX.RegisterValueChangedCallback( e => OnPercentOverlapXChanged(e));

        TextField PercentOverlapY = document.rootVisualElement.Q<TextField>("OverlapY");
        PercentOverlapY.SetValueWithoutNotify(string.Format("{0:N2}", Parameters.PercentOverlapY));
        PercentOverlapY.RegisterValueChangedCallback( e => OnPercentOverlapYChanged(e));

        // Bindings for the Tilt Strategy Panel

        TextField TiltTo = document.rootVisualElement.Q<TextField>("TiltTo");
        TiltTo.SetValueWithoutNotify(string.Format("{0:N2}", Parameters.TiltTo));
        TiltTo.RegisterValueChangedCallback( e => OnTiltToChanged(e));

        TextField TiltEnd = document.rootVisualElement.Q<TextField>("TiltEnd");
        TiltEnd.SetValueWithoutNotify(string.Format("{0:N2}", Parameters.TiltEnd));
        TiltEnd.RegisterValueChangedCallback( e => OnTiltEndChanged(e));

        TextField TiltStart = document.rootVisualElement.Q<TextField>("TiltStart");
        TiltStart.SetValueWithoutNotify(string.Format("{0:N2}", Parameters.TiltStart));
        TiltStart.RegisterValueChangedCallback( e => OnTiltStartChanged(e));

        TextField TiltIncrement = document.rootVisualElement.Q<TextField>("TiltIncrement");
        TiltIncrement.SetValueWithoutNotify(string.Format("{0:N2}", Parameters.TiltIncrement));
        TiltIncrement.RegisterValueChangedCallback( e => OnTiltIncrementChanged(e));

        TextField DosePerTilt = document.rootVisualElement.Q<TextField>("DosePerTilt");
        DosePerTilt.SetValueWithoutNotify(string.Format("{0:N2}", Parameters.DosePerTilt));
        DosePerTilt.RegisterValueChangedCallback( e => OnDosePerTiltChanged(e));

        TextField AmpInitial = document.rootVisualElement.Q<TextField>("AmpInitial");
        AmpInitial.SetValueWithoutNotify(string.Format("{0:N2}", Parameters.AmpInitial));
        AmpInitial.RegisterValueChangedCallback( e => OnAmpInitialChanged(e));

        TextField AmpFinal = document.rootVisualElement.Q<TextField>("AmpFinal");
        AmpFinal.SetValueWithoutNotify(string.Format("{0:N2}", Parameters.AmpFinal));
        AmpFinal.RegisterValueChangedCallback( e => OnAmpFinalChanged(e));

        TextField Period = document.rootVisualElement.Q<TextField>("Period");
        Period.SetValueWithoutNotify(string.Format("{0:N2}", Parameters.Period));
        Period.RegisterValueChangedCallback( e => OnPeriodChanged(e));

        TextField Turns = document.rootVisualElement.Q<TextField>("Turns");
        Turns.SetValueWithoutNotify(string.Format("{0:N2}", Parameters.Turns));
        Turns.RegisterValueChangedCallback( e => OnTurnsChanged(e));

        Toggle ApplyCosAngle = document.rootVisualElement.Q<Toggle>("CorrectCos");
        ApplyCosAngle.SetValueWithoutNotify(Parameters.ProjectOnPerpendicularAxis);
        ApplyCosAngle.RegisterValueChangedCallback( e => OnApplyCosAngle(e));

        Toggle ShowTargetObject = document.rootVisualElement.Q<Toggle>("ShowObjects");
        ShowTargetObject.SetValueWithoutNotify(Parameters.ShowTarget);
        ShowTargetObject.RegisterValueChangedCallback( e => OnShowTargetObject(e));

        Slider AdjustIntensity = document.rootVisualElement.Q<Slider>("AdjustIntensity");
        AdjustIntensity.SetValueWithoutNotify(Parameters.MaxIntensity);
        AdjustIntensity.RegisterValueChangedCallback( e => OnMaxIntensitySliderChanged(e));
    }

    private void OnPixelSpacingChanged(ChangeEvent<string> evt) 
    {
        if (Parameters) {
            if (float.TryParse(evt.newValue, out Parameters.PixelSpacing)) {
                Parameters.DebugImageShifts();
                UpdateAllVisible();
            }
        }
    }

    private void UpdateBeam() {
        if (beam)
        {
            // Illuminated Area = diameter of the beam
            beam.SetBeamRadius(Parameters.IlluminatedArea / 2.0f);
            beam.Dose = Parameters.DosePerTilt;
        }
    }

    private void OnIlluminatedAreaChanged(ChangeEvent<string> evt) 
    {
        if (Parameters) {
            if (float.TryParse(evt.newValue, out Parameters.IlluminatedArea)) {
                UpdateAllVisible();
            }
        }
    }

    private void OnCameraPixelsXChanged(ChangeEvent<string> evt) 
    {
        if (Parameters) {
            if (int.TryParse(evt.newValue, out Parameters.CameraPixelsX)) {
                UpdateAllVisible();
            }
        }
    }

    private void OnCameraPixelsYChanged(ChangeEvent<string> evt) 
    {
        if (Parameters) {
            if (int.TryParse(evt.newValue, out Parameters.CameraPixelsY)) {
                UpdateAllVisible();
            }
        }
    }

    private void OnImageShiftsXChanged(ChangeEvent<string> evt) 
    {
        if (Parameters) {
            if (int.TryParse(evt.newValue, out Parameters.ImageShiftsX)) {
                UpdateAllVisible();
            }
        }
    }

    private void OnImageShiftsYChanged(ChangeEvent<string> evt) 
    {
        if (Parameters) {
            if (int.TryParse(evt.newValue, out Parameters.ImageShiftsY)) {
                UpdateAllVisible();
            }
        }
    }

    private void OnPercentOverlapXChanged(ChangeEvent<string> evt) 
    {
        if (Parameters) {
            if (float.TryParse(evt.newValue, out Parameters.PercentOverlapX)) {
                UpdateAllVisible();
            }
        }
    }

    private void OnPercentOverlapYChanged(ChangeEvent<string> evt) 
    {
        if (Parameters) {
            if (float.TryParse(evt.newValue, out Parameters.PercentOverlapY)) {
                UpdateAllVisible();
            }
        }
    }

    private void OnApplyCosAngle(ChangeEvent<bool> evt)
    {
        if (Parameters) {
            Parameters.ProjectOnPerpendicularAxis = evt.newValue;
            UpdateSpiral();
        }
    }

    private void OnTiltToChanged(ChangeEvent<string> evt)
    {
        if (Parameters) {
            if (float.TryParse(evt.newValue, out Parameters.TiltTo)) {
                UpdateSpiral();
            }
        }
    }

    private void OnTiltEndChanged(ChangeEvent<string> evt)
    {
        if (Parameters) {
            if (float.TryParse(evt.newValue, out Parameters.TiltEnd)) {
                UpdateSpiral();
            }
        }
    }

    private void OnTiltStartChanged(ChangeEvent<string> evt)
    {
        if (Parameters) {
            if (float.TryParse(evt.newValue, out Parameters.TiltStart)) {
                UpdateSpiral();
            }
        }
    }

    private void OnTiltIncrementChanged(ChangeEvent<string> evt)
    {
        if (Parameters) {
            if (float.TryParse(evt.newValue, out Parameters.TiltIncrement)) {
                UpdateSpiral();
            }
        }
    }

    private void OnDosePerTiltChanged(ChangeEvent<string> evt)
    {
        if (Parameters) {
            if (float.TryParse(evt.newValue, out Parameters.DosePerTilt)) {
                UpdateBeam();
            }
        }
    }

    private void OnAmpInitialChanged(ChangeEvent<string> evt)
    {
        if (Parameters) {
            if (float.TryParse(evt.newValue, out Parameters.AmpInitial)) {
                UpdateSpiral();
            }
        }
    }

    private void OnAmpFinalChanged(ChangeEvent<string> evt)
    {
        if (Parameters) {
            if (float.TryParse(evt.newValue, out Parameters.AmpFinal)) {
                UpdateSpiral();
            }
        }
    }

    private void OnPeriodChanged(ChangeEvent<string> evt)
    {
        if (Parameters) {
            if (float.TryParse(evt.newValue, out Parameters.Period)) {
                UpdateSpiral();
            }
        }
    }

    private void OnTurnsChanged(ChangeEvent<string> evt)
    {
        if (Parameters) {
            if (float.TryParse(evt.newValue, out Parameters.Turns)) {
                UpdateSpiral();
            }
        }
    }

    void RunSimulationClicked()
    {
        // Reset on start
        ResetSimulationClicked();
        if (runner) {
            builder.BuildStrategy(Parameters);
            runner.StartSimulation(builder);
        }
    }

    void PauseSimulationClicked() 
    {
        if (runner) {
            runner.PauseSimulation();
        }
    }

    void ResetSimulationClicked() {
        if (runner) {
            // Should reset the tilt runner run
            runner.ResetSimulation();

            // Reset values
            if (VoxelMap) {
                VoxelMap.ResetVoxels();
            }
        }
    }

    void TopCameraClicked() {
        if (cameras) {
            cameras.ActivateStageCamera();
        }
    }

    void OrthoCameraClicked() {
        if (cameras) {
            cameras.ActivateOrthoCamera();
        }
    }

    void WorldCameraClicked() {
        if (cameras) {
            cameras.ActivateWorldCamera();
        }
    }

    /// Update a preview
    private void UpdateSpiral() {
        builder.UpdateSpiral(Parameters);
        // Debug.Log("Updated Spiral");
    }

    ///
    /// Export the intensities on voxels to a file.
    ///
    public void ExportClicked() {
        ExportCSV export = GetComponent<ExportCSV>();
        if (export != null) {
            string filename = "simulation.csv";
            export.WriteIntensities(filename);

            DetailsPanelController details = GetComponent<DetailsPanelController>();
            if (details)
            {
                details.SetMessage("Exported to : " + Path.GetFullPath(filename));
                details.UpdateDetails();
            }
        }
    }

    private void UpdateTargetView() {
        if (Target) {
            Target.SetActive(Parameters.ShowTarget);
        }
    }

    private void UpdateVoxelMaxIntensity() {
        if (VoxelMap) {
            VoxelMap.SetMaxValue(Parameters.MaxIntensity);
        }
    }

    private void OnShowTargetObject(ChangeEvent<bool> evt)
    {
        if (Parameters) {
            Parameters.ShowTarget = evt.newValue;
            UpdateTargetView();
        }
    }

    private void OnMaxIntensitySliderChanged(ChangeEvent<float> evt)
    {
        if (Parameters) {
            Parameters.MaxIntensity = evt.newValue;
            UpdateVoxelMaxIntensity();
        }
    }

    public void UpdateAllVisible() {
        // Should provide an initial spiral pattern
        UpdateSpiral();

        // Should update the initial beam diameter
        UpdateBeam();

        // For debugging
        // Parameters.DebugImageShifts();
    }
}
