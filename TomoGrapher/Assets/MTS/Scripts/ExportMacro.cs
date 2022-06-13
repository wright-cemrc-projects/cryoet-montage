using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class ExportMacro : MonoBehaviour
{
    public string TemplateName = "cryoMontage_template.txt";

    // Export of the voxel intensities to a file
    public void WriteMacro(string filename, SimulationParameters Parameters) {
        // Read a template which will have %VALUE% replaced with known values.
        string templatePath = Application.persistentDataPath + "/" + TemplateName;
        // StreamReader reader = new StreamReader(templatePath);
        // reader.Close();

        if (File.Exists(templatePath))
        {

            string[] lines = System.IO.File.ReadAllLines(templatePath);

            StreamWriter writer = new StreamWriter(filename);
            Debug.Log("Exporting a macro to: " + Path.GetFullPath(filename));

            string KEYWORD_A_INITIAL = "%A_INITIAL%";
            string KEYWORD_A_FINAL = "%A_FINAL%";
            string KEYWORD_TURNS = "%TURNS%";
            string KEYWORD_PERIOD = "%PERIOD%";
            string KEYWORD_REVOLUTIONS = "%REVOLUTIONS%";

            foreach (string line in lines)
            {
                // Replace %VALUE% with actual values.
                string out_line = line;

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

                writer.WriteLine(out_line);
            }

            writer.Close();
        } else
        {
            Debug.Log("Missing template: unable to write SerialEM Macro from: " + templatePath);
        }
    }
}
