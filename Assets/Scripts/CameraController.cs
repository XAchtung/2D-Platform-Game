using UnityEngine;

public class CameraController : MonoBehaviour
{
    //Room camera
    [SerializeField] private float speed;
    private float currentPosX;
    private Vector3 velocity = Vector3.zero;

    //Follow player
    [SerializeField]private Transform player;
    [SerializeField]private float aheadDistance;
    [SerializeField]private float cameraSpeed;
    private float lookAhead;


    //Mouse camera-look-ahead
    public float smoothTime = 0.12f;


    private void Update()
    {

        transform.rotation = Quaternion.Euler(0, 0, Mathf.Sin(Time.time) * 0.5f);


        //Room camera
        //transform.position = Vector3.SmoothDamp(transform.position, new Vector3(currentPosX, transform.position.y, transform.position.z), ref velocity, speed);

        //Follow player
        //transform.position = new Vector3(player.position.x + lookAhead, transform.position.y, transform.position.z);
        //lookAhead = Mathf.Lerp(lookAhead, (aheadDistance * player.localScale.x), Time.deltaTime*cameraSpeed);
    }


    [Header("Wychylenie myszy")]
    [Range(0.1f, 1.0f)]
    public float mouseInfluence = 0.3f; 
    public float maxOffset = 5f;        

    private Vector3 _currentVelocity = Vector3.zero;

    void LateUpdate()
    {
        if (player == null) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        Vector3 targetPos = Vector3.Lerp(player.position, mousePos, mouseInfluence);
        float distance = Vector3.Distance(player.position, targetPos);

        if (distance > maxOffset)
        {
            Vector3 direction = (targetPos - player.position).normalized;
            targetPos = player.position + (direction * maxOffset);
        }

        targetPos.z = -10f;
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref _currentVelocity, smoothTime);
        
    }


    public void MoveToNewRoom(Transform _newRoom)
    {
        currentPosX = _newRoom.position.x;
    }
}
