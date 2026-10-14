using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;

    [Header("Prefabs & Spawn")]
    [SerializeField] private GameObject zombiePrefab;
    [SerializeField] private Transform[] spawnPoints;

    [Header("Round Settings")]
    [SerializeField] private int startingRound = 1;
    [SerializeField] private int baseZombieCount = 6;
    [SerializeField] private int zombiesAddedPerRound = 3;
    [SerializeField] private float spawnInterval = 1.1f;
    [SerializeField] private float intermissionDuration = 8f;
    [SerializeField] private int roundClearBonus = 400;
    [SerializeField] private int bonusIncreasePerRound = 100;

    [Header("Zombie Scaling")]
    [SerializeField] private float baseZombieHealth = 100f;
    [SerializeField] private float healthIncreasePerRound = 0.2f;
    [SerializeField] private float baseMoveSpeed = 2.4f;
    [SerializeField] private float speedIncreasePerRound = 0.08f;
    [SerializeField] private float maxMoveSpeed = 5f;
    [SerializeField] private float baseAttackDamage = 10f;
    [SerializeField] private float damageIncreasePerRound = 1.5f;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI roundText;
    [SerializeField] private TextMeshProUGUI zombiesRemainingText;
    [SerializeField] private TextMeshProUGUI intermissionText;

    [Header("Events & Audio")]
    [SerializeField] private AudioClip roundStartSound;
    [SerializeField] private AudioClip roundEndSound;
    [SerializeField] private AudioClip countdownTickSound;
    public UnityEvent<int> OnRoundStarted;
    public UnityEvent<int> OnRoundCleared;
    public UnityEvent OnIntermissionStarted;

    public int CurrentRound { get; private set; }
    public int AliveZombies { get; private set; }

    private bool roundActive;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        CurrentRound = Mathf.Max(1, startingRound - 1);
    }

    private void Start()
    {
        if (zombiePrefab == null || spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("WaveManager: ustaw zombiePrefab oraz spawnPoints w Inspektorze.");
            enabled = false;
            return;
        }

        StartCoroutine(RoundLoop());
    }

    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    private IEnumerator RoundLoop()
    {
        while (true)
        {
            yield return StartCoroutine(RunIntermission());
            yield return StartCoroutine(RunRound());
        }
    }

    private IEnumerator RunIntermission()
    {
        roundActive = false;
        OnIntermissionStarted?.Invoke();

        float remaining = intermissionDuration;
        int lastAnnouncedSecond = -1;
        while (remaining > 0f)
        {
            int secondsLeft = Mathf.CeilToInt(remaining);
            if (intermissionText != null)
                intermissionText.text = CurrentRound == 0
                    ? $"Round starts in {secondsLeft}"
                    : $"Next round in {secondsLeft}";

            if (roundText != null && CurrentRound > 0)
                roundText.text = $"Round {CurrentRound} cleared";

            if (secondsLeft != lastAnnouncedSecond)
            {
                lastAnnouncedSecond = secondsLeft;
                if (SoundManager.instance != null && countdownTickSound != null)
                    SoundManager.instance.PlaySound(countdownTickSound);
            }

            remaining -= Time.deltaTime;
            yield return null;
        }

        if (intermissionText != null)
            intermissionText.text = string.Empty;
    }

    private IEnumerator RunRound()
    {
        CurrentRound++;
        roundActive = true;

        int zombiesThisRound = Mathf.Max(1, baseZombieCount + (CurrentRound - 1) * zombiesAddedPerRound);
        AliveZombies = 0;

        UpdateUI();
        OnRoundStarted?.Invoke(CurrentRound);

        if (SoundManager.instance != null && roundStartSound != null)
            SoundManager.instance.PlaySound(roundStartSound);

        for (int i = 0; i < zombiesThisRound; i++)
        {
            SpawnZombie();
            yield return new WaitForSeconds(spawnInterval);
        }

        yield return new WaitUntil(() => AliveZombies <= 0);

        roundActive = false;
        OnRoundCleared?.Invoke(CurrentRound);

        if (SoundManager.instance != null && roundEndSound != null)
            SoundManager.instance.PlaySound(roundEndSound);

        int bonus = roundClearBonus + (CurrentRound - 1) * bonusIncreasePerRound;
        if (PointsManager.instance != null && bonus > 0)
            PointsManager.instance.AddPoints(bonus);

        if (roundText != null)
            roundText.text = $"Round {CurrentRound} cleared  +{bonus}";
    }

    private void SpawnZombie()
    {
        Transform point = GetValidSpawnPoint();
        if (zombiePrefab == null || point == null)
            return;

        GameObject zombie = Instantiate(zombiePrefab, point.position, Quaternion.identity);
        if (zombie == null)
            return;

        AliveZombies++;

        float health = baseZombieHealth * (1f + healthIncreasePerRound * (CurrentRound - 1));
        float speed = Mathf.Min(maxMoveSpeed, baseMoveSpeed + speedIncreasePerRound * (CurrentRound - 1));
        float damage = baseAttackDamage + damageIncreasePerRound * (CurrentRound - 1);

        ZombieAI ai = zombie.GetComponent<ZombieAI>();
        if (ai == null)
            ai = zombie.GetComponentInChildren<ZombieAI>();

        if (ai != null)
            ai.SetupZombie(health, speed, damage);
        else
        {
            Health healthComponent = zombie.GetComponent<Health>();
            if (healthComponent != null)
                healthComponent.SetMaxHealth(health);
        }

        UpdateUI();
    }

    private Transform GetValidSpawnPoint()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
            return null;

        for (int attempt = 0; attempt < spawnPoints.Length; attempt++)
        {
            Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];
            if (point != null)
                return point;
        }

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (spawnPoints[i] != null)
                return spawnPoints[i];
        }

        return null;
    }

    public void EnemyKilled()
    {
        if (AliveZombies <= 0)
            return;

        AliveZombies--;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (roundText != null && roundActive)
            roundText.text = $"Round {CurrentRound}";

        if (zombiesRemainingText != null)
            zombiesRemainingText.text = $"Zombies: {Mathf.Max(0, AliveZombies)}";
    }
}
