using System.Collections.Generic;
using UnityEngine;
using Assets.Actors.Man.States;

public class ManController : MonoBehaviour
{
    public ManChMovementData onFootMoveData;
    public ManChMovementData flyMoveData;

    public float turnSpeed = 510f;

    [Header("Controller Output")]
    public Vector3 inputMotion = Vector3.zero;
    public bool inputJump = false;
    public bool inputAim = false;
    public bool inputSprint = false;
    public int inputWeaponSlot = -1;

    public Vector3 velocity = Vector3.zero;

    [HideInInspector] public Vector3 lastMotion;
    [HideInInspector] public Vector3 lookEuler;
    [HideInInspector] public ManChMovementData curMovementData;

    private readonly Dictionary<string, ManState> states = new();
    private ManState curState;

    private string prevState = "START";

    private bool firstUpdate = true;

    private float flyDashCooldown = 0f;

    private CharacterController character;
    [HideInInspector] public ManAnim anim;

    private void Start()
    {
        character = GetComponent<CharacterController>();
        anim = GetComponent<ManAnim>();

        curMovementData = onFootMoveData;

        lookEuler = new Vector3(0f, transform.eulerAngles.y, 0f);
        lastMotion = transform.forward;
        velocity = Vector3.zero;

        AddState("idle", new Idle());
        AddState("run", new Run());
        AddState("jump", new Jump());
        AddState("fall", new Fall());
        AddState("fly", new Fly());
        AddState("fly_dash", new FlyDash());

        anim.SetMovement(ManAnim.MOVEMENT_ON_FOOT);
        anim.SetWeapon(ManAnim.WEAPON_PISTOL);
    }

    private void Update()
    {
        if (firstUpdate)
        {
            firstUpdate = false;

            curState = states["idle"];
            curState.Enter(prevState);
            prevState = "idle";
        }
        flyDashCooldown = Mathf.MoveTowards(flyDashCooldown, 0f, Time.deltaTime);

        curState.Update(Time.deltaTime);

        character.Move(velocity * Time.deltaTime);

        if (inputWeaponSlot >= 0)
        {
            anim.SetWeapon(inputWeaponSlot);
        }

        anim.SetVelocity(velocity / curMovementData.maxSpeed);
    }

    private void AddState(string name, ManState state)
    {
        state.Init(this);
        states.Add(name, state);
    }

    public void ChangeState(string newState)
    {
        curState.Exit();
        curState = states[newState];
        curState.Enter(prevState);
        prevState = newState;
    }

    public void MotionPlatformer(float deltaTime)
    {
        if (inputMotion == Vector3.zero)
        {
            ApplyFriction(deltaTime);
        }
        else
        {
            ApplyMotionH(deltaTime);
            lastMotion = inputMotion;
        }
        ApplyMotionV(deltaTime);
    }

    public void MotionFlyGround(float deltaTime)
    {
        if (inputMotion == Vector3.zero)
        {
            ApplyFriction(deltaTime);
        }
        else
        {
            ApplyMotionH(deltaTime);
            lastMotion = inputMotion;
        }
        ApplyFlyMotionV(deltaTime);
    }

    public void MotionLinearH()
    {
        Vector3 motion = new Vector3(lastMotion.x, 0f, lastMotion.z).normalized;
        velocity = motion * curMovementData.flyDashForce;
    }

    private void ApplyFriction(float deltaTime)
    {
        Vector3 vel = new Vector3(velocity.x, 0f, velocity.z);
        float friction = character.isGrounded ? curMovementData.friction : curMovementData.airFriction;
        vel = Vector3.MoveTowards(vel, Vector3.zero, friction * deltaTime);
        velocity = new Vector3(vel.x, velocity.y, vel.z);
    }

    private void ApplyMotionH(float deltaTime)
    {
        Vector3 motion = new Vector3(inputMotion.x, 0f, inputMotion.z).normalized;
        Vector3 vel = new Vector3(velocity.x, 0f, velocity.z);
        float acceleration = character.isGrounded ? curMovementData.acceleration : curMovementData.airAcceleration;
        vel += motion * acceleration * deltaTime;
        vel = Vector3.ClampMagnitude(vel, curMovementData.maxSpeed);
        velocity = new Vector3(vel.x, velocity.y, vel.z);
    }

    private void ApplyMotionV(float deltaTime)
    {
        if (character.isGrounded)
        {
            velocity.y = -3.7f;
            return;
        }
        velocity.y += Physics.gravity.y * curMovementData.gravityScale * deltaTime;
        velocity.y = Mathf.Max(velocity.y, -curMovementData.maxFallSpeed);
    }

    private void ApplyFlyMotionV(float deltaTime)
    {
        if (character.isGrounded)
        {
            velocity.y = -3.7f;
            if (inputJump)
            {
                velocity.y = curMovementData.jumpForce;
            }
            return;
        }
        else
        {
            if (inputJump)
            {
                velocity.y = Mathf.MoveTowards(velocity.y, curMovementData.flyUpMaxSpeed, curMovementData.flyUpAcceleration * deltaTime);
                return;
            }
        }
        velocity.y += Physics.gravity.y * curMovementData.gravityScale * deltaTime;
        velocity.y = Mathf.Max(velocity.y, -curMovementData.maxFallSpeed);
    }

    public void TurnTowardLastMotion(float deltaTime)
    {
        Quaternion q2 = Quaternion.LookRotation(new Vector3(lastMotion.x, 0f, lastMotion.z));
        transform.rotation = Quaternion.RotateTowards(transform.rotation, q2, turnSpeed * deltaTime);

        anim.SetEulerX(0f);
    }

    public void TurnTowardLook(float deltaTime)
    {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0f, lookEuler.y, 0f), turnSpeed * deltaTime);
        //anim.SetEulerX(Mathf.DeltaAngle(0f, lookEuler.x));
        anim.SetEulerX(lookEuler.x);
    }

    public bool HasMotion()
    {
        return inputMotion != Vector3.zero;
    }

    public bool IsGrounded()
    {
        return character.isGrounded;
    }

    public bool RequestFlyDash()
    {
        if (flyDashCooldown > 0)
        {
            return false;
        }
        flyDashCooldown = curMovementData.flyDashCooldown;
        return true;
    }
}
