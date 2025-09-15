using UnityEngine;

[CreateAssetMenu(menuName="CaseStudy/Enemy Color Profile")]
public class EnemyColorProfile : ScriptableObject
{
    public Color tint = Color.white;
    [Range(0.2f, 5f)] public float speedMul = 1f;
    [Range(0.2f, 5f)] public float hpMul    = 1f;
    [Range(0.2f, 5f)] public float dmgMul   = 1f;
}
