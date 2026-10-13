using UnityEngine;

public class ManController : MonoBehaviour
{
    public ManChMovementData onFootMoveData;

    public float turnSpeed = 510f;

    [Header("Controller Output")]
    public Vector3 inputMotion = Vector3.zero;
    public bool inputJump = false;
    public bool inputAim = false;

    public Vector3 velocity = Vector3.zero;

    [HideInInspector] public Vector3 lastMotion;
    [HideInInspector] public Vector3 lookEuler;

    private CharacterController character;
    private ManAnim anim;

    private void Start()
    {
        character = GetComponent<CharacterController>();
        anim = GetComponent<ManAnim>();

        lookEuler = new Vector3(0f, transform.eulerAngles.y, 0f);
        lastMotion = transform.forward;
        velocity = Vector3.zero;
    }

    private void Update()
    {
        if (inputMotion == Vector3.zero)
        {
            ApplyFriction(Time.deltaTime);
            if (inputAim)
                anim.SetState(ManAnim.STATE_AIM);
            else
                anim.SetState(ManAnim.STATE_IDLE);
        }
        else
        {
            ApplyMotion(Time.deltaTime);
            lastMotion = inputMotion;
            if (inputAim)
                anim.SetState(ManAnim.STATE_AIM);
            else
                anim.SetState(ManAnim.STATE_RUN);
        }
        ApplyMotionV(Time.deltaTime);

        character.Move(velocity * Time.deltaTime);

        if (inputAim)
        {
            TurnTowardLook(Time.deltaTime);
            anim.SetLayer(ManAnim.LAYER_UPPER_BODY, 1f);
            anim.SetEulerX(lookEuler.x);
        }
        else
        {
            TurnTowardLastMotion(Time.deltaTime);
            anim.SetLayer(ManAnim.LAYER_UPPER_BODY, 0f);
            anim.SetEulerX(0f);
        }

        anim.SetVelocity(velocity / onFootMoveData.maxSpeed);
    }

    private void ApplyFriction(float deltaTime)
    {
        Vector3 vel = new Vector3(velocity.x, 0f, velocity.z);
        vel = Vector3.MoveTowards(vel, Vector3.zero, onFootMoveData.friction * deltaTime);
        velocity = new Vector3(vel.x, velocity.y, vel.z);
    }

    private void ApplyMotion(float deltaTime)
    {
        Vector3 motion = new Vector3(inputMotion.x, 0f, inputMotion.z).normalized;
        Vector3 vel = new Vector3(velocity.x, 0f, velocity.z);
        vel += motion * onFootMoveData.acceleration * deltaTime;
        vel = Vector3.ClampMagnitude(vel, onFootMoveData.maxSpeed);
        velocity = new Vector3(vel.x, velocity.y, vel.z);
    }

    private void ApplyMotionV(float deltaTime)
    {
        if (character.isGrounded)
        {
            if (inputJump)
            {
                velocity.y = onFootMoveData.jumpForce;
                return;
            }
            velocity.y = -3.7f;
            return;
        }
        velocity.y += Physics.gravity.y * onFootMoveData.gravityScale * deltaTime;
        velocity.y = Mathf.Max(velocity.y, -onFootMoveData.maxFallSpeed);
    }

    private void TurnTowardLastMotion(float deltaTime)
    {
        Quaternion q2 = Quaternion.LookRotation(new Vector3(lastMotion.x, 0f, lastMotion.z));
        transform.rotation = Quaternion.RotateTowards(transform.rotation, q2, turnSpeed * deltaTime);
    }

    private void TurnTowardLook(float deltaTime)
    {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0f, lookEuler.y, 0f), turnSpeed * deltaTime);
    }
}
