using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class EnemySpawnInfo
    {
        public GameObject enemyPrefab;
        public int unlockDay; 
    }

    public List<EnemySpawnInfo> enemyTypes = new List<EnemySpawnInfo>();

    public Transform player;
    public LayerMask groundMask;
    public TimeManager timeManager;
    public int maxActiveEnemies = 5;
    public float spawnRadius = 30f;
    public float minDistanceFromPlayer = 10f;
    public float minDistanceBetweenEnemies = 2.0f;
    public float spawnRadiusReductionPerDay = 1.5f; 
    public float minSpawnDelay = 1f;
    public float maxSpawnDelay = 5f;

    private List<GameObject> activeEnemies = new List<GameObject>();

    private void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    public void NotifyEnemyDeath(GameObject deadEnemy)
    {
        activeEnemies.Remove(deadEnemy);
        Debug.Log($"Enemy died; {activeEnemies.Count}/{maxActiveEnemies} remaining.");
    }

    private IEnumerator SpawnEnemies()
    {
        while (true)
        {
            activeEnemies.RemoveAll(e => e == null);

            bool isNight = timeManager.CurrentTimeOfDay == TimeManager.TimeOfDay.Night;
            int todayMax = Mathf.Clamp(timeManager.Days, 1, 10);

            if (isNight && activeEnemies.Count < todayMax)
            {
                Vector3 spawnPos = GetValidSpawnPositionNearPlayer();
                if (spawnPos != Vector3.zero)
                {
                    GameObject prefab = GetRandomEnemyBasedOnDay();
                    if (prefab != null)
                    {
                        GameObject newEnemy = Instantiate(prefab, spawnPos, Quaternion.identity);
                        var eComp = newEnemy.GetComponent<Enemy>();
                        if (eComp != null) eComp.spawner = this;

                        activeEnemies.Add(newEnemy);
                        Debug.Log($"Spawned {prefab.name}. {activeEnemies.Count}/{todayMax} allowed today.");
                    }
                }
            }

            float delay = (activeEnemies.Count >= todayMax)
                ? 1f
                : Random.Range(minSpawnDelay, maxSpawnDelay);

            yield return new WaitForSeconds(delay);
        }
    }


    private GameObject GetRandomEnemyBasedOnDay()
    {
        List<GameObject> availableEnemies = new List<GameObject>();
        int currentDay = timeManager.Days;
        float currentHour = timeManager.CurrentHour;

        foreach (var enemyInfo in enemyTypes)
        {
            if (enemyInfo.enemyPrefab.CompareTag("Zombie") && currentDay == 1 && currentHour < 6f)
            {
                continue;
            }

            if (currentDay >= enemyInfo.unlockDay)
            {
                availableEnemies.Add(enemyInfo.enemyPrefab);
            }
        }

        if (availableEnemies.Count == 0)
            return null;

        int randomIndex = Random.Range(0, availableEnemies.Count);
        Debug.Log("Spawning enemy: " + availableEnemies[randomIndex].name + " on Day " + currentDay);

        return availableEnemies[randomIndex];
    }




    private Vector3 GetValidSpawnPositionNearPlayer()
    {
        const int maxAttempts = 30;

        for (int i = 0; i < maxAttempts; i++)
        {
            float adjustedSpawnRadius = Mathf.Max(minDistanceFromPlayer + 2f, spawnRadius - (timeManager.Days * spawnRadiusReductionPerDay));
            float finalRadius = Mathf.Clamp(adjustedSpawnRadius, minDistanceFromPlayer + 2f, spawnRadius);

            Vector2 offset2D = Random.insideUnitCircle.normalized * Random.Range(minDistanceFromPlayer, finalRadius);

            Vector3 randomPos = player.position + new Vector3(offset2D.x, 0, offset2D.y);

            if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, 2.0f, NavMesh.AllAreas))
            {
                if (IsFarEnoughFromOtherEnemies(hit.position))
                    return hit.position;
            }
        }

        return Vector3.zero; 
    }

    private bool IsFarEnoughFromOtherEnemies(Vector3 pos)
    {
        foreach (var enemy in activeEnemies)
        {
            if (enemy != null && Vector3.Distance(pos, enemy.transform.position) < minDistanceBetweenEnemies)
                return false;
        }
        return true;
    }
}