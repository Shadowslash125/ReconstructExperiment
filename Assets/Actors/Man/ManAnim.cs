using UnityEngine;

public class ManAnim : MonoBehaviour
{
    public const int STATE_IDLE = 0;
    public const int STATE_RUN = 1;
    public const int STATE_AIM = 2;

    public const int LAYER_UPPER_BODY = 1;

    public const int WEAPON_PISTOL = 0;
    public const int WEAPON_RIFLE = 1;
    public const int WEAPON_DUAL = 2;

    public const int MOVEMENT_ON_FOOT = 0;
    public const int MOVEMENT_FLY_GROUND = 1;
    public const int MOVEMENT_FLY = 2;

    public const float LAYER_WEIGHT_MOVE_SPEED = 2f;
    public const float LOOK_SPEED_DEG = 250f;

    public Animator animator;

    private float upperBodyLayerWeightTarget = 0f;
    private float targetEulerX = 0f;

    private int hashState;
    private int hashPistol;
    private int hashRifle;
    private int hashDual;
    private int hashOnFoot;
    private int hashFlyGround;
    private int hashFly;
    private int hashMoveX;
    private int hashMoveY;
    private int hashEulerX;

    private int prevHashMovement;
    private int curHashMovement;

    private int prevHashWeapon;
    private int curHashWeapon;

    private void Awake()
    {
        hashState = Animator.StringToHash("State");
        hashPistol = Animator.StringToHash("Pistol");
        hashRifle = Animator.StringToHash("Rifle");
        hashDual = Animator.StringToHash("Dual");
        hashOnFoot = Animator.StringToHash("OnFoot");
        hashFlyGround = Animator.StringToHash("FlyGround");
        hashFly = Animator.StringToHash("Fly");
        hashMoveX = Animator.StringToHash("MoveX");
        hashMoveY = Animator.StringToHash("MoveY");
        hashEulerX = Animator.StringToHash("EulerX");

        prevHashMovement = hashFly;
        curHashMovement = hashOnFoot;

        prevHashWeapon = hashRifle;
        curHashWeapon = hashPistol;
    }

    private void Update()
    {
        animator.SetFloat(prevHashMovement, 0f, 0.1f, Time.deltaTime);
        animator.SetFloat(curHashMovement, 1f, 0.1f, Time.deltaTime);

        animator.SetFloat(prevHashWeapon, 0f, 0.1f, Time.deltaTime);
        animator.SetFloat(curHashWeapon, 1f, 0.1f, Time.deltaTime);

        float eulerX = Mathf.MoveTowardsAngle(animator.GetFloat(hashEulerX), targetEulerX, LOOK_SPEED_DEG * Time.deltaTime);
        animator.SetFloat(hashEulerX, eulerX);

        float v = Mathf.MoveTowards(animator.GetLayerWeight(LAYER_UPPER_BODY), upperBodyLayerWeightTarget, LAYER_WEIGHT_MOVE_SPEED * Time.deltaTime);
        animator.SetLayerWeight(LAYER_UPPER_BODY, v);
    }

    public void SetState(int state)
    {
        animator.SetInteger(hashState, state);
    }

    public void SetWeapon(int weapon)
    {
        int newHash = weapon switch
        {
            WEAPON_PISTOL => hashPistol,
            WEAPON_RIFLE => hashRifle,
            _ => hashDual,
        };
        
        if (newHash == curHashWeapon)
        {
            return;
        }

        animator.SetFloat(prevHashWeapon, 0f);
        animator.SetFloat(curHashWeapon, 1f);

        prevHashWeapon = curHashWeapon;
        curHashWeapon = newHash;

        animator.SetFloat(prevHashWeapon, 1f);
        animator.SetFloat(curHashWeapon, 0f);
    }

    public void SetMovement(int movement)
    {
        int newHash = movement switch
        {
            MOVEMENT_ON_FOOT => hashOnFoot,
            MOVEMENT_FLY_GROUND => hashFlyGround,
            _ => hashFly,
        };

        if (newHash == curHashMovement)
        {
            return;
        }

        animator.SetFloat(prevHashMovement, 0f);
        animator.SetFloat(curHashWeapon, 1f);

        prevHashMovement = curHashMovement;
        curHashMovement = newHash;

        animator.SetFloat(prevHashMovement, 1f);
        animator.SetFloat(curHashMovement, 0f);
    }

    public void SetUpperLayerWeight(float weight)
    {
        upperBodyLayerWeightTarget = weight;
    }

    public void SetEulerX(float x)
    {
        targetEulerX = x;
    }

    public void SetVelocity(Vector3 velocity)
    {
        Vector3 vel = Quaternion.Inverse(transform.rotation) * velocity;
        animator.SetFloat(hashMoveX, vel.x);
        animator.SetFloat(hashMoveY, vel.z);
    }
}
