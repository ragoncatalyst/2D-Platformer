using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
[AddComponentMenu("Game/Drone Initializer")]
public class DroneInitializer : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Player transform. If not set, will try to find by tag 'Player' or PlayerMovement component.")]
    [SerializeField] Transform player;

    [Header("Placement")]
    [SerializeField] float yOffset = 5f;
    [Tooltip("Match player's X on spawn.")]
    [SerializeField] bool matchX = true;
    [Tooltip("Match player's Z on spawn (usually off for 2D).")]
    [SerializeField] bool matchZToPlayer = false;

    void Awake()
    {
        TryAssignPlayer();
    }

    void Start()
    {
        // If player not ready yet, wait briefly.
        if (player == null)
        {
            StartCoroutine(WaitAndPosition());
        }
        else
        {
            PlaceDrone();
        }
    }

    void TryAssignPlayer()
    {
        if (player != null) return;
        var tagged = GameObject.FindWithTag("Player");
        if (tagged) player = tagged.transform;
        if (player == null)
        {
            var pm = FindObjectOfType<PlayerMovement>(true);
            if (pm) player = pm.transform;
        }
    }

    IEnumerator WaitAndPosition()
    {
        float timeout = 1f; // seconds
        float t = 0f;
        while (player == null && t < timeout)
        {
            TryAssignPlayer();
            if (player) break;
            t += Time.unscaledDeltaTime;
            yield return null;
        }
        if (player)
        {
            PlaceDrone();
        }
        else
        {
            Debug.LogWarning("DroneInitializer: Player not found; cannot position drone.");
        }
    }

    void PlaceDrone()
    {
        if (player == null) return;
        Vector3 pos = transform.position;
        Vector3 p = player.position;
        if (matchX) pos.x = p.x;
        pos.y = p.y + yOffset;
        if (matchZToPlayer) pos.z = p.z;
        transform.position = pos;
    }
}
