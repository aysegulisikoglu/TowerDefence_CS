using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TD
{
    public class UIController_TDLite : MonoBehaviour
    {
        public static UIController_TDLite Instance { get; private set; }

        [SerializeField] bool autoWireByName = true;





        [Header("Panels & Buttons")]
        [SerializeField] GameObject pausePanel;
        [SerializeField] Button speed1Button;
        [SerializeField] Button speed2Button;
        [SerializeField] Button speed3Button;
        [SerializeField] Button pauseButton;

        [Header("Optional")]
        [SerializeField] TMP_Text objectiveText;
        [SerializeField] GameObject gameOverPanel;
        [SerializeField] GameObject missionCompletePanel;

        bool _isPaused;

        float _prevScale = 1f;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (autoWireByName) AutoWire();
        }


        void AutoWire()
        {



            if (!speed1Button) speed1Button = transform.Find("SpeedBar/Speed1Button")?.GetComponent<Button>();
            if (!speed2Button) speed2Button = transform.Find("SpeedBar/Speed2Button")?.GetComponent<Button>();
            if (!speed3Button) speed3Button = transform.Find("SpeedBar/Speed3Button")?.GetComponent<Button>();
            if (!pauseButton) pauseButton = transform.Find("PauseButton")?.GetComponent<Button>();


            if (!pausePanel) pausePanel = transform.Find("PausePanel")?.gameObject;
            if (pausePanel)
            {
                var grp = pausePanel.transform.Find("ButtonGroup");
                var resume = grp?.Find("ResumeButton")?.GetComponent<Button>();
                var restart = grp?.Find("RestartLevelButton")?.GetComponent<Button>();
                var menu = grp?.Find("MainMenuButton")?.GetComponent<Button>();
                var quit = grp?.Find("QuitGameButton")?.GetComponent<Button>();

                if (resume) { resume.onClick.RemoveAllListeners(); resume.onClick.AddListener(TogglePause); }
                if (restart) { restart.onClick.RemoveAllListeners(); restart.onClick.AddListener(RestartLevel); }
                if (menu) { menu.onClick.RemoveAllListeners(); menu.onClick.AddListener(GoToMainMenu); }
                if (quit) { quit.onClick.RemoveAllListeners(); quit.onClick.AddListener(QuitGame); }
            }


            if (!gameOverPanel) gameOverPanel = transform.Find("GameOverPanel")?.gameObject;
            if (gameOverPanel)
            {
                var grp = gameOverPanel.transform.Find("ButtonGroup");
                var restart = grp?.Find("RestartLevelButton")?.GetComponent<Button>();
                var menu = grp?.Find("MainMenuButton")?.GetComponent<Button>();
                var quit = grp?.Find("QuitGameButton")?.GetComponent<Button>();

                if (restart) { restart.onClick.RemoveAllListeners(); restart.onClick.AddListener(RestartLevel); }
                if (menu) { menu.onClick.RemoveAllListeners(); menu.onClick.AddListener(GoToMainMenu); }
                if (quit) { quit.onClick.RemoveAllListeners(); quit.onClick.AddListener(QuitGame); }
            }
        }


        void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        void Start()
        {

            if (speed1Button) speed1Button.onClick.AddListener(() => { TD.Enemy.GlobalSpeed = 0.6f; Highlight(0.6f); });
            if (speed2Button) speed2Button.onClick.AddListener(() => { TD.Enemy.GlobalSpeed = 1.0f; Highlight(1.0f); });
            if (speed3Button) speed3Button.onClick.AddListener(() => { TD.Enemy.GlobalSpeed = 1.8f; Highlight(1.8f); });
            if (pauseButton) pauseButton.onClick.AddListener(TogglePause);

            Highlight(GameManager.Instance?.GameSpeed ?? 1f);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) TogglePause();
        }



        void OnLivesChanged(int lives)
        {
            {
                GameManager.Instance.SetTimeScale(0f);
                gameOverPanel.SetActive(true);
            }
        }

        void OnSceneLoaded(Scene s, LoadSceneMode m)
        {

            var cam = GameObject.FindGameObjectWithTag("MainCamera")?.GetComponent<Camera>();
            var canvas = GetComponent<Canvas>();
            if (canvas && cam) canvas.worldCamera = cam;

            bool isMainMenu = s.name == "MainMenu" || s.buildIndex == 0;
            if (isMainMenu) HideUI(); else ShowUI();

            if (!isMainMenu && objectiveText) { StopAllCoroutines(); StartCoroutine(ShowObjectiveCR()); }


            if (pausePanel) pausePanel.SetActive(false);
            if (missionCompletePanel) missionCompletePanel.SetActive(false);
            if (gameOverPanel) gameOverPanel.SetActive(false);
            _isPaused = false;
            TD.GameManager.Instance.SetTimeScale(1f);
        }

        IEnumerator ShowObjectiveCR()
        {
            var lm = LevelManager.Instance;
            if (lm && lm.CurrentLevel && objectiveText)
            {
                objectiveText.text = $"Survive {lm.CurrentLevel.wavesToWin} waves!";
                objectiveText.gameObject.SetActive(true);
                yield return new WaitForSeconds(3f);
                objectiveText.gameObject.SetActive(false);
            }
        }


        void HideUI()
        {

            SetSpeedBarActive(false);
            if (pauseButton) pauseButton.gameObject.SetActive(false);
        }
        void ShowUI()
        {

            SetSpeedBarActive(true);
            if (pauseButton) pauseButton.gameObject.SetActive(true);
        }
        void SetSpeedBarActive(bool a)
        {
            if (speed1Button) speed1Button.gameObject.SetActive(a);
            if (speed2Button) speed2Button.gameObject.SetActive(a);
            if (speed3Button) speed3Button.gameObject.SetActive(a);
        }


        public void TogglePause()
        {
            if (!pausePanel) return;

            _isPaused = !_isPaused;
            pausePanel.SetActive(_isPaused);

            if (_isPaused)
            {
                _prevScale = Time.timeScale;
                Time.timeScale = 0f;
            }
            else
            {
                Time.timeScale = (_prevScale <= 0f) ? 1f : _prevScale;
            }
        }


        void Highlight(float selected)
        {
            SetSel(speed1Button, Mathf.Abs(selected - 0.6f) < 0.01f);
            SetSel(speed2Button, Mathf.Abs(selected - 1.0f) < 0.01f);
            SetSel(speed3Button, Mathf.Abs(selected - 1.8f) < 0.01f);
        }
        void SetSel(Button b, bool sel)
        {
            if (!b) return;
            var img = b.image; if (img) img.color = sel ? new Color(0.2f, 0.5f, 1f, 1f) : Color.white;
            var txt = b.GetComponentInChildren<TMP_Text>(); if (txt) txt.color = sel ? Color.white : Color.black;
        }


        public void RestartLevel()
        {

            _isPaused = false;
            if (pausePanel) pausePanel.SetActive(false);
            TD.GameManager.Instance.SetTimeScale(1f);


            LevelManager.Instance.LoadLevel(LevelManager.Instance.CurrentLevel);
        }
        public void GoToMainMenu() { GameManager.Instance.SetTimeScale(1f); SceneManager.LoadScene(0); }
        public void QuitGame() => Application.Quit();
    }
}
