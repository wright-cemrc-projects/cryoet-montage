using System.Collections;
using System.Collections.Generic;
using System.IO;
//using UnityEditor;
//using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

using SimpleFileBrowser;

public class ToolkitController : MonoBehaviour
{
    public SpiralStrategyBuilder builder;
    public Beam beam;
    public RunStrategy2 runner;
    public CameraController cameras;
    public VoxelHolder VoxelMap;
    public SimulationParameters Parameters;
    public GameObject Target;
    public CameraBeamPreviewController Preview;

    private RadioButton SerialEM4;
    private RadioButton SerialEM3;

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

        Button ExportMacroButton = document.rootVisualElement.Q<Button>("ExportMacro");
        ExportMacroButton.clicked += ExportMacroClicked;

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

        TextField Revolutions = document.rootVisualElement.Q<TextField>("Revolutions");
        Revolutions.SetValueWithoutNotify(string.Format("{0:N2}", Parameters.Revolutions));
        Revolutions.RegisterValueChangedCallback( e => OnRevolutionsChanged(e));

        Toggle DoseWeighting = document.rootVisualElement.Q<Toggle>("DoseWeight");
        DoseWeighting.SetValueWithoutNotify(Parameters.DoseWeighting);
        DoseWeighting.RegisterValueChangedCallback( e => OnDoseWeighting(e));

        Toggle ApplyCosAngle = document.rootVisualElement.Q<Toggle>("CorrectCos");
        ApplyCosAngle.SetValueWithoutNotify(Parameters.ProjectOnPerpendicularAxis);
        ApplyCosAngle.RegisterValueChangedCallback( e => OnApplyCosAngle(e));

        Toggle ShowTargetObject = document.rootVisualElement.Q<Toggle>("ShowObjects");
        ShowTargetObject.SetValueWithoutNotify(Parameters.ShowTarget);
        ShowTargetObject.RegisterValueChangedCallback( e => OnShowTargetObject(e));

        Slider AdjustIntensity = document.rootVisualElement.Q<Slider>("AdjustIntensity");
        AdjustIntensity.SetValueWithoutNotify(Parameters.MaxIntensity);
        AdjustIntensity.RegisterValueChangedCallback( e => OnMaxIntensitySliderChanged(e));

        TextField Power = document.rootVisualElement.Q<TextField>("Power");
        Power.SetValueWithoutNotify(string.Format("{0:N2}", Parameters.DoseWeightingPower));
        Power.RegisterValueChangedCallback(e => OnPowerChanged(e));

        // Add handlers for the DoseSymmetric + Bidirectional buttons.
        RadioButton DoseSymmetric = document.rootVisualElement.Q<RadioButton>("DoseSymmetric");
        DoseSymmetric.RegisterValueChangedCallback( e => OnDoseSymmetricChanged(e) );

        // Add handlers for the SerialEM version selection buttons.
        SerialEM3 = document.rootVisualElement.Q<RadioButton>("SerialEM3");
        SerialEM4 = document.rootVisualElement.Q<RadioButton>("SerialEM4");
        SerialEM4.RegisterValueChangedCallback( e => OnSerialVersionChanged(e) );

        TextField LogDirectory = document.rootVisualElement.Q<TextField>("Logdir");
        LogDirectory.RegisterValueChangedCallback( e => OnLogDirectoryChanged(e) );

    }

    private void UpdatePreview()
    {
        if (Preview)
        {
            // Update the Box for the camera.
            Preview.SetCameraSize(Parameters.GetCameraMicronsX(), Parameters.GetCameraMicronsY());
            Preview.SetBeamDiameter(Parameters.IlluminatedArea);
        }
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

    private void OnDoseWeighting(ChangeEvent<bool> evt)
    {
        if (Parameters) {
            Parameters.DoseWeighting = evt.newValue;
            UpdateSpiral();
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

    private void OnRevolutionsChanged(ChangeEvent<string> evt)
    {
        if (Parameters) {
            if (float.TryParse(evt.newValue, out Parameters.Revolutions)) {
                UpdateSpiral();
            }
        }
    }

    private void OnPowerChanged(ChangeEvent<string> evt)
    {
        if (Parameters)
        {
            if (float.TryParse(evt.newValue, out Parameters.DoseWeightingPower))
            {
                UpdateSpiral();
            }
        }
    }

    private void OnDoseSymmetricChanged(ChangeEvent<bool> evt)
    {
        
        if (Parameters) 
        {
            if (evt.newValue) {
                Parameters.DoseSymmetric = 0;
            } else {
                Parameters.DoseSymmetric = 1;
            }
            Debug.Log("OnDoseSymmetric has value " + Parameters.DoseSymmetric.ToString());
        }
    }

    private void OnSerialVersionChanged(ChangeEvent<bool> evt)
    {
        if (Parameters)
        {
            if (SerialEM3.value) {
                Parameters.SerialEM_ver = SerialEM_Version.SerialEM3_8;
            } else {
                Parameters.SerialEM_ver = SerialEM_Version.SerialEM4_1;
            }

            Debug.Log("OnSerialVersion has value " + Parameters.SerialEM_ver.ToString());
        }
    }

    private void OnLogDirectoryChanged(ChangeEvent<string> evt)
    {
        if (Parameters)
        {
            Parameters.LogDirectory = evt.newValue;
            Debug.Log("OnLogDirectory has value " + Parameters.LogDirectory.ToString());
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
            StartCoroutine(ShowSaveCSVDialogCoroutine(export));
        }
    }

    public void ExportMacroClicked() {
        // Get the export, and provide Parameters to fill out a SerialEMMacro.
        ExportMacro export = GetComponent<ExportMacro>();
        if (export != null) {
            StartCoroutine(ShowSaveMacroDialogCoroutine(export));
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

        // Update a preview view
        UpdatePreview();
    }

    IEnumerator ShowSaveMacroDialogCoroutine(ExportMacro export)
    {
        yield return FileBrowser.WaitForSaveDialog(FileBrowser.PickMode.Files, true, null, "cryoMontage.txt", "Save file", "Save");

        Debug.Log(FileBrowser.Success);

        if (FileBrowser.Success)
        {
            // Print paths of the selected files (FileBrowser.Result) (null, if FileBrowser.Success is false)
            if (FileBrowser.Result.Length > 0)
            {
                for (int i = 0; i < FileBrowser.Result.Length; i++)
                    Debug.Log(FileBrowser.Result[i]);

                // Save the macro to this path.
                string filename = FileBrowser.Result[0];

                if (File.Exists(export.GetTemplatePath(Parameters.SerialEM_ver))) {
                   
                    if (Parameters)
                    {
                        export.WriteMacro(filename, Parameters);
                    }
                    else
                    {
                        Debug.Log("Parameters is not set.");
                    }

                    DetailsPanelController details = GetComponent<DetailsPanelController>();
                    if (details)
                    {
                        details.SetMessage("Exported Macro to : " + Path.GetFullPath(filename));
                        details.UpdateDetails();
                    }

                } else {
                    DetailsPanelController details = GetComponent<DetailsPanelController>();
                    if (details)
                    {
                        details.SetMessage("Unable to find template at  : " + export.GetTemplatePath(Parameters.SerialEM_ver));
                        details.UpdateDetails();
                    }
                }
            }
        }
    }

    IEnumerator ShowSaveCSVDialogCoroutine(ExportCSV export)
    {
        yield return FileBrowser.WaitForSaveDialog(FileBrowser.PickMode.Files, true, null, "simulation.csv", "Save file", "Save");

        Debug.Log(FileBrowser.Success);

        if (FileBrowser.Success)
        {
            // Print paths of the selected files (FileBrowser.Result) (null, if FileBrowser.Success is false)
            if (FileBrowser.Result.Length > 0)
            {
                for (int i = 0; i < FileBrowser.Result.Length; i++)
                    Debug.Log(FileBrowser.Result[i]);

                // Save the macro to this path.
                string filename = FileBrowser.Result[0];
                export.WriteIntensities(filename);

                DetailsPanelController details = GetComponent<DetailsPanelController>();
                if (details)
                {
                    details.SetMessage("Exported Voxel Values to : " + Path.GetFullPath(filename));
                    details.UpdateDetails();
                }
            }
        }
    }
}
