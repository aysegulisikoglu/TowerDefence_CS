using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class PlayerHealth3D : MonoBehaviour
{
    [Header("Health")]
    public float maxHP = 5f;
    public float currentHP;

    [Header("Respawn")]
    public Transform spawnPoint;
    public GameObject deathVfxPrefab;
    public GameObject respawnVfxPrefab;

    [Header("I-Frame")]
    public float iFrameOnHit = 0.7f;
    public float iFrameOnRespawn = 1.5f;
    public SpriteRenderer bodySR;

    bool _invulnerable;
    Coroutine _flashCR;

    void Start()
    {
        if (!bodySR) bodySR = GetComponentInChildren<SpriteRenderer>(true);
        currentHP = Mathf.Clamp(currentHP <= 0 ? maxHP : currentHP, 0, maxHP);
    }

    public void Damage(float amount)
    {
        if (_invulnerable) return;

        currentHP -= amount;
        if (currentHP <= 0f)
        {
            Die();
            return;
        }

        if (AudioController.I && AudioController.I.playerHurt)
            AudioController.I.Play2D(AudioController.I.playerHurt, 0.9f, 0.05f);

        StartInvulnerability(iFrameOnHit);
    }

    public void HealFull() { currentHP = maxHP; }

    void Die()
    {
        if (AudioController.I && AudioController.I.playerDeath)
            AudioController.I.Play2D(AudioController.I.playerDeath, 1f, 0.05f);

        if (deathVfxPrefab) Instantiate(deathVfxPrefab, transform.position, Quaternion.identity);

        transform.position = spawnPoint ? spawnPoint.position : Vector3.zero;
        HealFull();

        if (respawnVfxPrefab) Instantiate(respawnVfxPrefab, transform.position, Quaternion.identity);
        if (AudioController.I && AudioController.I.respawn)
            AudioController.I.Play2D(AudioController.I.respawn, 0.9f, 0f);

        StartInvulnerability(iFrameOnRespawn);
    }

    void StartInvulnerability(float duration)
    {
        if (_flashCR != null) StopCoroutine(_flashCR);
        _flashCR = StartCoroutine(FlashCR(duration));
    }

    IEnumerator FlashCR(float duration)
    {
        _invulnerable = true;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            if (bodySR)
            {
                float a = 0.5f + Mathf.PingPong(Time.time * 8f, 0.5f);
                var c = bodySR.color; c.a = a; bodySR.color = c;
            }
            yield return null;
        }

        if (bodySR) { var c = bodySR.color; c.a = 1f; bodySR.color = c; }
        _invulnerable = false; _flashCR = null;
    }
}
