using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class PlayerTopDown3D : MonoBehaviour
{
    public Animator anim;

    public float moveSpeed = 8f;
    public float maxHP = 100f;
    public float iFrameDuration = 0.8f;
    public SpriteRenderer bodySR;
    Rigidbody rb;
    Vector3 input;
    float hp;
    float iTimer;

    void Awake() { rb = GetComponent<Rigidbody>(); }
    void Start() { hp = maxHP; rb.useGravity = false; }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        input = new Vector3(h, 0f, v).normalized;

        if (iTimer > 0f) iTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Escape))
            Time.timeScale = Time.timeScale == 0f ? 1f : 0f;
        if (anim)
        {
            anim.SetFloat("MoveX", input.x);
            anim.SetFloat("MoveZ", input.z);
        }

    }

    void FixedUpdate()
    {
        rb.linearVelocity = input * moveSpeed;

        if (anim) anim.SetFloat("Speed", rb.linearVelocity.magnitude);

    }

    public void TakeDamage(float dmg)
    {
        if (iTimer > 0f) return;
        hp -= dmg;
        iTimer = iFrameDuration;
        StopAllCoroutines();
        StartCoroutine(Blink());
        if (hp <= 0f)
        {
            gameObject.SetActive(false);
            Time.timeScale = 0f;
        }
    }

    public void ReviveFull(float delay = 1f)
    {
        StartCoroutine(_Revive(delay));
    }

    IEnumerator _Revive(float d)
    {
        yield return new WaitForSeconds(d);
        hp = maxHP;
        iTimer = iFrameDuration;
        gameObject.SetActive(true);
    }

    IEnumerator Blink()
    {
        if (!bodySR) yield break;
        float t = iFrameDuration;
        bool on = false;
        while (t > 0f)
        {
            on = !on;
            bodySR.color = on ? new Color(1, 1, 1, 0.35f) : Color.white;
            yield return new WaitForSeconds(0.1f);
            t -= 0.1f;
        }
        bodySR.color = Color.white;
    }
}
