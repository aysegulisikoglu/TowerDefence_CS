using UnityEngine;
using TMPro;

public class StressCounter : MonoBehaviour
{
    public TMP_Text animCountText;
    public TMP_Text enemyCountText;

    void Update()
    {
        int anims = Object.FindObjectsByType<Animator>(FindObjectsSortMode.None).Length;
        int enemies = Object.FindObjectsByType<TD.Enemy>(FindObjectsSortMode.None).Length;

        if (animCountText) animCountText.text = $"Active Animations: {anims}";
        if (enemyCountText) enemyCountText.text = $"Active Enemies: {enemies}";
    }
}
