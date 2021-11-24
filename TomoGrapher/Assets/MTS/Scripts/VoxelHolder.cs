using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///
/// Controller affecting all voxels in a grid.
///
public class VoxelHolder : MonoBehaviour
{
    public List<VoxelBehaviour> Voxels = new List<VoxelBehaviour>();

    public void ResetVoxels() {
        foreach (Transform t in transform) {
            if (t.gameObject.GetComponent<VoxelBehaviour>() != null)
            {
                // Reset the dose values and colors of voxels in the map.
                VoxelBehaviour voxel = t.gameObject.GetComponent<VoxelBehaviour>();
                voxel.ResetDose();
                voxel.SetColorFromDose();
            }
        }
    }

}
