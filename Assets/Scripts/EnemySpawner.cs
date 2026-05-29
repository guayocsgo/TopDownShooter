using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject weakEnemyPrefab;
    [SerializeField] private GameObject strongEnemyPrefab;

    [Header("Puntos de spawn")]
    [SerializeField] private Transform[] spawnPoints;

    [Header("Dificultad")]
    [SerializeField] private float initialSpawnInterval = 3f;
    [SerializeField] private float minimumSpawnInterval = 0.8f;
    [SerializeField] private float difficultyIncreaseInterval = 15f;
    [SerializeField] private float spawnIntervalReduction = 0.3f;

    private float currentSpawnInterval;
    private float difficultyTimer;

    private void Start()
    {
        currentSpawnInterval = initialSpawnInterval;
        StartCoroutine(SpawnLoop());
    }

    private void Update()
    {
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

        // cada 3 spawns aparece uno fuerte, el resto débiles
        GameObject prefab = Random.value < 0.25f ? strongEnemyPrefab : weakEnemyPrefab;

        Instantiate(prefab, spawnPoint.position, Quaternion.identity);
    }

    private void IncreaseDifficulty()
    {
        currentSpawnInterval -= spawnIntervalReduction;
        currentSpawnInterval = Mathf.Max(currentSpawnInterval, minimumSpawnInterval);

        Debug.Log("Dificultad aumentada. Intervalo: " + currentSpawnInterval);
    }
}