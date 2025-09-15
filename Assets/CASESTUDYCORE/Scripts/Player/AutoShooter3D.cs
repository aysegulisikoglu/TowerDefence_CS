using System.Collections.Generic;
using UnityEngine;

public class AutoShooter3D : MonoBehaviour
{
    [Header("Shoot")]
    public Transform muzzle;
    public Projectile3D projectilePrefab;
    public TD.ObjectPooler projectilePool;
    public float range = 9f;
    public float fireInterval = 0.25f;

    [Header("Melee (optional)")]
    public bool useMelee = false;
    public float meleeRadius = 2f;
    public float meleeDamage = 12f;

    float fireTimer;
    float rescanTimer;
    GameObject[] cachedEnemies;

    [Header("Facing / Cone")]
    public bool requireFacing = true;
    [Range(1f, 180f)] public float coneAngle = 90f;
    [Range(0f, 180f)] public float fallbackConeAngle = 130f;

    Vector3 _lastInputDir = Vector3.forward;

    public enum TargetingMode { Nearest, TopN, AllInCone }
    [Header("Targeting")]
    public TargetingMode targetingMode = TargetingMode.TopN;
    public int maxTargetsPerShot = 3;

    [Header("Multi-Weapon Pools (optional)")]
    public TD.ObjectPooler poolBlue;
    public TD.ObjectPooler poolOrange;
    public TD.ObjectPooler poolPurple;
    public int currentIndex = 0;

    [Header("Perf")]
    public float rescanPeriod = 0.12f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) useMelee = !useMelee;
        if (Input.GetKeyDown(KeyCode.Alpha1)) currentIndex = 0;
        if (Input.GetKeyDown(KeyCode.Alpha2)) currentIndex = 1;
        if (Input.GetKeyDown(KeyCode.Alpha3)) currentIndex = 2;

        float ix = Input.GetAxisRaw("Horizontal");
        float iz = Input.GetAxisRaw("Vertical");
        Vector3 input = new Vector3(ix, 0f, iz);
        if (input.sqrMagnitude > 0.001f) _lastInputDir = input.normalized;

        rescanTimer -= Time.deltaTime;
        if (rescanTimer <= 0f)
        {
            rescanTimer = Mathf.Max(0.04f, rescanPeriod);
            cachedEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        }

        fireTimer -= Time.deltaTime;
        if (fireTimer > 0f) return;

        if (useMelee) { TryMelee(); return; }

        Vector3 me = muzzle ? muzzle.position : transform.position;
        Vector3 fwd = GetForwardXZ();

        if (!TryShoot(me, fwd, coneAngle) && requireFacing && fallbackConeAngle > coneAngle)
            TryShoot(me, fwd, fallbackConeAngle);
    }

    void TryMelee()
    {
        Vector3 me = muzzle ? muzzle.position : transform.position;
        var hits = Physics.OverlapSphere(me, meleeRadius);
        bool hitSomething = false;

        foreach (var h in hits)
        {
            if (h && h.CompareTag("Enemy"))
            {
                var eTD = h.GetComponent<TD.Enemy>();
                if (eTD != null) eTD.TakeDamage(meleeDamage);
                else h.SendMessage("TakeDamage", meleeDamage, SendMessageOptions.DontRequireReceiver);
                hitSomething = true;
            }
        }

        if (hitSomething)
        {
            fireTimer = fireInterval;
            if (AudioController.I && AudioController.I.meleeSwing)
                AudioController.I.PlayAt(AudioController.I.meleeSwing, me, 0.9f, 0.05f);
        }
    }

    bool TryShoot(Vector3 me, Vector3 fwd, float angle)
    {
        if (cachedEnemies == null || cachedEnemies.Length == 0) return false;

        float cosHalf = Mathf.Cos((angle * 0.5f) * Mathf.Deg2Rad);
        float rangeSqr = range * range;

        List<GameObject> inCone = new List<GameObject>(16);
        foreach (var go in cachedEnemies)
        {
            if (!go || !go.activeInHierarchy) continue;

            Vector3 to = go.transform.position - me;
            to.y = 0f;
            float d2 = to.sqrMagnitude;
            if (d2 > rangeSqr) continue;

            if (requireFacing)
            {
                Vector3 dir = (d2 > 0.0001f) ? to / Mathf.Sqrt(d2) : Vector3.zero;
                float dot = Vector3.Dot(fwd, dir);
                if (dot < cosHalf) continue;
            }

            inCone.Add(go);
        }

        if (inCone.Count == 0) return false;

        switch (targetingMode)
        {
            case TargetingMode.Nearest:
                ShootOne(me, Nearest(inCone, me));
                break;
            case TargetingMode.TopN:
                inCone.Sort((a, b) =>
                {
                    float da = (a.transform.position - me).sqrMagnitude;
                    float db = (b.transform.position - me).sqrMagnitude;
                    return da.CompareTo(db);
                });
                for (int i = 0; i < Mathf.Min(maxTargetsPerShot, inCone.Count); i++)
                    ShootOne(me, inCone[i]);
                break;
            case TargetingMode.AllInCone:
                for (int i = 0; i < Mathf.Min(maxTargetsPerShot <= 0 ? inCone.Count : maxTargetsPerShot, inCone.Count); i++)
                    ShootOne(me, inCone[i]);
                break;
        }

        fireTimer = fireInterval;
        return true;
    }

    GameObject Nearest(List<GameObject> list, Vector3 me)
    {
        GameObject bestGo = null;
        float best = float.PositiveInfinity;
        foreach (var go in list)
        {
            float d2 = (go.transform.position - me).sqrMagnitude;
            if (d2 < best) { best = d2; bestGo = go; }
        }
        return bestGo;
    }

    void ShootOne(Vector3 me, GameObject target)
    {
        if (!target) return;
        Vector3 shootDir = target.transform.position - me;
        shootDir.y = 0f;
        if (shootDir.sqrMagnitude < 0.0001f) shootDir = GetForwardXZ();
        else shootDir.Normalize();

        GameObject projGo;
        var pool = CurrentPoolOrDefault();
        if (pool != null)
        {
            projGo = pool.GetPooledObject();
            projGo.transform.position = me;
            projGo.transform.rotation = Quaternion.LookRotation(shootDir, Vector3.up);
            projGo.SetActive(true);
        }
        else
        {
            projGo = Instantiate(projectilePrefab.gameObject, me, Quaternion.LookRotation(shootDir, Vector3.up));
        }

        var proj = projGo.GetComponent<Projectile3D>();
        proj.Fire(shootDir);


        if (AudioController.I)
        {
            AudioClip shot = AudioController.I.shootBlue;
            if (currentIndex == 1 && AudioController.I.shootOrange) shot = AudioController.I.shootOrange;
            else if (currentIndex == 2 && AudioController.I.shootPurple) shot = AudioController.I.shootPurple;

            AudioController.I.PlayAt(shot, me, 0.8f, 0.05f);
        }
    }

    Vector3 GetForwardXZ()
    {
        return (_lastInputDir.sqrMagnitude > 0.0001f) ? _lastInputDir : Vector3.forward;
    }

    TD.ObjectPooler CurrentPoolOrDefault()
    {
        if (poolBlue == null && poolOrange == null && poolPurple == null) return projectilePool;
        if (currentIndex == 1 && poolOrange != null) return poolOrange;
        if (currentIndex == 2 && poolPurple != null) return poolPurple;
        return (poolBlue != null) ? poolBlue : projectilePool;
    }

    void OnDrawGizmosSelected()
    {
        Vector3 me = muzzle ? muzzle.position : transform.position;
        Vector3 fwd = GetForwardXZ();
        DrawCone(me, fwd, coneAngle, range, Color.cyan);
        if (fallbackConeAngle > coneAngle)
            DrawCone(me, fwd, fallbackConeAngle, range, new Color(1f, 0.6f, 0f, 0.35f));
    }
    void DrawCone(Vector3 origin, Vector3 forward, float angle, float r, Color c)
    {
        Gizmos.color = c;
        Vector3 right = Quaternion.Euler(0f, angle * 0.5f, 0f) * forward;
        Vector3 left = Quaternion.Euler(0f, -angle * 0.5f, 0f) * forward;
        Gizmos.DrawLine(origin, origin + forward.normalized * r);
        Gizmos.DrawLine(origin, origin + right.normalized * r);
        Gizmos.DrawLine(origin, origin + left.normalized * r);
    }
}
