using UnityEngine;

[CreateAssetMenu(menuName = "CaseStudy/Weapon3D")]
public class Weapon3D : ScriptableObject
{
    public string id = "Red";
    public GameObject projectilePrefab;
    public float fireRate = 6f;
    public int burstCount = 1;
    public float spread = 0f;
    public float projectileSpeed = 12f;
    public float damage = 5f;
    public GameObject muzzleVfxPrefab;
    public GameObject hitVfxPrefab;
    public AudioClip shootSfx;
}
