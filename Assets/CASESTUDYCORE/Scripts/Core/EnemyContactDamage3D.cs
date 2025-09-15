using UnityEngine;

[DisallowMultipleComponent]
public class EnemyContactDamage3D : MonoBehaviour
{
    public float damage = 1f;
    public float tick = 0.35f;
    float _t;


    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        var hp = other.GetComponent<PlayerHealth3D>();
        if (!hp) return;

        hp.Damage(damage);
        _t = tick;
    }


    void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        var hp = other.GetComponent<PlayerHealth3D>();
        if (!hp) return;

        _t -= Time.deltaTime;
        if (_t > 0f) return;
        _t = tick;

        hp.Damage(damage);
    }
}
