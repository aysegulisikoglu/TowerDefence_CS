namespace TD
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "LevelData", menuName = "Scriptable Objects/LevelData")]
    public class LevelData : ScriptableObject
    {
        public string levelName;
        public int wavesToWin;
        public int startingResources;
        public int startingLives;


    }
}