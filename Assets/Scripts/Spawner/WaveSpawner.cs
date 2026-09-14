using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner instance; // U³atwia wrogom zg³oszenie œmierci

    [Header("UI Reference")]
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI enemiesRemainingText; // Opcjonalny licznik wrogów

    [Header("Prefabs & Spawn Points")]
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private Transform[] spawnPoints;

    [Header("Wave Settings")]
    [SerializeField] private float timeBetweenWaves = 5f; // Czas wytchnienia PO zabiciu wszystkich
    [SerializeField] private float spawnDelay = 0.5f;     // Odstêp miêdzy pojawianiem siê kolejnych wrogów
    [SerializeField] private int initialEnemyCount = 5;
    [SerializeField] private int enemiesIncreasePerWave = 3;

    private int currentWave = 0;
    private int totalEnemiesThisWave;
    private int aliveEnemiesCount = 0;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        StartCoroutine(CoDWaveLoop());
    }

    private IEnumerator CoDWaveLoop()
    {
        while (true)
        {
            currentWave++;
            totalEnemiesThisWave = initialEnemyCount + (currentWave - 1) * enemiesIncreasePerWave;
            aliveEnemiesCount = totalEnemiesThisWave;

            UpdateUI();

            // 1. Spawnowanie wszystkich potworów zaplanowanych na tê falê
            yield return StartCoroutine(SpawnAllEnemiesInWave(totalEnemiesThisWave));

            // 2. COD ZOMBIES CORE: Czekamy, a¿ gracz zabije OSTATNIEGO wroga
            yield return new WaitUntil(() => aliveEnemiesCount <= 0);

            // 3. Krótka przerwa miêdzy rundami po wyczyszczeniu planszy
            if (waveText != null)
                waveText.text = "Wave Cleared!";

            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }

    private IEnumerator SpawnAllEnemiesInWave(int count)
    {
        for (int i = 0; i < count; i++)
        {
            SpawnSingleEnemy();
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    private void SpawnSingleEnemy()
    {
        if (enemyPrefabs.Length == 0 || spawnPoints.Length == 0) return;

        Transform randomPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject randomPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

        Instantiate(randomPrefab, randomPoint.position, Quaternion.identity);
    }

    // Wywo³ywane przez skrypt wroga w momencie œmierci
    public void EnemyKilled()
    {
        aliveEnemiesCount--;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (waveText != null)
            waveText.text = $"Wave {currentWave}";

        if (enemiesRemainingText != null)
            enemiesRemainingText.text = $"Enemies: {Mathf.Max(0, aliveEnemiesCount)}";
    }
}