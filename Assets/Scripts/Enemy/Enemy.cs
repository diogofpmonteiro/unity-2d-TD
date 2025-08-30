using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyData data; 
    private Vector3 _targetPosition;
    private Path _currentPath; 
    private int _currentWaypoint; 

    public static event Action<EnemyData> OnEnemyReachedEnd;

    private void Awake()
    {
        _currentPath = GameObject.Find("Path1").GetComponent<Path>();
    }

    private void OnEnable()
    {
        _currentWaypoint = 0; 
        _targetPosition = _currentPath.GetPosition(_currentWaypoint); 
    }

    void Update()
    {
        // move towards target position   
        transform.position = Vector3.MoveTowards(transform.position, _targetPosition,
                                                    data.speed * Time.deltaTime);

        // when target reached, set new target position
        float relativeDistance = (transform.position - _targetPosition).magnitude;
        if (relativeDistance < 0.1f)
        {
            if (_currentWaypoint < _currentPath.Waypoints.Length - 1)
            {
                _currentWaypoint++;
                _targetPosition = _currentPath.GetPosition(_currentWaypoint);
            }
            else // reached last waypoint
            {
                OnEnemyReachedEnd?.Invoke(data);
                gameObject.SetActive(false);
            }
        }
    }
}
