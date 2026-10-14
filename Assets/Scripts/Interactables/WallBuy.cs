using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class WallBuy : MonoBehaviour
{
    [Header("Purchase")]
    [SerializeField] private int cost = 1200;
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    [Header("Weapon")]
    [SerializeField] private GameObject weaponPrefab;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Sprite weaponSprite;
    [SerializeField] private float weaponDamage = 35f;
    [SerializeField] private float weaponFireRate = 0.15f;
    [SerializeField] private AudioClip weaponShootSound;

    [Header("Feedback")]
    [SerializeField] private GameObject promptObject;
    [SerializeField] private Vector3 promptOffset = new Vector3(0f, 1.35f, 0f);
    [SerializeField] private AudioClip buySound;
    [SerializeField] private AudioClip denySound;

    private bool playerInRange;
    private Transform playerInTrigger;

    private void Reset()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.isTrigger = true;
    }

    private void Awake()
    {
        string message = $"Naciśnij {interactKey}\nKoszt: {cost}";
        promptObject = InteractPrompt.CreateOrUpdate(transform, promptObject, message, promptOffset);
        if (promptObject != null)
            promptObject.SetActive(false);
    }

    private void Update()
    {
        if (!playerInRange || playerInTrigger == null)
            return;

        if (!Input.GetKeyDown(interactKey))
            return;

        TryPurchase(playerInTrigger);
    }

    private void TryPurchase(Transform playerTransform)
    {
        if (PointsManager.instance == null)
            return;

        if (!PointsManager.instance.SpendPoints(cost))
        {
            if (SoundManager.instance != null && denySound != null)
                SoundManager.instance.PlaySound(denySound);
            return;
        }

        PlayerAimWeapon aim = playerTransform.GetComponent<PlayerAimWeapon>();
        if (aim == null)
            aim = playerTransform.GetComponentInChildren<PlayerAimWeapon>();
        if (aim == null)
            aim = playerTransform.GetComponentInParent<PlayerAimWeapon>();

        if (aim == null)
        {
            PointsManager.instance.AddPoints(cost);
            Debug.LogWarning("WallBuy: nie znaleziono PlayerAimWeapon na graczu.");
            return;
        }

        Sprite spriteToUse = weaponSprite;
        if (spriteToUse == null && weaponPrefab != null)
        {
            SpriteRenderer sr = weaponPrefab.GetComponentInChildren<SpriteRenderer>();
            if (sr != null)
                spriteToUse = sr.sprite;
        }

        aim.EquipWeapon(projectilePrefab, spriteToUse, weaponDamage, weaponFireRate, weaponShootSound);

        if (SoundManager.instance != null && buySound != null)
            SoundManager.instance.PlaySound(buySound);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInRange = true;
        playerInTrigger = other.transform;
        if (promptObject != null)
            promptObject.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInRange = false;
        playerInTrigger = null;
        if (promptObject != null)
            promptObject.SetActive(false);
    }
}
