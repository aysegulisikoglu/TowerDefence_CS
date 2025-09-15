namespace TD
{
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance { get; private set; }

        public LevelData[] allLevels;
        public LevelData CurrentLevel { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void LoadLevel(LevelData levelData)
        {
            if (levelData == null)
            {
                Debug.LogError("LevelManager.LoadLevel: levelData is NULL. LevelManager.allLevels'e asset atadın mı?");
                return;
            }
            if (string.IsNullOrEmpty(levelData.levelName))
            {
                Debug.LogError("LevelManager.LoadLevel: levelData.levelName boş. Sahne adını LevelData'da doldur.");
                return;
            }

            CurrentLevel = levelData;
            SceneManager.LoadScene(levelData.levelName);
        }
    }
}
