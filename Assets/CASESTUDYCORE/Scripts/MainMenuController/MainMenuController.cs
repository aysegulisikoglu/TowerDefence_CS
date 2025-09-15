using UnityEngine;

namespace TD
{
    public class MainMenuController : MonoBehaviour
    {
        public void StartNewGame()
        {
            LevelManager.Instance.LoadLevel(LevelManager.Instance.allLevels[0]);
        }

        public void QuitGame() => Application.Quit();
    }
}
