using UnityEngine;

public class Extinguisher : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private FireType _type;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private FoamProjectile _foamPrefab;
    [SerializeField] private float _foamSpeed = 10f;
    [SerializeField] private float _fireRate = 0.1f;
    [SerializeField] private int _poolSize = 20;

    private ObjectPool<FoamProjectile> _pool;
    private float _fireTimer;
    [SerializeField] private bool _isExtinguishing;

    void Awake()
    {
        _pool = new ObjectPool<FoamProjectile>(_foamPrefab, _poolSize);
    }

    void Update()
    {
        if (!_isExtinguishing) return;

        _fireTimer += Time.deltaTime;
        if (_fireTimer >= _fireRate)
        {
            _fireTimer = 0f;
            FireFoam();
        }
    }

    private void FireFoam()
    {
        FoamProjectile foam = _pool.GetFromPool();
        foam.transform.position = _firePoint.position;
        foam.transform.rotation = _firePoint.rotation;
        foam.Initialize(_type, _pool);
        foam.Fire(_firePoint.forward, _foamSpeed);
    }

    public void StartExtinguishing() => _isExtinguishing = true;

    public void StopExtinguishing() => _isExtinguishing = false;
}
