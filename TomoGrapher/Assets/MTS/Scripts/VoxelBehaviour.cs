using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelBehaviour : MonoBehaviour
{
    public Color VoxelColor;
    private Color Highlight;
    private Renderer _renderer;
    private MaterialPropertyBlock _propBlock;

    public double TotalDose = 0;
    public DetailsPanelController Details;

    /// <summary>
    ///  Dimensions in Angstrom.
    /// </summary>
    public float AngstromX = 1.0f;
    public float AngstromY = 1.0f;
    public float AngstromZ = 1.0f;

    /// Index of the voxel
    public int X;
    public int Y;
    public int Z;

    /// Add e-/A2 dose to this voxel
    public void AddDose(double dose) {
        TotalDose += dose;
    }

    /// Reset to no dose.
    public void ResetDose() {
        TotalDose = 0;
    }

    public void SetColorFromDose() {
        // Blue to red
        //Color c = new Color((float) (TotalDose) / 256.0f, 0, (float) (255-TotalDose) / 255.0f, 255);

        // White to black
        Color c = new Color((float) (255-TotalDose) / 255.0f, (float)(255 - TotalDose) / 255.0f, (float) (255-TotalDose) / 255.0f, 255);

        SetColor(c);
    }

    void Awake()
    {
        _propBlock = new MaterialPropertyBlock();
        _renderer = GetComponent<Renderer>();
        SetColorFromDose();
        UpdateColor();
        Highlight = new Color(255f, 255f, 0f);

        GameObject result = GameObject.Find("Toolkit");
        if (result) {
            Details = result.GetComponent<DetailsPanelController>();
        }
    }

    public void SetColor(Color newColor) {
        VoxelColor = newColor;
        UpdateColor();
    }

    private void UpdateColor() {
        // Get the current value of the material properties in the renderer.
        _renderer.GetPropertyBlock(_propBlock);
        // Assign our new value.
        _propBlock.SetColor("_Color", VoxelColor);
        // Apply the edited values to the renderer.
        _renderer.SetPropertyBlock(_propBlock);
    }

    void OnMouseOver() {
        Details.SetActiveVoxel(this);
        SetColor(Highlight);
        UpdateColor();
    }

    void OnMouseExit() {
        Details.SetActiveVoxel(null);
        SetColorFromDose();
        UpdateColor();
    }

    /*
    private void Awake()
    {
        // Create a random color
        Color32 newColor =
            new Color32(
                (byte)Random.Range(0, 256),
                (byte)Random.Range(0, 256),
                (byte)Random.Range(0, 256),
                255);
        SetColor(newColor);
    }

    public void SetColor(Color32 newColor) {
        // Get the mesh filter
        Mesh mesh = GetComponent< MeshFilter >().mesh;

        // Create an array of colors matching the same length as the mesh's color array
        Color32[] newColors = new Color32[mesh.vertices.Length];
        for (int vertexIndex = 0; vertexIndex < newColors.Length; vertexIndex++)
        {
            newColors[vertexIndex] = newColor;
        }

        // Apply the color
        mesh.colors32 = newColors;
    }*/
}
