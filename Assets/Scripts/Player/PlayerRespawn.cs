using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [SerializeField]private AudioClip checkpointSound;
    private Transform currentCheckpoint;
    private Health playerHealth;
    private UIManager uiManager;

    private void Awake()
    {
        playerHealth = GetComponent<Health>();
        uiManager = FindFirstObjectByType<UIManager>();
    }

    public void CheckRespawn()
    {
        if(currentCheckpoint == null)
        {

            uiManager.GameOver();
            return;
        }

        transform.position = currentCheckpoint.position;
        playerHealth.Respawn();

        //move camera to checpoint room
        Camera.main.GetComponent<CameraController>().MoveToNewRoom(currentCheckpoint.parent);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Checkpoint"))
        {
            currentCheckpoint = collision.transform; //store the checkpoint that got activated as the current one
            SoundManager.instance.PlaySound(checkpointSound);
            collision.GetComponent<Collider2D>().enabled = false; //deactivate checkpoint collider
            collision.GetComponent<Animator>().SetTrigger("appear");
        }
    }
}
