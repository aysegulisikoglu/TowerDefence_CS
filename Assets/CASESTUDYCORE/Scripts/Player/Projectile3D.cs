using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class Projectile3D : MonoBehaviour
{
    static float _lastVfxTime;
    [Header("Defaults (override edilebilir)")]
    public float speed = 24f;
    public float life = 1.5f;
    public float damage = 10f;

    [Header("Optional VFX Prefab")]
    public GameObject hitVfxPrefab;


    private Vector3 dir;
    private float t;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    void OnEnable() { t = life; }

    void Update()
    {
        t -= Time.deltaTime;
        if (t <= 0f) { gameObject.SetActive(false); return; }

        if (rb == null || rb.isKinematic)
            transform.position += dir * speed * Time.deltaTime;
        else
            rb.linearVelocity = dir * speed;
    }


    public void Fire(Vector3 direction)
    {
        dir = direction.normalized;
        t = life;

    }


    public void Fire(Vector3 direction, float overrideSpeed, float overrideDamage)
    {
        speed = overrideSpeed;
        damage = overrideDamage;
        Fire(direction);

    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;


        other.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);


        if (AudioController.I && AudioController.I.projectileHit)
            AudioController.I.PlayAt(AudioController.I.projectileHit, transform.position, 0.9f, 0.05f);


        if (hitVfxPrefab && (Time.unscaledTime - _lastVfxTime) > 0.02f)
        {
            Instantiate(hitVfxPrefab, transform.position, Quaternion.identity);
            _lastVfxTime = Time.unscaledTime;
        }


        gameObject.SetActive(false);
    }
}
