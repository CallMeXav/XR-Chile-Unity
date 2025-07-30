
using UnityEngine;
public class FireObject : MonoBehaviour, IExtinguishable
{
    [Header("Fire Settings")]
    [SerializeField] private FireType _type;
    public float burnTime = 5.0f; //Seconds
    public float combustibility = 0.5f;
    public float explosionRadius = 0.0f;
    public GameObject fireParticlesPrefab;

    [Header("Life System")]
    [SerializeField] private float _maxLife = 100f;
    private float _currentLife;
    [SerializeField, Tooltip("How much does the expansion affects life"), Range(0f,2f)] private float _expansionFactor = 1.2f;
    [SerializeField] private float _expansionTime = 2.5f; //Seconds

    private float _expansionTimer;
    private float _burnMultiplier = 1;
    private Material _material;
    private FireParticle _fireParticle;
    private GameObject _fireParticleGameObject;

    [HideInInspector] public bool isBurning = false;
    [HideInInspector] public bool isBurnt = false;
    [HideInInspector] public float burnTimer = 0.0f;

    public float GetPercentCombusted() => Mathf.Clamp01((burnTime - burnTimer) / burnTime);

    void Awake()
    {
        _fireParticleGameObject = GameObject.Instantiate(fireParticlesPrefab, transform);
        _fireParticleGameObject.transform.position = transform.position;
    }
    void Start()
    {
        _material = GetComponent<Renderer>().material;
        _fireParticle = _fireParticleGameObject.GetComponent<FireParticle>();
    }

    public void Ignite()
    {
        if (isBurning || isBurnt)
        {
            return;
        }

        isBurning = true;
        burnTimer = burnTime;
        _expansionTimer = _expansionTime;
        _currentLife = _maxLife;
        _burnMultiplier = 1;

        Debug.Log($"{gameObject.name} has ignited!");
    }

    // Update is called once per frame
    public void BurnUpdate()
    {
        if (!isBurning) return;

        burnTimer -= Time.deltaTime;
        _expansionTimer -= Time.deltaTime * _burnMultiplier;

        //Calculate progress (0 to 1)
        float burningProgress = Mathf.Clamp01(1.0f - (burnTimer / burnTime));
        float expansionProgress = Mathf.Clamp01(1.0f - (_expansionTimer / _expansionTime));

        _material.SetFloat("_BurnProgress", burningProgress);
        _fireParticle.UpdateValues(expansionProgress);

        if (burnTimer <= 0.0f) BurnOut();
    }

    private void BurnOut()
    {
        isBurning = false;
        isBurnt = true;
        _material.SetFloat("_BurnProgress", 1f);
        Debug.Log($"{gameObject.name} has burnt out.");
    }

    public void Extinguish(FireType foamType)
    {
        if (isBurnt || !isBurning) return; //If already burnt or not burning do nothing

        float effectiveness = foamType == _type ? 1.0f : 0.66f;

        float extinguishEffect = effectiveness * 12f * Time.deltaTime;
        extinguishEffect /= CalculateExpansionModifier();

        _currentLife = Mathf.Max(_currentLife - extinguishEffect, 0);
        _burnMultiplier = _currentLife / _maxLife;

        if (_currentLife <= 0) CompletelyExtinguish();
    }
    private float CalculateExpansionModifier()
    {
        float expansionProgress = 1f - Mathf.Clamp01(_expansionTimer / _expansionTime);
        return 1f + (expansionProgress * _expansionFactor);
    }

    private void CompletelyExtinguish()
    {
        isBurning = false;
        _burnMultiplier = 0;
        _fireParticle.Extinguish();
        Debug.Log($"{gameObject.name} has been completely extinguished.");
    }

}

