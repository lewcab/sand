using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SimulationRenderer : MonoBehaviour
{
    private Color32 air_color = new(10, 50, 70, 255);
    private Color32 sand_color = new(150, 150, 100, 255);

    private Texture2D texture;
    private Color32[] pixelBuffer;
    private SpriteRenderer spriteRenderer;

    private readonly float pixels_per_unit = 5f;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Initialize(int width, int height)
    {
        texture = new Texture2D(width, height);
        pixelBuffer = new Color32[width * height];

        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;

        spriteRenderer.sprite = Sprite.Create(
            texture,
            new Rect(0, 0, width, height),
            new Vector2(0.5f, 0.5f),
            pixels_per_unit
        );
        texture.filterMode = FilterMode.Point;
    }

    public void Render(Particle[] grid)
    {
        for (int i = 0; i < grid.Length; i++)
        {
            try
            {
                pixelBuffer[i] = GetColor(grid[i]);
            }
            catch (Exception)
            {
                pixelBuffer[i] = air_color;
            }
        }

        texture.SetPixels32(pixelBuffer);
        texture.Apply(false);
    }

    private Color32 GetColor(Particle p)
    {
        return p.type switch
        {
            ParticleType.Air => air_color,
            ParticleType.Sand => sand_color,
            _ => sand_color,
        };
    }
}