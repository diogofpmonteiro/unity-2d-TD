using System;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public static event Action<int> OnMissionComplete;
    public static event Action<int> OnWaveChanged;

    [SerializeField] private WaveData[] waves;
    [SerializeField] private ObjectPooler orcPool;
    [SerializeField] private ObjectPooler dragonPool;
    [SerializeField] private ObjectPooler kaijuPool;


    private Dictionary<EnemyType, ObjectPooler> _poolDictionary;
    private int _currentWaveIndex = 0;
    private int _waveCounter = 0;
    private float _spawnTimer;
    private float _spawnCounter;
    private int _enemiesRemoved;
    private WaveData CurrentWave => waves[_currentWaveIndex];
    private float _timeBetweenWaves = 1f;
    private float _waveCooldown;
    private bool _isBetweenWaves = false;



    private void Awake()
    {
        _poolDictionary = new Dictionary<EnemyType, ObjectPooler>()
        {
            { EnemyType.Orc, orcPool },
            { EnemyType.Dragon, dragonPool },
            { EnemyType.Kaiju, kaijuPool },
        };
    }

    private void OnEnable()
    {
        Enemy.OnEnemyReachedEnd += HandleEnemyReachedEnd;
        Enemy.OnEnemyDestroyed += HandleEnemyDestroyed;
    }

    private void OnDisable()
    {
        Enemy.OnEnemyReachedEnd -= HandleEnemyReachedEnd;
        Enemy.OnEnemyDestroyed -= HandleEnemyDestroyed;
    }

    private void Start()
    {
        OnWaveChanged?.Invoke(_waveCounter);
    }


    void Update()
    {
        if (_isBetweenWaves)
        {
            _waveCooldown -= Time.deltaTime;
            if (_waveCooldown <= 0f)
            {
                // if we won enough waves to complete mission
                if (_waveCounter + 1 >= LevelManager.Instance.CurrentLevel.wavesToWin)
                {
                    OnMissionComplete?.Invoke(_waveCounter);
                    return;
                }

                // if we completed the current wave
                _currentWaveIndex = (_currentWaveIndex + 1) % waves.Length;
                _waveCounter++;
                OnWaveChanged?.Invoke(_waveCounter);
                _spawnCounter = 0;
                _enemiesRemoved = 0;
                _spawnTimer = 0f;
                _isBetweenWaves = false;
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

    private void SpawnEnemy()
    {
        if (_poolDictionary.TryGetValue(CurrentWave.enemyType, out var pool))
        {
            GameObject spawnedObject = pool.GetPooledObject();
            spawnedObject.transform.position = transform.position; // where game objects will be spawned

            float healthMultiplier = 1f + (_waveCounter * 0.1f); // 10% per wave
            Enemy enemy = spawnedObject.GetComponent<Enemy>();
            enemy.Initialize(healthMultiplier);
            
            spawnedObject.SetActive(true);
        }
    }

    private void HandleEnemyReachedEnd(EnemyData data)
    {
        _enemiesRemoved++;
    }

    private void HandleEnemyDestroyed(Enemy enemy)
    {
        _enemiesRemoved++;
    }
}
