using UnityEngine;

[CreateAssetMenu(fileName = "ManChMovementData", menuName = "Scriptable Objects/ManChMovementData")]
public class ManChMovementData : ScriptableObject
{
    public float acceleration = 60f;
    public float airAcceleration = 22f;
    public float maxSpeed = 3.7f;
    public float friction = 17f;
    public float airFriction = 3f;
    public float gravityScale = 1f;
    public float maxFallSpeed = 25f;
    public float jumpForce = 5.2f;

    [Header("Flying")]
    public float flyUpAcceleration = 40f;
    public float flyUpMaxSpeed = 7.5f;
    public float flyDashForce = 13.6f;
    public float maxFlyDashTime = 0.45f;
    public float flyDashCooldown = 0.8f;
}
