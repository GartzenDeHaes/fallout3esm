using System.Collections;
using System.IO;
using System.Threading;
using TES3Unity.Inputs;
using UnityEngine;
#if UNITY_ANDROID
using UnityEngine.Android;
#endif
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TES3Unity.Components
{
    public sealed class MenuComponent : MonoBehaviour
    {
        private MusicPlayer m_MusicPlayer = null;
        private Thread m_PreloadThread = null;
        private string m_GamePath = string.Empty;
        private bool m_MenuLoaded = false;

        [Header("Panels")]
        [SerializeField]
        private GameObject m_PermissionPanel = null;
        [SerializeField]
        private GameObject m_PreloadPanel = null;
        [SerializeField]
        private GameObject m_MenuPanel = null;
        [SerializeField]
        private GameObject m_OptionsPanel = null;

        [Header("UI Elements")]
        [SerializeField]
        private Image m_Background = null;

        [Header("Preloading")]
        [SerializeField]
        private GameObject m_DesktopPathSelection = null;
        [SerializeField]
        private GameObject m_MobilePathSelection = null;
        [SerializeField]
        private Text m_PathValidationText = null;

        [Header("Menu")]
        [SerializeField]
        private InputField m_PathSelector = null;
        [SerializeField]
        private Button m_LoadButton = null;
        [SerializeField]
        private Text m_InfoMessage = null;
        [SerializeField]
        private GameObject m_ButtonsContainer = null;
        [SerializeField]
        private Button m_LoadSaveButton = null;

#if UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
        [Header("Editor Only")]
        [SerializeField]
        private bool m_DisplayDesktopPathOnAndroid = true;
#endif

        private void Awake()
        {
            if (!CanReadStorage())
            {
                RequestReadStoragePermission();
                StartCoroutine(DeferredLoad());
            }
            else
            {
                Initialize();
            }
        }

        private void Initialize()
        {
            m_GamePath = GameSettings.GetDataPath();

            if (GameSettings.IsValidPath(m_GamePath))
            {
                LoadMenu();
            }
            else
            {
                LoadPreloader();
            }
        }

        private IEnumerator DeferredLoad()
        {
            SetPanelVisible(-1);

            // Required for static initialization of the InputManager.
            yield return null;

            var wait = new WaitForEndOfFrame();

            var actionMap = InputManager.GetActionMap("UI");
            actionMap.Enable();

            var backAction = actionMap["Menu"];

            while (!CanReadStorage())
            {
                if (backAction.phase == InputActionPhase.Started)
                {
                    Quit();
                }

                yield return wait;
            }

            Initialize();
        }

        #region PreLoading

        private void LoadPreloader()
        {
            var mobile = false;

#if UNITY_ANDROID || UNITY_IOS
            mobile = true;

#if UNITY_EDITOR
            if (m_DisplayDesktopPathOnAndroid)
            {
                mobile = false;
            }
#endif
#endif

            m_DesktopPathSelection.SetActive(!mobile);
            m_MobilePathSelection.SetActive(mobile);

            SetPanelVisible(0);
        }

        public void ValidatePathSelection()
        {
            m_GamePath = m_PathSelector.text;

            if (GameSettings.IsValidPath(m_GamePath))
            {
                GameSettings.SetDataPath(m_GamePath);
                m_PathValidationText.enabled = false;
                LoadMenu();
            }
            else
            {
                m_PathValidationText.enabled = true;
            }
        }

        #endregion

        #region Menu

        private void LoadMenu()
        {
            // Preload data if the reader is not yet initialized.
            if (TES3Engine.Inst.DataReader == null)
            {
                m_PreloadThread = new Thread(new ThreadStart(() =>
                {
                    TES3Engine.Inst.DataReader = new TES3DataReader(m_GamePath);
                }));

                m_PreloadThread.Start();

                StartCoroutine(CheckLoadButton());
            }

            SetPanelVisible(1);

            var path = Path.Combine(m_GamePath, "Splash");

            if (Directory.Exists(path))
            {
                var files = Directory.GetFiles(path);
                if (files.Length > 0)
                {
                    var target = files[Random.Range(0, files.Length - 1)];
                    var texture = TGALoader.LoadTGA(target);
                    var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                    m_Background.sprite = sprite;
                    m_Background.color = Color.white;
                }
            }

            path = Path.Combine(m_GamePath, "Music", "Explore", "Morrowind Title.mp3");

            if (File.Exists(path))
            {
                m_MusicPlayer = new MusicPlayer();
                m_MusicPlayer.AddSong(path);
                m_MusicPlayer.Play();
            }

            m_MenuLoaded = true;
        }

        public void LoadWorld()
        {
            TES3Engine.AutoLoadSavedGame = false;
            StartCoroutine(LoadWorld(m_GamePath));
        }

        public void LoadSavedGame()
        {
            TES3Engine.AutoLoadSavedGame = true;
            LoadWorld();
        }

        private IEnumerator CheckLoadButton()
        {
            var wait = new WaitForSeconds(0.5f);

            m_InfoMessage.text = "Please wait while loading Morrowind's data...";
            m_InfoMessage.enabled = true;

            m_LoadButton.interactable = false;
            m_LoadSaveButton.interactable = false;

            while (m_PreloadThread.IsAlive)
            {
                yield return wait;
            }

            m_LoadButton.interactable = true;
            m_InfoMessage.enabled = false;
            m_LoadSaveButton.interactable = !TES3Save.Get().IsEmpty();
        }

        private IEnumerator LoadWorld(string path)
        {
            m_ButtonsContainer.gameObject.SetActive(false);
            m_InfoMessage.text = "Loading...";
            m_InfoMessage.enabled = true;

            yield return new WaitForEndOfFrame();

            var asyncOperation = SceneManager.LoadSceneAsync("Game");
            var waitForSeconds = new WaitForSeconds(0.1f);

            while (!asyncOperation.isDone)
            {
                m_InfoMessage.text = string.Format("Loading {0}%", Mathf.RoundToInt(asyncOperation.progress * 100.0f));
                yield return waitForSeconds;
            }
        }

        public void Quit()
        {
            var dataReader = TES3Engine.Inst.DataReader;
            dataReader?.Close();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif

            Application.Quit();
        }

        #endregion

        #region Options

        public void ShowOptions(bool visible)
        {
            if (visible)
            {
                SetPanelVisible(2);
            }
            else
            {
                SetPanelVisible(1);
            }
        }

        #endregion

        private void RequestReadStoragePermission()
        {
#if UNITY_ANDROID
            Permission.RequestUserPermission(Permission.ExternalStorageRead);
#endif
        }

        public bool CanReadStorage()
        {
#if UNITY_ANDROID
            return Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead);
#else
            return true;
#endif
        }

        public bool CanLoadWorld()
        {
            return m_MenuLoaded && (m_PreloadThread == null || !m_PreloadThread.IsAlive);
        }

        public void SetPanelVisible(int index)
        {
            m_PermissionPanel.SetActive(index == -1);
            m_PreloadPanel.SetActive(index == 0);
            m_MenuPanel.SetActive(index == 1);
            m_OptionsPanel.SetActive(index == 2);
        }

        private IEnumerator ShowMessage(string message, float duration)
        {
            m_InfoMessage.text = message;
            m_InfoMessage.enabled = true;
            yield return new WaitForSeconds(duration);
            m_InfoMessage.enabled = false;
        }
    }
}