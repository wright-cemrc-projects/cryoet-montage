using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DetailsPanelController : MonoBehaviour
{
    public VoxelBehaviour ActiveVoxel;
    public float CurrentTilt;
    TextField DetailsPanel;

    private Queue<string> MessageLog = new Queue<string>();

    public void SetActiveVoxel(VoxelBehaviour voxel) {
        if (voxel != ActiveVoxel) {
            ActiveVoxel = voxel;
            UpdateDetails();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        UIDocument document = GetComponent<UIDocument>();
        DetailsPanel = document.rootVisualElement.Q<TextField>("DetailsPanel");
    }

    public void SetMessage(string message)
    {
        MessageLog.Enqueue(message);
    }

    public void UpdateDetails() {
        if (ActiveVoxel) {
            MessageLog.Enqueue( "Dose " + string.Format("{0}", ActiveVoxel.TotalDose) + " e- / angstrom^2");
        }

        while (MessageLog.Count > 3)
        {
            MessageLog.Dequeue();
        }

        string textBlob = "";

        while (MessageLog.Count > 0)
        {
            textBlob += MessageLog.Dequeue();
        }

        DetailsPanel.SetValueWithoutNotify(textBlob);
    }
}
