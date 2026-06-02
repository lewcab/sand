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

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Initialize(int width, int height, float pixels_per_unit)
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
                pixelBuffer[i] = GetColor(grid[i], i, texture.width);
            }
            catch (Exception)
            {
                pixelBuffer[i] = air_color;
            }
        }

        texture.SetPixels32(pixelBuffer);
        texture.Apply(false);
    }

    private Color32 GetColor(Particle p, int index, int width)
    {
        Color32 baseColor = p.type switch
        {
            ParticleType.Air => air_color,
            ParticleType.Sand => sand_color,
            _ => sand_color,
        };

        if (p.type == ParticleType.Air) return baseColor;

        // Use position hash for deterministic but varied results
        int hash = (index * 73856093) ^ ((index / width) * 19349663);
        float noise = ((hash ^ (hash >> 16)) & 0xFF) / 255f;
        float variance = Mathf.Lerp(-15f, 15f, noise);

        return new Color32(
            (byte)Mathf.Clamp(baseColor.r + variance, 0, 255),
            (byte)Mathf.Clamp(baseColor.g + variance, 0, 255),
            (byte)Mathf.Clamp(baseColor.b + variance, 0, 255),
            baseColor.a
        );
    }
}