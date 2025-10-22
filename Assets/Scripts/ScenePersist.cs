using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenePersist : MonoBehaviour
{
    int startingSceneIndex = -1;

    HashSet<string> collectedObjectIDs = new HashSet<string>();

    Dictionary<string, string> objectStates = new Dictionary<string, string>();

    public static ScenePersist Instance { get; private set; }

    void Awake()
    {
        int numScenePersists = FindObjectsOfType<ScenePersist>().Length;
        if (numScenePersists > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
            startingSceneIndex = SceneManager.GetActiveScene().buildIndex;
            // 监听场景加载，以便在进入新关卡时销毁自己
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
    }

    void OnDestroy()
    {
        // 注销事件，避免内存/引用泄漏
        SceneManager.sceneLoaded -= OnSceneLoaded;
        if (Instance == this) Instance = null;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 如果玩家进入了新的关卡（buildIndex 不同），说明应该清除当前持久化数据
        // 以便下次重新进入此关卡可以执行初始化
        if (scene.buildIndex != startingSceneIndex)
        {
            Destroy(gameObject);
        }
    }

    // API: 记录某个对象已被收集（例如金币）
    public void RecordCollected(string objectID)
    {
        if (string.IsNullOrEmpty(objectID)) return;
        if (!collectedObjectIDs.Contains(objectID))
        {
            collectedObjectIDs.Add(objectID);
        }
    }

    public bool WasCollected(string objectID)
    {
        if (string.IsNullOrEmpty(objectID)) return false;
        return collectedObjectIDs.Contains(objectID);
    }

    public void SaveObjectState(string objectID, string serializedState)
    {
        if (string.IsNullOrEmpty(objectID)) return;
        objectStates[objectID] = serializedState;
    }

    // API: 获取保存的对象状态（若存在）
    public bool TryGetObjectState(string objectID, out string serializedState)
    {
        if (string.IsNullOrEmpty(objectID))
        {
            serializedState = null;
            return false;
        }
        return objectStates.TryGetValue(objectID, out serializedState);
    }

    public void ClearAllPersistedData()
    {
        collectedObjectIDs.Clear();
        objectStates.Clear();
    }

    // Called by GameSession when the entire game session is reset (e.g. player lost all lives)
    // This will clear any persisted scene data and destroy this object so the next play
    // through will start with a fresh state.
    public void ResetScenePersists()
    {
        ClearAllPersistedData();
        // Destroy this persist so it won't carry data into a new game/session
        Destroy(gameObject);
    }
}
