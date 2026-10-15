using UnityEngine;
using UnityEngine.Events;

public class RynAnimBridge : MonoBehaviour
{
    public UnityEvent eventLanceSlashLR;

    private void LanceSlashLR()
    {
        eventLanceSlashLR?.Invoke();
    }
}
