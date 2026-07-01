using System.Collections.Generic;
using UnityEngine;

public class SimulationController : MonoBehaviour
{
    public int width;
    public int height;
    public float pixels_per_unit;

    private GridController gc;
    public SimulationRenderer sr;
    public Painter p;

    private static readonly HashSet<ParticleType> SOLID_PARTICLE_TYPES = new()
    {
        ParticleType.Sand,
    };

    void Start()
    {
        gc = new GridController(width, height);
        sr.Initialize(width, height, pixels_per_unit);
        p.Initialize(ref gc, pixels_per_unit);
    }

    void FixedUpdate()
    {
        Tick();
    }

    void Update()
    {
        sr.Render(gc.ReadCurrent());
    }

    /// <summary>
    /// Handles entire simulation iteration
    /// </summary>
    public void Tick()
    {
        gc.ClearGrid();
        for (int y = 0; y < gc.Height; y++)
            for (int x = 0; x < gc.Width; x++)
                SimulateCell(x, y);

        gc.CopyGrid(to_next: false);
    }

    /// <summary>
    /// Given a cell, calculates and updates its state in the next frame
    /// </summary>
    /// <param name="x">Horizontal position of particle to simulate</param>
    /// <param name="y">Vertical position of particle to simulate</param>
    private void SimulateCell(int x, int y)
    {
        Particle p = gc.Get(x, y);

        if (p.type == ParticleType.Air) SimulateAir(x, y);
        else if (p.type == ParticleType.Sand) SimulateSand(x, y, p.side_bias);
    }

    private void SimulateAir(int x, int y)
    {
        return;
    }

    /// <summary>
    /// Simulate Sand particle
    /// Falls straight down until reaches bottom of grid or other (wet) sand particle
    /// If "resting", checks below left and right positions to settle into
    /// </summary>
    private void SimulateSand(int x, int y, int side_bias)
    {
        Particle p = gc.Get(x, y);

        if (
            gc.IsValidPosition(x, y - 1) &&
            !SOLID_PARTICLE_TYPES.Contains(gc.Get(x, y - 1).type))
        {
            // Below is empty
            gc.Set(x, y - 1, p, false);
        }
        else if (
            gc.IsValidPosition(x + side_bias, y - 1) &&
            !SOLID_PARTICLE_TYPES.Contains(gc.Get(x + side_bias, y - 1).type)
        )
        {
            // Below and to the side is empty
            gc.Set(x + side_bias, y - 1, p, false);
        }
        else if (
            gc.IsValidPosition(x - side_bias, y - 1) &&
            !SOLID_PARTICLE_TYPES.Contains(gc.Get(x - side_bias, y - 1).type)
        )
        {
            // Below and to other side is empty
            gc.Set(x - side_bias, y - 1, p, false);
        }
        else
        {
            // All options reserved, stay put
            gc.Set(x, y, p, false);
        }
    }
}
