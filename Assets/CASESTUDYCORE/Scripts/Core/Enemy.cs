using System;
using UnityEngine;

namespace TD
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private TD.EnemyData data;
        public TD.EnemyData Data => data;

        public static float GlobalSpeed = 1f;

        public static event Action<TD.EnemyData> OnEnemyReachedEnd;
        public static event Action<Enemy> OnEnemyDestroyed;

        private Path _currentPath;
        private Vector3 _targetPosition;
        private int _currentWaypoint;
        private float _lives;
        private float _maxLives;

        [SerializeField] private Transform healthBar;
        private Vector3 _healthBarOriginalScale;

        private bool _hasBeenCounted = false;

        private float _speedMul = 1f;
        public void SetSpeedMultiplier(float m) { _speedMul = Mathf.Max(0.1f, m); }

        void Awake()
        {
            _currentPath = GameObject.Find("Path1").GetComponent<Path>();
            _healthBarOriginalScale = healthBar.localScale;
        }

        void OnEnable()
        {
            _currentWaypoint = 0;
            _targetPosition = _currentPath.GetPosition(_currentWaypoint);
        }

        void Update()
        {
            if (Time.timeScale <= 0f) return;
            if (_hasBeenCounted) return;

            transform.position = Vector3.MoveTowards(
    transform.position, _targetPosition, (data.speed * _speedMul * GlobalSpeed) * Time.deltaTime);

            float relativeDistance = (transform.position - _targetPosition).magnitude;
            if (relativeDistance < 0.1f)
            {
                if (_currentWaypoint < _currentPath.Waypoints.Length - 1)
                {
                    _currentWaypoint++;
                    _targetPosition = _currentPath.GetPosition(_currentWaypoint);
                }
                else
                {
                    _hasBeenCounted = true;
                    OnEnemyReachedEnd?.Invoke(data);
                    gameObject.SetActive(false);
                }
            }
        }

        public void TakeDamage(float damage)
        {
            if (_hasBeenCounted) return;

            _lives -= damage;
            _lives = Math.Max(_lives, 0);
            UpdateHealthBar();

            if (_lives <= 0)
            {
                _hasBeenCounted = true;


                if (AudioController.I && AudioController.I.enemyDie)
                    AudioController.I.PlayAt(AudioController.I.enemyDie, transform.position, 0.8f, 0.05f);

                OnEnemyDestroyed?.Invoke(this);
                gameObject.SetActive(false);
            }
        }

        void UpdateHealthBar()
        {
            if (!healthBar) return;
            float healthPercent = _lives / _maxLives;
            Vector3 scale = _healthBarOriginalScale;
            scale.x = _healthBarOriginalScale.x * healthPercent;
            healthBar.localScale = scale;
        }

        public void Initialize(float healthMultiplier)
        {
            _hasBeenCounted = false;
            _maxLives = data.lives * healthMultiplier;
            _lives = _maxLives;
            UpdateHealthBar();
        }
    }
}
