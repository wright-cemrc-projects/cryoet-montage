using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beam : MonoBehaviour
{
    // Radius of the beam in unit X
    public float BeamRadius = 1.0f;
    // Sampling spacing
    public float SamplingSpacing = 1.0f;
    // Dose per shot
    public float Dose = 1.0f;
    // Visible beam
    public GameObject VisibleBeam;
    public GameObject VisibleBeamCylinder;

    // Hide delay
    public float Delay = 0.05f;

    // What way to shoot beam
    public GameObject Direction;

    // Start is called before the first frame update
    void Start()
    {
        // RayIntersectVoxels();
        // StartCoroutine(ExecuteAfterTime(0.1f));
        // VisibleBeam.SetActive(false);
        UpdateBeamScale();
    }

     IEnumerator HideBeamDelay(float time)
    {
        //yield return new WaitForSeconds(time);
        // Code to execute after the delay
        // RayIntersectVoxels();

        yield return new WaitForSeconds(time);
        VisibleBeam.SetActive(false);
    }

    public void RayIntersectVoxels() {
        // Generate a grid of rays (BeamRadius divided by the SamplingSpacing to create a grid of rays)
        // For each ray, find all the voxes

        Vector3 direction = Direction.transform.forward;

        VisibleBeam.SetActive(true);
        HashSet<GameObject> voxels = new HashSet<GameObject>();
        StopAllCoroutines();
        StartCoroutine(HideBeamDelay(Delay));

        // Range -1 to 1 is X
        List<double> values = new List<double>();
        double v = 0.0;
        values.Add(v);
        v += SamplingSpacing;
        while (v < BeamRadius)
        {
            values.Add(v);
            values.Add(v * -1.0);
            v += SamplingSpacing;
        }

        //Debug.Log("Start " + transform.position);
        //Debug.Log("Direction " + direction);

        foreach (double x in values)
        {
            foreach (double y in values)
            {
                if (x * x + y * y <= BeamRadius * BeamRadius)
                {
                    RaycastHit[] hits;
                    Vector3 start = transform.position;
                    start.Set(start.x + (float) x, start.y, start.z + (float)y);
                    hits = Physics.RaycastAll(start, direction, 100.0F);

                    for (int i = 0; i < hits.Length; i++)
                    {
                        RaycastHit hit = hits[i];
                        GameObject voxel = hit.collider.gameObject;
                        if (voxel.GetComponent<VoxelBehaviour>() != null)
                        {
                            voxels.Add(voxel);
                        }
                    }
                }
            }
        }

        foreach (GameObject g in voxels)
        {
            VoxelBehaviour voxel = g.GetComponent<VoxelBehaviour>();

            // TODO: Dose* = Voxel Area * Dose Rate
            // This should be computed using Angstrom^2 area.
            voxel.AddDose(Dose);
            voxel.SetColorFromDose();
        }
    }

    /// 
    /// Change the beam radius visible on the grid and its intersections.
    ///
    public void SetBeamRadius(float beamRadius) {
        BeamRadius = beamRadius;
        UpdateBeamScale();
    }

    private void UpdateBeamScale() {
        if (VisibleBeamCylinder) {
            VisibleBeamCylinder.transform.localScale = new Vector3(BeamRadius * 2.0f, 50.0f, BeamRadius * 2.0f);
        }

        SamplingSpacing = BeamRadius / 32f;
    }
}
