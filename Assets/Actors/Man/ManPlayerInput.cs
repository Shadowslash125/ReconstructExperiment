using UnityEngine;
using UnityEngine.InputSystem;

public class ManPlayerInput : MonoBehaviour
{
    private const float LOCK_ON_DETECTOR_RADIUS = 20f;

    public Transform man;
    public float manOffsetY = 1.7f;

    public InputActionAsset playerActions;

    public float mouseSensitivityX = 0.35f;
    public float mouseSensitivityY = 0.35f;

    public bool mouseInvertedX = false;
    public bool mouseInvertedY = false;

    public LayerMask lockableTarget;

    private InputAction playerMove;
    private InputAction playerLook;
    private InputAction playerJump;
    private InputAction playerAim;
    private InputAction playerSprint;
    private InputAction[] playerWeaponSlot = new InputAction[3];

    private Vector3 lookEuler;

    private Transform lockedTarget;
    private ManController manController;
    private Transform cam;

    private void OnEnable()
    {
        playerActions.FindActionMap("Player").Enable();
    }

    private void OnDisable()
    {
        playerActions.FindActionMap("Player").Disable();
    }

    private void Start()
    {
        playerMove = playerActions.FindAction("Move");
        playerLook = playerActions.FindAction("Look");
        playerJump = playerActions.FindAction("Jump");
        playerAim = playerActions.FindAction("Aim");
        playerSprint = playerActions.FindAction("Sprint");
        playerWeaponSlot[0] = playerActions.FindAction("WeaponSlot0");
        playerWeaponSlot[1] = playerActions.FindAction("WeaponSlot1");
        playerWeaponSlot[2] = playerActions.FindAction("WeaponSlot2");

        manController = man.GetComponent<ManController>();
        cam = GameObject.FindGameObjectWithTag("MainCamera").transform;

        lookEuler = manController.lookEuler;

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (playerAim.IsPressed())
        {
            lockedTarget = GetMostCenteredViewObject();
        }
        if (lockedTarget)
        {
            if (Vector3.Distance(cam.position, lockedTarget.position) > LOCK_ON_DETECTOR_RADIUS)
            {
                lockedTarget = null;
            }
        }
        HandleMotion();
        HandleLook();

        manController.inputJump = playerJump.IsPressed();
        manController.inputSprint = playerSprint.IsPressed();
        manController.inputAim = lockedTarget || playerAim.IsPressed();

        HandleWeaponSlot();
    }

    private void LateUpdate()
    {
        transform.position = man.position + new Vector3(0f, manOffsetY, 0f);
        HandleRotation();
    }

    private void HandleMotion()
    {
        Vector2 move = playerMove.ReadValue<Vector2>();
        Vector3 motion = Quaternion.Euler(lookEuler) * new Vector3(move.x, 0, move.y);
        manController.inputMotion = motion;
    }

    private void HandleLook()
    {
        if (playerAim.IsPressed())
        {
            lookEuler = lookEulerDelta(lookEuler);
        }
        else if (lockedTarget)
        {
            lookEuler = manController.lookEuler;
        }
        else
        {
            lookEuler = lookEulerDelta(lookEuler);
        }

        if (lockedTarget)
        {
            Vector3 euler = Quaternion.LookRotation((lockedTarget.position - transform.position).normalized).eulerAngles;
            manController.lookEuler = new Vector3(WrapAngle(euler.x), WrapAngle(euler.y), 0f);
        }
        else
        {
            manController.lookEuler = lookEuler;
        }
    }

    private void HandleWeaponSlot()
    {
        for (int i = 0; i < playerWeaponSlot.Length; i++)
        {
            if (playerWeaponSlot[i].IsPressed())
            {
                manController.inputWeaponSlot = i;
                return;
            }
        }
        manController.inputWeaponSlot = -1;
    }

    private void HandleRotation()
    {
        Quaternion q2 = Quaternion.Euler(lookEuler);
        transform.rotation = Quaternion.Lerp(transform.rotation, q2, 0.05f);
    }

    private Vector3 lookEulerDelta(Vector3 euler)
    {
        Vector2 look = playerLook.ReadValue<Vector2>();
        float x = euler.x;
        float y = euler.y;
        x += look.y * mouseSensitivityY * (mouseInvertedY ? 1f : -1f);
        x = Mathf.Clamp(x, -86f, 86f);
        y += look.x * mouseSensitivityX * (mouseInvertedX ? -1f : 1f);
        return new Vector3(x, y, 0f);
    }

    private Transform GetMostCenteredViewObject()
    {
        Collider[] resultsBuffer = new Collider[30];

        int numCollidersFound = Physics.OverlapSphereNonAlloc(
            cam.position,
            LOCK_ON_DETECTOR_RADIUS,
            resultsBuffer,
            lockableTarget,
            QueryTriggerInteraction.Ignore
        );

        Transform mostCentered = null;
        float maxDot = Mathf.NegativeInfinity;

        for (int i = 0; i < numCollidersFound; i++)
        {
            if (resultsBuffer[i] == null)
                continue;

            Transform target = resultsBuffer[i].transform;

            float dot = Vector3.Dot(cam.forward, (target.position - cam.position).normalized);
            if (dot > maxDot)
            {
                maxDot = dot;
                mostCentered = target;
            }
        }

        return mostCentered;
    }

    private float WrapAngle(float angle)
    {
        return Mathf.Repeat(angle + 180f, 360f) - 180f;
    }
}
