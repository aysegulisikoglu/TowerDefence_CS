using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class ChaserSpawner : MonoBehaviour
{
    [Header("Spawn Area (Rectangle)")]
    public Transform minPos;
    public Transform maxPos;
    public float spawnY = 0f;

    [Header("Chaser Prefab")]
    public GameObject chaserPrefab;

    [Header("Wave-based extra burst")]
    public int everyNWave = 3;
    public int minCount = 5;
    public int maxCount = 10;
    public float spawnInterval = 0.15f;

    [Header("Periodic Spawn (optional)")]
    public bool usePeriodic = true;
    public float periodSeconds = 3f;

    [Header("Safety / Limits")]
    public int maxActiveChasers = 400;
    public float minPlayerDistance = 12f;

    [Header("Debug/Test")]
    public bool drawGizmos = true;
    public bool spawnOnPlay = false;
    public KeyCode testKey = KeyCode.C;

    Transform _player;

    void Start()
    {
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p) _player = p.transform;

        if (!minPos || !maxPos) Debug.LogWarning("ChaserSpawner: minPos / maxPos atamadın.");
        if (!chaserPrefab) Debug.LogWarning("ChaserSpawner: chaserPrefab atamadın.");

        if (spawnOnPlay)
        {
            int count = Random.Range(minCount, maxCount + 1);
            StartCoroutine(SpawnBurst(count));
        }

        if (usePeriodic) StartCoroutine(PeriodicSpawnLoop());
    }

    IEnumerator PeriodicSpawnLoop()
    {

        yield return new WaitForSeconds(0.25f);

        while (enabled && gameObject.activeInHierarchy)
        {
            int count = Random.Range(minCount, maxCount + 1);
            yield return StartCoroutine(SpawnBurst(count));
            yield return new WaitForSeconds(periodSeconds);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(testKey))
        {
            int count = Random.Range(minCount, maxCount + 1);
            StartCoroutine(SpawnBurst(count));
        }
    }

    void OnEnable() { TD.Spawner.OnWaveChanged += HandleWaveChanged; }
    void OnDisable() { TD.Spawner.OnWaveChanged -= HandleWaveChanged; }

    void HandleWaveChanged(int waveIndex)
    {

        int waveNumber = waveIndex + 1;
        if (everyNWave <= 0) return;
        if (waveNumber % everyNWave != 0) return;

        int count = Random.Range(minCount, maxCount + 1);
        StartCoroutine(SpawnBurst(count));
    }

    IEnumerator SpawnBurst(int count)
    {
        for (int i = 0; i < count; i++)
        {
            TrySpawnOne();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void TrySpawnOne()
    {
        if (!chaserPrefab || !minPos || !maxPos) return;


        int active = Object.FindObjectsByType<ChaserEnemy3D>(FindObjectsSortMode.None).Length;
        if (active >= maxActiveChasers) return;

        Vector3 pos = GetPerimeterSpawn();


        if (_player)
        {
            int tries = 0;
            while ((pos - _player.position).sqrMagnitude < (minPlayerDistance * minPlayerDistance) && tries < 6)
            {
                pos = GetPerimeterSpawn();
                tries++;
            }
        }

        var go = Instantiate(chaserPrefab, pos, Quaternion.identity);
        var ce = go.GetComponent<ChaserEnemy3D>();
        if (ce) ce.groundY = spawnY;
    }

    Vector3 GetPerimeterSpawn()
    {
        Vector3 a = minPos.position;
        Vector3 b = maxPos.position;


        int side = Random.Range(0, 4);
        float x = 0f, z = 0f;
        switch (side)
        {
            case 0: x = Random.Range(a.x, b.x); z = a.z; break;
            case 1: x = Random.Range(a.x, b.x); z = b.z; break;
            case 2: x = a.x; z = Random.Range(a.z, b.z); break;
            default: x = b.x; z = Random.Range(a.z, b.z); break;
        }
        return new Vector3(x, spawnY, z);
    }

    void OnDrawGizmos()
    {
        if (!drawGizmos || !minPos || !maxPos) return;

        Vector3 a = minPos.position; a.y = spawnY;
        Vector3 b = maxPos.position; b.y = spawnY;

        Gizmos.color = Color.yellow;
        Vector3 p1 = new Vector3(a.x, spawnY, a.z);
        Vector3 p2 = new Vector3(b.x, spawnY, a.z);
        Vector3 p3 = new Vector3(b.x, spawnY, b.z);
        Vector3 p4 = new Vector3(a.x, spawnY, b.z);
        Gizmos.DrawLine(p1, p2); Gizmos.DrawLine(p2, p3);
        Gizmos.DrawLine(p3, p4); Gizmos.DrawLine(p4, p1);
    }
}
