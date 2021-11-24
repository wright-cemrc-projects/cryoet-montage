using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBuilder : MonoBehaviour
{
    public GameObject GridBoxPrefab;
    public GameObject GridHolder;
    public int X_Dimension = 1;
    public int Y_Dimension = 1;
    public int Z_Dimension = 1;
    public float X_Size = 1.0f;
    public float Y_Size = 1.0f;
    public float Z_Size = 1.0f;

    private void Awake()
    {
        float x = 0;
        float y = 0;
        float z = 0;

        // X, Y, Z in micron, determine per voxel size and convert to angstrom.
        float AngstromX = X_Size / X_Dimension * 1000.0f * 10.0f;
        float AngstromY = Y_Size / Y_Dimension * 1000.0f * 10.0f;
        float AngstromZ = Z_Size / Z_Dimension * 1000.0f * 10.0f;

        for (int i = 0; i < X_Dimension; i++)
        {
            for (int j = 0; j < Y_Dimension; j++)
            {
                for (int k = 0; k < Z_Dimension; k++)
                {
                    GameObject gridBox = Instantiate(GridBoxPrefab);
                    gridBox.transform.parent = GridHolder.transform;
                    gridBox.transform.localPosition = new Vector3(x, y, z);
                    VoxelBehaviour b = gridBox.GetComponent<VoxelBehaviour>();
                    b.AngstromX = AngstromX;
                    b.AngstromY = AngstromY;
                    b.AngstromZ = AngstromZ;
                    b.X = i;
                    b.Y = j;
                    b.Z = k;
                    z++;
                    GridHolder.GetComponent<VoxelHolder>().Voxels.Add(b);
                }
                z = 0;
                y++;
            }
            y = 0;
            x++;
        }

        ScaleHolder();
    }

    private void ScaleHolder()
    {
        if (GridHolder)
        {
            GridHolder.transform.localScale = new Vector3(X_Size* 1.0f / X_Dimension, Y_Size* 1.0f / Y_Dimension, Z_Size* 1.0f / Z_Dimension);
            GridHolder.transform.position = new Vector3(-X_Size / 2.0f, -Y_Size / 2.0f, -Z_Size / 2.0f);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
