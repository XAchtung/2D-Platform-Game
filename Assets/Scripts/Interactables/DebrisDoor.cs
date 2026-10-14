using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DebrisDoor : MonoBehaviour
{
    [Header("Purchase")]
    [SerializeField] private int cost = 750;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private bool destroyOnPurchase = true;

    [Header("Feedback")]
    [SerializeField] private GameObject promptObject;
    [SerializeField] private Vector3 promptOffset = new Vector3(0f, 1.35f, 0f);
    [SerializeField] private AudioClip openSound;
    [SerializeField] private AudioClip denySound;
    [SerializeField] private GameObject openEffect;

    private bool playerInRange;
    private bool opened;
    private Collider2D[] colliders;
    private SpriteRenderer[] sprites;

    private void Awake()
    {
        colliders = GetComponentsInChildren<Collider2D>(true);
        sprites = GetComponentsInChildren<SpriteRenderer>(true);

        string message = $"Naciśnij {interactKey}\nKoszt: {cost}";
        promptObject = InteractPrompt.CreateOrUpdate(transform, promptObject, message, promptOffset);
        if (promptObject != null)
            promptObject.SetActive(false);
    }

    private void Update()
    {
        if (opened || !playerInRange)
            return;

        if (Input.GetKeyDown(interactKey))
            TryOpen();
    }

    private void TryOpen()
    {
        if (PointsManager.instance == null)
            return;

        if (!PointsManager.instance.SpendPoints(cost))
        {
            if (SoundManager.instance != null && denySound != null)
                SoundManager.instance.PlaySound(denySound);
            return;
        }

        opened = true;

        if (SoundManager.instance != null && openSound != null)
            SoundManager.instance.PlaySound(openSound);

        if (openEffect != null)
            Instantiate(openEffect, transform.position, Quaternion.identity);

        if (destroyOnPurchase)
        {
            Destroy(gameObject);
            return;
        }

        if (promptObject != null)
            promptObject.SetActive(false);

        if (colliders != null)
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i] != null)
                    colliders[i].enabled = false;
            }
        }

        if (sprites != null)
        {
            for (int i = 0; i < sprites.Length; i++)
            {
                if (sprites[i] != null)
                    sprites[i].enabled = false;
            }
        }

        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (opened || !other.CompareTag("Player"))
            return;

        playerInRange = true;
        if (promptObject != null)
            promptObject.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInRange = false;
        if (promptObject != null)
            promptObject.SetActive(false);
    }
}
