using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class ExportCSV : MonoBehaviour
{
    // Assign this to the grid of Voxel instances
    public GameObject VoxelHolder;

    // Export of the voxel intensities to a file
    public void WriteIntensities(string filename) {
        StreamWriter writer = new StreamWriter(filename);
        Debug.Log("Wrote to " + Path.GetFullPath(filename));
        writer.WriteLine("X,Y,Z,TotalDose");

        List<VoxelBehaviour> voxels = VoxelHolder.GetComponent<VoxelHolder>().Voxels;
        foreach (VoxelBehaviour v in voxels) {
            // for each voxel
            // get the coordinate x,z
            // query for the TotalDose
            // write these values out to the CSV.

            // Then can plot the values.

            double dose = v.TotalDose;
            writer.WriteLine(v.X + "," + v.Y + "," + v.Z + "," + dose);
        }
        writer.Close();
    }
}
