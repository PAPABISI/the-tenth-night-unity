using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class LocalPlayer2DMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 4.5f;
    public bool normalizeDiagonal = true;
    public bool useUnscaledTime = false;

    [Header("Optional Visual")]
    public SpriteRenderer spriteRenderer;
    public bool flipSpriteByX = true;

    private Rigidbody2D _rb;
    private Vector2 _input;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        _input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (normalizeDiagonal && _input.sqrMagnitude > 1f)
            _input.Normalize();

        if (flipSpriteByX && spriteRenderer != null)
        {
            if (_input.x > 0.01f) spriteRenderer.flipX = false;
            else if (_input.x < -0.01f) spriteRenderer.flipX = true;
        }
    }

    private void FixedUpdate()
    {
        var dt = useUnscaledTime ? Time.fixedUnscaledDeltaTime : Time.fixedDeltaTime;
        var nextPos = _rb.position + _input * (moveSpeed * dt);
        _rb.MovePosition(nextPos);
    }
}
