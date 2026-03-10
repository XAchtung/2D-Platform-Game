using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public string waveName;
        public GameObject enemyPrefab;
        public int count;
        public float rate; // Odstêp miêdzy spawnami (w sekundach)
    }

    public List<Wave> waves; // Lista fal definiowana w Inspektorze
    public Transform[] spawnPoints; // Punkty, w których pojawi¹ siê wrogowie

    private int currentWaveIndex = 0;
    private bool isSpawning = false;

    void Update()
    {
        // Jeœli nie spawnujemy i naciœniesz np. "Enter" (albo automatycznie po zabiciu wszystkich)
        if (!isSpawning && Input.GetKeyDown(KeyCode.Return))
        {
            if (currentWaveIndex < waves.Count)
            {
                StartCoroutine(SpawnWave(waves[currentWaveIndex]));
                currentWaveIndex++;
            }
            else
            {
                Debug.Log("Wszystkie fale ukoñczone!");
            }
        }
    }

    IEnumerator SpawnWave(Wave _wave)
    {
        isSpawning = true;
        Debug.Log("Start fali: " + _wave.waveName);

        for (int i = 0; i < _wave.count; i++)
        {
            SpawnEnemy(_wave.enemyPrefab);
            yield return new WaitForSeconds(_wave.rate); // Czekaj przed kolejnym wrogiem
        }

        isSpawning = false;
    }

    void SpawnEnemy(GameObject _enemy)
    {
        // Wybierz losowy punkt spawnu z Twojej listy
        Transform _sp = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Instantiate(_enemy, _sp.position, _sp.rotation);
    }
}