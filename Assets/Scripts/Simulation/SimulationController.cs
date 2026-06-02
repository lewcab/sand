using System.Collections.Generic;
using UnityEngine;

public class SandSystem : MonoBehaviour
{
    public int width;
    public int height;

    private GridController gc;
    public SimulationRenderer sr;

    private static readonly HashSet<ParticleType> SOLID_PARTICLE_TYPES = new()
    {
        ParticleType.Sand,
    };

    void Start()
    {
        gc = new GridController(width, height);
        sr.Initialize(width, height);

        // Test: spawn some piles of sand
        for (int i = 0; i < 20; i++)
        {
            gc.Set((width / 2) - 5, height / 2 + i, new Particle
            {
                type = ParticleType.Sand,
                side_bias = (i % 2 == 0) ? -1 : 1,
            });
        }

        for (int j = 0; j < 10; j++)
        {
            gc.Set(width / 2, height / 2 + j - 6, new Particle
            {
                type = ParticleType.Sand,
                side_bias = (j % 2 == 0) ? -1 : 1,
            });
        }

        for (int k = 0; k < 30; k++)
        {
            gc.Set((width / 2) + 3, height / 2 + k - 11, new Particle
            {
                type = ParticleType.Sand,
                side_bias = (k % 2 == 0) ? -1 : 1,
            });
        }
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
        gc.CopyGrid();

        for (int y = 0; y < gc.Height; y++)
            for (int x = 0; x < gc.Width; x++)
                SimulateCell(x, y);
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
        if (!gc.IsValidPosition(x, y - 1)) return;

        if (!SOLID_PARTICLE_TYPES.Contains(gc.Get(x, y - 1).type))
        {
            // Below is empty
            gc.Swap((x, y), (x, y - 1));
        }
        else if (!SOLID_PARTICLE_TYPES.Contains(gc.Get(x + side_bias, y - 1).type))
        {
            // Below and to the side is empty
            gc.Swap((x, y), (x + side_bias, y - 1));
        }
        else if (!SOLID_PARTICLE_TYPES.Contains(gc.Get(x - side_bias, y - 1).type))
        {
            // Below and to other side is empty
            gc.Swap((x, y), (x - side_bias, y - 1));
        }
    }
}
