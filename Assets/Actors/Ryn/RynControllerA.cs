using Assets.Actors.Ryn.States;
using System.Collections.Generic;
using UnityEngine;

public class RynControllerA : MonoBehaviour
{
    public const float TURN_SPEED_MOVEMENT = 500f;
    public const float REL_SPEED = 27f;

    public RynAnimBridge animBridge;

    public Transform enemyTarget;

    public float turnSpeed = 500f;

    [Header("Outputs(Don't Set")]
    public Vector3 velocity = Vector3.zero;
    public Vector3 lookEuler = Vector3.zero;
    public Vector3 lastMotion = Vector3.forward;

    public Vector3 inputMotion = Vector3.zero;

    private readonly Dictionary<string, RynState> states = new();

    private RynState curState;
    private string prevState = "START";

    private bool firstState = true;

    private CharacterController cc;
    [HideInInspector] public RynAnim anim;

    private void Start()
    {
        cc = GetComponent<CharacterController>();
        anim = GetComponent<RynAnim>();

        animBridge.eventLanceSlashLR.AddListener(OnLanceSlashLR);

        AddState("idle", new Idle());
        AddState("chase", new Chase());
        AddState("slash_attack", new SlashAttack());
        AddState("slash_attack2", new SlashAttack2());

        lastMotion = transform.forward;
    }

    private void Update()
    {
        if (firstState)
        {
            firstState = false;

            curState = states["idle"];
            curState.Enter(prevState);
            prevState = "idle";
        }

        curState.Update(Time.deltaTime);

        cc.Move(velocity * Time.deltaTime);
    }

    public void ChangeState(string newState)
    {
        curState.Exit();
        curState = states[newState];
        curState.Enter(prevState);
        prevState = newState;
    }

    public void AddState(string name, RynState state)
    {
        state.Init(this);
        states.Add(name, state);
    }

    public void ApplyMotion(Vector3 motion, float deltaTime)
    {
        float speed = 3f;

        velocity = motion * speed;

        anim.SetVelocity(velocity / speed);
    }

    public void HandleMotion(float deltaTime)
    {
        if (inputMotion == Vector3.zero)
        {
            velocity = Vector3.zero;
        }
        else
        {
            ApplyMotion(inputMotion, deltaTime);
            lastMotion = inputMotion;
        }
        TurnTowardLastMotion(deltaTime);
    }

    public void HandleDirRelMotion(Vector3 dir, float deltaTime)
    {
        Vector3 eulerSpeed = anim.GetRelTurnSpeed();
        float relSpeed = anim.GetRelSpeed() * REL_SPEED;
        Vector3 targetEuler = Quaternion.LookRotation(dir).eulerAngles;

        lookEuler.x = Mathf.MoveTowardsAngle(lookEuler.x, targetEuler.x, eulerSpeed.x * deltaTime);
        lookEuler.y = Mathf.MoveTowardsAngle(lookEuler.y, targetEuler.y, eulerSpeed.y * deltaTime);

        velocity = Quaternion.Euler(lookEuler) * Vector3.forward * relSpeed;

        transform.rotation = Quaternion.Euler(0f, lookEuler.y, 0f);
    }

    public void TurnTowardLastMotion(float deltaTime)
    {
        transform.rotation = Quaternion.RotateTowards(transform.rotation,
                                                        Quaternion.LookRotation(new Vector3(lastMotion.x, 0f, lastMotion.z)),
                                                        turnSpeed * deltaTime);
    }

    public void SetLookAtDir(Vector3 dir)
    {
        Vector3 targetEuler = Quaternion.LookRotation(dir).eulerAngles;
        lookEuler.x = Mathf.MoveTowardsAngle(lookEuler.x, targetEuler.x, 9999f);
        lookEuler.y = Mathf.MoveTowardsAngle(lookEuler.y, targetEuler.y, 9999f);
    }

    public void SetLookAtTransform(Transform t)
    {
        SetLookAtDir((t.position - transform.position).normalized);
    }

    public Vector3 DirToTarget(Transform target)
    {
        return (target.position - transform.position).normalized;
    }

    public float DistToTarget(Transform target)
    {
        return Vector3.Distance(transform.position, target.position);
    }

    private void OnLanceSlashLR()
    {
        Debug.Log("RynControllerA: Lance Slash LR!");
    }

    private void OnLanceSlashRL()
    {

    }
}
