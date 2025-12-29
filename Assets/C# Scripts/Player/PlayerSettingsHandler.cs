using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;



public class PlayerSettingsHandler : MonoBehaviour
{
    public static PlayerSettings Settings { get; private set; }

    [SerializeField] private InputActionAsset playerInput;
    [SerializeField] private InputActionReference playerMenuInput;

    [Header("Where is UI Parent")]
    [SerializeField] private RectTransform UITransform;
    private bool isSettingsMenuActive;

    [Header("Default settings if no save file found")]
    [SerializeField] private PlayerSettings defaultSettings;

    private const string SettingsFilePath = "SaveData/PlayerSettings";

    private bool savedWasPlayerInControl;


    private async void Awake()
    {
        // Load saved MatchSettings, or load default if that doesnt exist.
        Settings = await LoadSettingsFromFileAsync();

        UIComponentGroup[] UIInputHandlers = UITransform.GetComponentsInChildren<UIComponentGroup>(true);
        int UIhandlerCount = UIInputHandlers.Length;

        for (int i = 0; i < UIhandlerCount; i++)
        {
            int dataIndex = i;
            UIInputHandlers[i].Init(Settings.GetSavedData(dataIndex));

            UIInputHandlers[i].OnValueChanged += (value) => UpdateMatchSettingsSettings(dataIndex, value);
        }

        playerMenuInput.asset.Enable();
        playerMenuInput.action.performed += OnOpenSettings;
    }

    public async void OnOpenSettings(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            if (isSettingsMenuActive)
            {
                bool isInMainSettingsScreen = ReturnButton.PressActiveReturnButton();
                if (isInMainSettingsScreen == false)
                {
                    UITransform.gameObject.SetActive(false);
                    isSettingsMenuActive = false;
                    playerInput.Enable();

                    PlayerDataLibrary.Controller.IsControlEnabled = savedWasPlayerInControl;
                    PlayerDataLibrary.FlashLight.IsForceDisabled = !savedWasPlayerInControl;

                    await SaveSettingsAsync(Settings);
                }
            }
            else
            {
                UITransform.gameObject.SetActive(true);
                isSettingsMenuActive = true;
                playerInput.Disable();

                savedWasPlayerInControl = PlayerDataLibrary.Controller.IsControlEnabled;
                PlayerDataLibrary.Controller.IsControlEnabled = false;
                PlayerDataLibrary.FlashLight.IsForceDisabled = true;
            }
        }
    }

    private void UpdateMatchSettingsSettings(int sliderId, float value)
    {
        Settings.SetData(sliderId, value);
    }


    private async Task<PlayerSettings> LoadSettingsFromFileAsync()
    {
        (bool succes, PlayerSettings loadedMatchSettings) = await FileManager.LoadInfoAsync<PlayerSettings>(SettingsFilePath);

        if (succes)
        {
            return loadedMatchSettings;
        }
        else
        {
            return defaultSettings;
        }
    }

    /// <summary>
    /// Settings are saved when creating the lobby
    /// </summary>
    private async Task SaveSettingsAsync(PlayerSettings data)
    {
        await FileManager.SaveInfoAsync(data, SettingsFilePath);
    }


    private void OnDestroy()
    {
        playerMenuInput.asset.Disable();
        playerMenuInput.action.performed -= OnOpenSettings;
    }
}