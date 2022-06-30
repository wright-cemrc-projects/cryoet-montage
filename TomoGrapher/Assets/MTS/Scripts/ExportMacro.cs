using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class ExportMacro : MonoBehaviour
{
    public string TemplateName = "cryoMontage_template.txt";

    /// Expected template location
    public string GetTemplatePath() {
        return Application.dataPath + "/" + TemplateName;
    }

    // Export of the voxel intensities to a file
    public void WriteMacro(string filename, SimulationParameters Parameters) {

        // Read a template which will have %VALUE% replaced with known values.
        string templatePath = GetTemplatePath();
        if (File.Exists(templatePath))
        {

            string[] lines = System.IO.File.ReadAllLines(templatePath);
            Debug.Log("Read template from: " + templatePath);

            StreamWriter writer = new StreamWriter(filename);
            Debug.Log("Exporting a macro to: " + Path.GetFullPath(filename));

            string KEYWORD_A_INITIAL = "%A_INITIAL%";
            string KEYWORD_A_FINAL = "%A_FINAL%";
            string KEYWORD_TURNS = "%TURNS%";
            string KEYWORD_PERIOD = "%PERIOD%";
            string KEYWORD_REVOLUTIONS = "%REVOLUTIONS%";
            string KEYWORD_START_ANGLE = "%START_ANGLE%";
            string KEYWORD_END_AT = "%END_AT%";
            string KEYWORD_TILT_TO = "%TILT_TO%";
            string KEYWORD_TILT_STEP = "%TILT_STEP%";
            string KEYWORD_LOG_DIR = "%LOG_DIR%";
            string KEYWORD_TILT_SCHEME = "%TILT_SCHEME%";

            foreach (string line in lines)
            {
                // Replace %VALUE% with actual values.
                string out_line = line;

                if (line.Contains(KEYWORD_START_ANGLE))
                {
                    Debug.Log(KEYWORD_START_ANGLE);
                    out_line = line.Replace(KEYWORD_START_ANGLE, Parameters.TiltStart.ToString("0.0"));
                }

                if (line.Contains(KEYWORD_END_AT))
                {
                    Debug.Log(KEYWORD_END_AT);
                    out_line = line.Replace(KEYWORD_END_AT, Parameters.TiltEnd.ToString("0.0"));
                }

                if (line.Contains(KEYWORD_TILT_TO))
                {
                    Debug.Log(KEYWORD_TILT_TO);
                    out_line = line.Replace(KEYWORD_TILT_TO, Parameters.TiltTo.ToString("0.0"));
                }

                if (line.Contains(KEYWORD_TILT_STEP))
                {
                    Debug.Log(KEYWORD_TILT_STEP);
                    out_line = line.Replace(KEYWORD_TILT_STEP, Parameters.TiltIncrement.ToString("0.0"));
                }

                if (line.Contains(KEYWORD_LOG_DIR))
                {
                    Debug.Log(KEYWORD_LOG_DIR);
                    out_line = line.Replace(KEYWORD_LOG_DIR, Parameters.LogDirectory);
                }

                if (line.Contains(KEYWORD_A_INITIAL))
                {
                    Debug.Log(KEYWORD_A_INITIAL);
                    out_line = line.Replace(KEYWORD_A_INITIAL, Parameters.AmpInitial.ToString("0.0"));
                }

                if (line.Contains(KEYWORD_A_FINAL))
                {
                    Debug.Log(KEYWORD_A_FINAL);
                    out_line = line.Replace(KEYWORD_A_FINAL, Parameters.AmpFinal.ToString("0.0"));
                }

                if (line.Contains(KEYWORD_TURNS))
                {
                    Debug.Log(KEYWORD_TURNS);
                    out_line = line.Replace(KEYWORD_TURNS, Parameters.Turns.ToString("0.0"));
                }

                if (line.Contains(KEYWORD_PERIOD))
                {
                    Debug.Log(KEYWORD_PERIOD);
                    out_line = line.Replace(KEYWORD_PERIOD, Parameters.Period.ToString("0.0"));
                }

                if (line.Contains(KEYWORD_REVOLUTIONS))
                {
                    Debug.Log(KEYWORD_REVOLUTIONS);
                    out_line = line.Replace(KEYWORD_REVOLUTIONS, Parameters.Revolutions.ToString("0.0"));
                }

                if (line.Contains(KEYWORD_TILT_SCHEME))
                {
                    Debug.Log(KEYWORD_TILT_SCHEME);
                    out_line = line.Replace(KEYWORD_TILT_SCHEME, Parameters.DoseSymmetric.ToString());
                }

                writer.WriteLine(out_line);
            }

            writer.Close();
        } else
        {
            Debug.Log("Missing template: unable to write SerialEM Macro from: " + templatePath);
        }
    }
}
