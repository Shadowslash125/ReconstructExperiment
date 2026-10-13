using System.Collections.Generic;
using UnityEngine;
using Assets.Actors.Man.States;

public class ManController : MonoBehaviour
{
    public ManChMovementData onFootMoveData;

    public float turnSpeed = 510f;

    [Header("Controller Output")]
    public Vector3 inputMotion = Vector3.zero;
    public bool inputJump = false;
    public bool inputAim = false;
    public int inputWeaponSlot = -1;

    public Vector3 velocity = Vector3.zero;

    [HideInInspector] public Vector3 lastMotion;
    [HideInInspector] public Vector3 lookEuler;

    private readonly Dictionary<string, ManState> states = new();
    private ManState curState;

    private string prevState = "START";

    private bool firstUpdate = true;

    private CharacterController character;
    [HideInInspector] public ManAnim anim;

    private void Start()
    {
        character = GetComponent<CharacterController>();
        anim = GetComponent<ManAnim>();

        lookEuler = new Vector3(0f, transform.eulerAngles.y, 0f);
        lastMotion = transform.forward;
        velocity = Vector3.zero;

        AddState("idle", new Idle());
        AddState("run", new Run());
        AddState("jump", new Jump());
        AddState("fall", new Fall());

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
        curState.Update(Time.deltaTime);

        character.Move(velocity * Time.deltaTime);

        if (inputWeaponSlot >= 0)
        {
            anim.SetWeapon(inputWeaponSlot);
        }

        anim.SetVelocity(velocity / onFootMoveData.maxSpeed);
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

    private void ApplyFriction(float deltaTime)
    {
        Vector3 vel = new Vector3(velocity.x, 0f, velocity.z);
        vel = Vector3.MoveTowards(vel, Vector3.zero, onFootMoveData.friction * deltaTime);
        velocity = new Vector3(vel.x, velocity.y, vel.z);
    }

    private void ApplyMotionH(float deltaTime)
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
            velocity.y = -3.7f;
            return;
        }
        velocity.y += Physics.gravity.y * onFootMoveData.gravityScale * deltaTime;
        velocity.y = Mathf.Max(velocity.y, -onFootMoveData.maxFallSpeed);
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
}
