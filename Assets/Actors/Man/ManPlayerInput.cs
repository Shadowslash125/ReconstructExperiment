using UnityEngine;
using UnityEngine.InputSystem;

public class ManPlayerInput : MonoBehaviour
{
    public Transform man;
    public InputActionAsset playerActions;

    public float mouseSensitivityX = 0.35f;
    public float mouseSensitivityY = 0.35f;

    public bool mouseInvertedX = false;
    public bool mouseInvertedY = false;

    private InputAction playerMove;
    private InputAction playerLook;
    private InputAction playerJump;
    private InputAction playerAim;
    private InputAction[] playerWeaponSlot = new InputAction[3];

    private ManController manController;

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
        playerWeaponSlot[0] = playerActions.FindAction("WeaponSlot0");
        playerWeaponSlot[1] = playerActions.FindAction("WeaponSlot1");
        playerWeaponSlot[2] = playerActions.FindAction("WeaponSlot2");

        manController = man.GetComponent<ManController>();

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        HandleMotion();
        HandleLook();

        manController.inputJump = playerJump.IsPressed();
        manController.inputAim = playerAim.IsPressed();

        HandleWeaponSlot();
    }

    private void LateUpdate()
    {
        transform.rotation = Quaternion.Euler(manController.lookEuler);
    }

    private void HandleMotion()
    {
        Vector2 move = playerMove.ReadValue<Vector2>();
        Vector3 motion = Quaternion.Euler(manController.lookEuler) * new Vector3(move.x, 0, move.y);
        manController.inputMotion = motion;
    }

    private void HandleLook()
    {
        Vector2 look = playerLook.ReadValue<Vector2>();
        manController.lookEuler.x += look.y * mouseSensitivityY * (mouseInvertedY ? 1f : -1f);
        manController.lookEuler.x = Mathf.Clamp(manController.lookEuler.x, -86f, 86f);
        manController.lookEuler.y += look.x * mouseSensitivityX * (mouseInvertedX ? -1f : 1f);
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
}
