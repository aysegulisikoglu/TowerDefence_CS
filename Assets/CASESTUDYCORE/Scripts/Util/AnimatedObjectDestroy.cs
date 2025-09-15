using UnityEngine;

[DisallowMultipleComponent]
public class AnimatedObjectDestroy : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [Tooltip("Klip süresi bulunamazsa bu kadar sonra yok et.")]
    [SerializeField] private float fallbackLifetime = 1f;

    void OnEnable()
    {
        if (!animator) animator = GetComponent<Animator>();

        float life = fallbackLifetime;

        if (animator)
        {

            animator.Update(0f);


            var infos = animator.GetCurrentAnimatorClipInfo(0);
            if (infos != null && infos.Length > 0 && infos[0].clip)
            {

                life = infos[0].clip.length / Mathf.Max(0.0001f, animator.speed);
            }
            else
            {

                var st = animator.GetCurrentAnimatorStateInfo(0);
                if (st.length > 0f)
                    life = st.length / Mathf.Max(0.0001f, animator.speed);
            }
        }

        Destroy(gameObject, life);
    }
}

