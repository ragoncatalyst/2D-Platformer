using UnityEngine;
using System.Collections.Generic;

public class HideTileTrigger : MonoBehaviour
{
    [Tooltip("拖入需要被隐藏的地块根对象（可多个）。留空则使用本触发器的父物体作为根。我们只会切换这些对象下所有 Renderer 的显示状态。")]
    [SerializeField] GameObject[] tilesToHide;

    readonly List<Renderer> _renderers = new List<Renderer>();

    void Awake()
    {
        _renderers.Clear();
        if (tilesToHide != null && tilesToHide.Length > 0)
        {
            foreach (var root in tilesToHide)
            {
                if (root == null) continue;
                _renderers.AddRange(root.GetComponentsInChildren<Renderer>(includeInactive: true));
            }
        }
        else if (transform.parent != null)
        {
            _renderers.AddRange(transform.parent.GetComponentsInChildren<Renderer>(includeInactive: true));
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        SetVisible(false);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        SetVisible(true);
    }

    void SetVisible(bool visible)
    {
        foreach (var r in _renderers)
        {
            if (r != null) r.enabled = visible;
        }
    }
}