using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Follow player")]
    [SerializeField] private Transform player;
    [SerializeField] private float smoothTime = 0.12f;

    [Header("Wychylenie myszy")]
    [Range(0.1f, 1.0f)]
    public float mouseInfluence = 0.3f;
    public float maxOffset = 5f;

    private Vector3 _currentVelocity = Vector3.zero;
    private Camera _cam;
    private float currentPosX;

    private void Awake()
    {
        _cam = GetComponent<Camera>();
        if (_cam == null)
            _cam = Camera.main;
    }

    private void LateUpdate()
    {
        if (player == null || _cam == null)
            return;

        Vector3 mouseWorld = _cam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = player.position.z;

        Vector3 targetPos = Vector3.Lerp(player.position, mouseWorld, mouseInfluence);

        Vector3 offset = targetPos - player.position;
        if (offset.sqrMagnitude > maxOffset * maxOffset)
            targetPos = player.position + offset.normalized * maxOffset;

        targetPos.z = transform.position.z;
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref _currentVelocity, smoothTime);
    }

    public void MoveToNewRoom(Transform _newRoom)
    {
        currentPosX = _newRoom.position.x;
    }
}
