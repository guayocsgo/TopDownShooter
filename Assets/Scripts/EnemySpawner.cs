using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject weakEnemyPrefab;
    [SerializeField] private GameObject strongEnemyPrefab;
    [SerializeField] private GameObject flyingEnemyPrefab;
    [SerializeField] private GameObject fastEnemyPrefab;

    [Header("Puntos de spawn")]
    [SerializeField] private Transform[] spawnPoints;

    [Header("Dificultad")]
    [SerializeField] private float initialSpawnInterval = 3f;
    [SerializeField] private float minimumSpawnInterval = 0.8f;
    [SerializeField] private float difficultyIncreaseInterval = 15f;
    [SerializeField] private float spawnIntervalReduction = 0.3f;

    [Header("Delays de aparicion")]
    [SerializeField] private float flyingEnemyDelay = 30f;
    [SerializeField] private float fastEnemyDelay = 20f;

    private float currentSpawnInterval;
    private float difficultyTimer;
    private float elapsedTime;

    private void Start()
    {
        currentSpawnInterval = initialSpawnInterval;
        StartCoroutine(SpawnLoop());
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;
        difficultyTimer += Time.deltaTime;

        if (difficultyTimer >= difficultyIncreaseInterval)
        {
            difficultyTimer = 0f;
            IncreaseDifficulty();
        }
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(currentSpawnInterval);
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        if (spawnPoints.Length == 0) return;

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject prefab = ChooseEnemy();

        if (prefab != null)
            Instantiate(prefab, spawnPoint.position, Quaternion.identity);
    }

    private GameObject ChooseEnemy()
    {
       
        float roll = Random.value;

        bool flyingAvailable = elapsedTime >= flyingEnemyDelay && flyingEnemyPrefab != null;
        bool fastAvailable = elapsedTime >= fastEnemyDelay && fastEnemyPrefab != null;

        if (flyingAvailable && fastAvailable)
        {
            
            if (roll < 0.15f) return flyingEnemyPrefab;
            if (roll < 0.30f) return fastEnemyPrefab;
            if (roll < 0.55f) return strongEnemyPrefab;
            return weakEnemyPrefab;
        }
        else if (fastAvailable)
        {
            if (roll < 0.20f) return fastEnemyPrefab;
            if (roll < 0.45f) return strongEnemyPrefab;
            return weakEnemyPrefab;
        }
        else if (flyingAvailable)
        {
            if (roll < 0.20f) return flyingEnemyPrefab;
            if (roll < 0.45f) return strongEnemyPrefab;
            return weakEnemyPrefab;
        }
        else
        {
            
            if (roll < 0.25f) return strongEnemyPrefab;
            return weakEnemyPrefab;
        }
    }

    private void IncreaseDifficulty()
    {
        currentSpawnInterval -= spawnIntervalReduction;
        currentSpawnInterval = Mathf.Max(currentSpawnInterval, minimumSpawnInterval);
        Debug.Log("Dificultad aumentada. Intervalo: " + currentSpawnInterval);
    }
}