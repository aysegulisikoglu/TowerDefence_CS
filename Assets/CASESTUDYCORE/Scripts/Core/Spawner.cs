using System;
using System.Collections.Generic;
using UnityEngine;

namespace TD
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] int maxActiveEnemies = 1200;
        public static Spawner Instance { get; private set; }
        public static event Action<int> OnWaveChanged;
        public static event Action OnMissionComplete;

        [SerializeField] private WaveData[] waves;
        private int _currentWaveIndex = 0;
        private int _waveCounter = 0;
        private WaveData CurrentWave => waves[_currentWaveIndex];

        private float _spawnTimer;
        private float _spawnCounter;
        private int _enemiesRemoved;

        [SerializeField] private ObjectPooler orcPool;
        [SerializeField] private ObjectPooler dragonPool;
        [SerializeField] private ObjectPooler kaijuPool;

        private Dictionary<EnemyType, ObjectPooler> _poolDictionary;

        [SerializeField] private float _timeBetweenWaves = 0f;
        private float _waveCooldown;
        private bool _isBetweenWaves = false;
        private bool _isEndlessMode = false;

        private bool _isCurrentBoss = false;

        void Awake()
        {
            _poolDictionary = new Dictionary<EnemyType, ObjectPooler>()
            {
                { EnemyType.Orc, orcPool },
                { EnemyType.Dragon, dragonPool },
                { EnemyType.Kaiju, kaijuPool },
            };

            if (Instance != null && Instance != this) Destroy(gameObject);
            else Instance = this;
        }

        void OnEnable()
        {
            Enemy.OnEnemyReachedEnd += HandleEnemyReachedEnd;
            Enemy.OnEnemyDestroyed += HandleEnemyDestroyed;
        }
        void OnDisable()
        {
            Enemy.OnEnemyReachedEnd -= HandleEnemyReachedEnd;
            Enemy.OnEnemyDestroyed -= HandleEnemyDestroyed;
        }

        void Start()
        {
            OnWaveChanged?.Invoke(_waveCounter);
            EnableEndlessMode();
        }

        private int GetWavesToWin()
        {
            if (LevelManager.Instance != null && LevelManager.Instance.CurrentLevel != null)
                return LevelManager.Instance.CurrentLevel.wavesToWin;
            return 15;
        }

        public void ForceNextWave()
        {
            if (_isBetweenWaves)
            {
                _waveCooldown = 0f;
                if (AudioController.I && AudioController.I.waveCall)
                    AudioController.I.PlayUI(AudioController.I.waveCall, 1f);
            }
        }

        void Update()
        {
            if (Time.timeScale <= 0f) return;

            if (Input.GetKeyDown(KeyCode.N))
            {
                if (_isBetweenWaves) ForceNextWave();
                else ForceAdvanceWave();
            }

            if (_isBetweenWaves)
            {
                _waveCooldown -= Time.deltaTime;
                if (_waveCooldown <= 0f)
                {
                    if (_waveCounter + 1 >= GetWavesToWin() && !_isEndlessMode)
                    {
                        OnMissionComplete?.Invoke();
                        return;
                    }

                    _currentWaveIndex = (_currentWaveIndex + 1) % waves.Length;
                    _waveCounter++;
                    OnWaveChanged?.Invoke(_waveCounter);

                    int shownWaveNumber = _waveCounter + 1;
                    _isCurrentBoss = (shownWaveNumber == 5 || shownWaveNumber == 10 || shownWaveNumber == 15);

                    _spawnCounter = 0;
                    _enemiesRemoved = 0;
                    _spawnTimer = 0f;
                    _isBetweenWaves = false;


                    if (AudioController.I && AudioController.I.waveStart)
                        AudioController.I.Play2D(AudioController.I.waveStart, 1f, 0f);
                }
            }
            else
            {
                _spawnTimer -= Time.deltaTime;
                if (_spawnTimer <= 0 && _spawnCounter < CurrentWave.enemiesPerWave)
                {
                    _spawnTimer = CurrentWave.spawnInterval;
                    SpawnEnemy();
                    _spawnCounter++;
                }
                else if (_spawnCounter >= CurrentWave.enemiesPerWave && _enemiesRemoved >= CurrentWave.enemiesPerWave)
                {
                    _isBetweenWaves = true;
                    _waveCooldown = _timeBetweenWaves;
                }
            }
        }
        void ForceAdvanceWave()
        {

            _spawnCounter = CurrentWave.enemiesPerWave;
            _enemiesRemoved = CurrentWave.enemiesPerWave;
            _isBetweenWaves = true;
            _waveCooldown = 0f;


        }

        private void SpawnEnemy()
        {
            if (UnityEngine.Object.FindObjectsByType<Enemy>(FindObjectsSortMode.None).Length >= maxActiveEnemies)
                return;
            if (_poolDictionary.TryGetValue(CurrentWave.enemyType, out var pool))
            {
                if (_isCurrentBoss) pool = kaijuPool;

                GameObject spawnedObject = pool.GetPooledObject();
                spawnedObject.transform.position = transform.position;

                float baseHealthMul = 1f + (_waveCounter * 0.4f);
                float bossMul = _isCurrentBoss ? 2.0f : 1.0f;

                Color tint = Color.white;
                float speedMul = 1f;
                float extraHpMul = 1f;

                int shown = _waveCounter + 1;
                switch (shown % 5)
                {
                    case 1: tint = Color.white; speedMul = 1f; extraHpMul = 1f; break;
                    case 2: tint = Color.green; speedMul = 1.25f; extraHpMul = 1f; break;
                    case 3: tint = Color.blue; speedMul = 1f; extraHpMul = 1.35f; break;
                    case 4: tint = Color.red; speedMul = 1.1f; extraHpMul = 1.1f; break;
                    case 0: tint = new Color(0.6f, 0.2f, 0.8f); speedMul = 1.1f; extraHpMul = 1.6f; break;
                }
                if (_isCurrentBoss) tint = new Color(0.6f, 0.2f, 0.8f);

                Enemy enemy = spawnedObject.GetComponent<Enemy>();
                enemy.Initialize(baseHealthMul * bossMul * extraHpMul);
                enemy.SetSpeedMultiplier(speedMul);

                var sr = spawnedObject.GetComponentInChildren<SpriteRenderer>(true);
                if (sr) sr.color = tint;

                spawnedObject.SetActive(true);
            }
        }

        void HandleEnemyReachedEnd(TD.EnemyData data) { _enemiesRemoved++; }
        void HandleEnemyDestroyed(Enemy enemy) { _enemiesRemoved++; }

        public void EnableEndlessMode() { _isEndlessMode = true; }
    }
}
