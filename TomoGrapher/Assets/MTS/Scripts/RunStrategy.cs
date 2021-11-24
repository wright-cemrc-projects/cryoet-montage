using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunStrategy : MonoBehaviour
{
    public List<Exposure> ShiftTiltStrategy = new List<Exposure>();
    public Beam beam;
    public GameObject stage;

    public double TimeInterval = 0.25;
    private double Timer;

    // The strategy will iterate through this.
    public int CurrentPoint = 0;
    public bool Running = false;

    void Start() {
        ResetSimulation();
    }

    // Update is called once per frame
    void Update()
    {
        Timer -= Time.deltaTime;

        if (Running && Timer < 0 && CurrentPoint < ShiftTiltStrategy.Count)
        {
            Timer = TimeInterval;

            Exposure exp = ShiftTiltStrategy[CurrentPoint];
            MoveImaging(exp.x, exp.y);
            TiltStage(exp.tiltDegrees);
            TakeImage();

            CurrentPoint++;
        }
    }

    private void MoveImaging(double x, double z) {
        // Update the x, and z positions.
        Debug.Log("Shift to " + x + ", " + z);
        beam.transform.position = new Vector3((float) x, (float) beam.transform.position.y, (float) z);
    }

    private void TiltStage(double degrees) {
        // Set the stage tilt
        Debug.Log("Tilt to " + degrees);
        Quaternion rot = new Quaternion();
        rot.eulerAngles = new Vector3(0, 0, (float)degrees);
        stage.transform.rotation = rot;
    }

    private void TakeImage() {
        // Move to the next tilt and fire.
        beam.RayIntersectVoxels();
    }

    public void StartSimulation() {
        Running = true;
    }

    public void PauseSimulation() {
        Running = false;
    }

    public void ResetSimulation() {
        Timer = TimeInterval;
        TiltStage(0);
        Running = false;
        CurrentPoint = 0;
    }
}
