using UnityEngine;
using UnityEngine.InputSystem;


public class Painter : MonoBehaviour
{
    public InputActionReference paint;
    public InputActionReference erase;

    private bool is_painting;
    private bool is_erasing;

    private GridController gc;
    private float pixels_per_unit;

    private Vector2 mouse_world_position;
    private Vector2Int mouse_grid_position;

    public void Initialize(ref GridController grid_controller, float pixels_per_unit)
    {
        gc = grid_controller;
        this.pixels_per_unit = pixels_per_unit;

        is_painting = false;
        is_erasing = false;
    }

    void OnEnable()
    {
        paint.action.Enable();
        erase.action.Enable();
    }

    void OnDisable()
    {
        paint.action.Disable();
        erase.action.Disable();
    }

    void Update()
    {
        UpdateMousePosition();
        UpdateActions();
        Paint();
        Erase();
    }

    private void UpdateMousePosition()
    {
        Vector2 mouse_screen_position = Mouse.current.position.ReadValue();
        mouse_world_position = Camera.main.ScreenToWorldPoint(mouse_screen_position);

        float grid_bottom_left_x = -(gc.Width / (2f * pixels_per_unit));
        float grid_bottom_left_y = -(gc.Height / (2f * pixels_per_unit));

        mouse_grid_position = new Vector2Int(
            Mathf.FloorToInt((mouse_world_position.x - grid_bottom_left_x) * pixels_per_unit),
            Mathf.FloorToInt((mouse_world_position.y - grid_bottom_left_y) * pixels_per_unit)
        );
    }

    private void UpdateActions()
    {
        if (paint.action.WasPressedThisFrame()) is_painting = true;
        if (paint.action.WasReleasedThisFrame()) is_painting = false;

        if (erase.action.WasPressedThisFrame()) is_erasing = true;
        if (erase.action.WasReleasedThisFrame()) is_erasing = false;
    }

    private void Paint()
    {
        if (is_painting && gc.IsValidPosition(mouse_grid_position.x, mouse_grid_position.y))
        {
            Debug.Log($"Paint at {mouse_grid_position}");
            gc.Set(
                mouse_grid_position.x,
                mouse_grid_position.y,
                new Particle
                {
                    type = ParticleType.Sand,
                    side_bias = Random.Range(0, 2) == 0 ? -1 : 1,
                }
            );
        }
    }

    private void Erase()
    {
        if (is_erasing && gc.IsValidPosition(mouse_grid_position.x, mouse_grid_position.y))
        {
            Debug.Log($"Erase at {mouse_grid_position}");
            gc.Set(
                mouse_grid_position.x,
                mouse_grid_position.y,
                new Particle
                {
                    type = ParticleType.Air,
                }
            );
        }
    }
}