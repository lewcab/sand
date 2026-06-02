using System;

/// <summary>
/// Handles simulation grid CRUD operations
/// </summary>
public class GridController
{
    private int width;
    private int height;

    public int Width => width;
    public int Height => height;

    // Position (0, 0) refers to bottom left corner
    private Particle[] current;
    private Particle[] next;

    public GridController(int width, int height)
    {
        this.width = width;
        this.height = height;

        current = new Particle[width * height];
        next = new Particle[width * height];
    }

    public Particle[] ReadCurrent()
    {
        return current;
    }

    public Particle Get(int x, int y, bool current_buffer = true)
    {
        if (!IsValidPosition(x, y))
            throw new IndexOutOfRangeException(
                $"Coordinates ({x}, {y}) are out of bounds. Grid size: {width}x{height}"
            );

        int i = x + y * width;
        return current_buffer ? current[i] : next[i];
    }

    public void Set(int x, int y, Particle p, bool current_buffer = true)
    {
        if (!IsValidPosition(x, y))
            throw new IndexOutOfRangeException(
                $"Coordinates ({x}, {y}) are out of bounds. Grid size: {width}x{height}"
            );

        int i = x + y * width;
        if (current_buffer) current[i] = p;
        else next[i] = p;
    }

    public bool IsValidPosition(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    public void Swap((int x, int y) p1, (int x, int y) p2)
    {
        Particle temp = Get(p1.x, p1.y);
        Set(p1.x, p1.y, Get(p2.x, p2.y));
        Set(p2.x, p2.y, temp);
    }

    public void ClearGrid(
        ParticleType particle_type = ParticleType.Air,
        bool current_buffer = false
    )
    {
        for (int i = 0; i < width * height; i++)
        {
            Particle p = new()
            {
                type = particle_type,
            };

            if (current_buffer)
                current[i] = p;
            else
                next[i] = p;
        }
    }

    public void CopyGrid(bool to_next = true)
    {
        if (to_next)
            Array.Copy(current, next, width * height);
        else
            Array.Copy(next, current, width * height);
    }
}
