using UnityEngine;
using System.Collections;

public class PickupSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject heartPickupPrefab;
    [SerializeField] private GameObject speedPickupPrefab;

    [Header("Puntos de spawn")]
    [SerializeField] private Transform[] spawnPoints;

    [Header("Tiempos")]
    [SerializeField] private float heartSpawnInterval = 10f;
    [SerializeField] private float speedSpawnInterval = 15f;

    private void Start()
    {
        StartCoroutine(SpawnHeart());
        StartCoroutine(SpawnSpeed());
    }

    private IEnumerator SpawnHeart()
    {
        while (true)
        {
            yield return new WaitForSeconds(heartSpawnInterval);

            if (heartPickupPrefab != null && spawnPoints.Length > 0)
            {
                Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];
                Instantiate(heartPickupPrefab, point.position, Quaternion.identity);
            }
        }
    }

    private IEnumerator SpawnSpeed()
    {
        while (true)
        {
            yield return new WaitForSeconds(speedSpawnInterval);

            if (speedPickupPrefab != null && spawnPoints.Length > 0)
            {
                Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];
                Instantiate(speedPickupPrefab, point.position, Quaternion.identity);
            }
        }
    }
}