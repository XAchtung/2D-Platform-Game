using UnityEngine;

public class WorldPromptBillboard : MonoBehaviour
{
    [SerializeField] private float worldSize = 0.45f;

    private void LateUpdate()
    {
        Transform parent = transform.parent;
        if (parent == null)
        {
            transform.rotation = Quaternion.identity;
            transform.localScale = Vector3.one * worldSize;
            return;
        }

        transform.rotation = Quaternion.identity;

        Vector3 lossy = parent.lossyScale;
        float sx = Mathf.Abs(lossy.x) < 0.001f ? 1f : worldSize / Mathf.Abs(lossy.x);
        float sy = Mathf.Abs(lossy.y) < 0.001f ? 1f : worldSize / Mathf.Abs(lossy.y);
        transform.localScale = new Vector3(sx, sy, 1f);
    }
}
