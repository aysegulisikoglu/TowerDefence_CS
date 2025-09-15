using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
public class ChaserEnemy3D : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 3.5f;
    public float groundY = 0f;

    [Header("Visual")]
    public SpriteRenderer bodySR;
    public Transform player;

    [Header("VFX (optional)")]
    public GameObject deathVfxPrefab;

    float _refindTimer;

    void Reset()
    {
        var col = GetComponent<Collider>();
        if (col) col.isTrigger = true;
        if (!bodySR) bodySR = GetComponentInChildren<SpriteRenderer>(true);
        gameObject.tag = "Enemy";
    }

    void Start()
    {
        if (!bodySR) bodySR = GetComponentInChildren<SpriteRenderer>(true);

        if (!player)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
        }
    }

    void Update()
    {
        if (Time.timeScale <= 0f) return;


        if (!player)
        {
            _refindTimer -= Time.deltaTime;
            if (_refindTimer <= 0f)
            {
                _refindTimer = 1f;
                var p = GameObject.FindGameObjectWithTag("Player");
                if (p) player = p.transform;
            }
            return;
        }


        Vector3 pos = transform.position;
        Vector3 dir = player.position - pos; dir.y = 0f;
        float mag = dir.magnitude;
        dir = (mag > 0.001f) ? (dir / mag) : Vector3.zero;

        pos += dir * speed * Time.deltaTime;
        pos.y = groundY;
        transform.position = pos;


        if (bodySR && Mathf.Abs(dir.x) > 0.01f)
        {
            var s = bodySR.transform.localScale;
            s.x = Mathf.Abs(s.x) * (dir.x >= 0 ? 1f : -1f);
            bodySR.transform.localScale = s;
        }
    }


    public void TakeDamage(float dmg)
    {
        if (deathVfxPrefab)
            Instantiate(deathVfxPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
