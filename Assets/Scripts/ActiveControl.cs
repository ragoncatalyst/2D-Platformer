using UnityEngine;

// Global flag to decide which actor is currently player-controlled.
// Keep it simple and static so any script can query quickly.
[AddComponentMenu("Game/Active Control (Singleton)")]
public class ActiveControl : MonoBehaviour
{
    public enum Actor
    {
        Player,
        Drone
    }

    static ActiveControl _instance;
    public static ActiveControl Instance
    {
        get
        {
            if (_instance == null)
            {
                var go = new GameObject("ActiveControl");
                _instance = go.AddComponent<ActiveControl>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    Actor _current = Actor.Player;
    public Actor Current
    {
        get => _current;
        set
        {
            if (_current == value) return;
            _current = value;
            if (OnChanged != null) OnChanged(_current);
        }
    }

    // Simple change event so movers can react (e.g., zero velocity when deactivated)
    public delegate void ActiveChanged(Actor active);
    public event ActiveChanged OnChanged;

    // Ensure the singleton survives scene loads if manually dropped in a scene
    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
