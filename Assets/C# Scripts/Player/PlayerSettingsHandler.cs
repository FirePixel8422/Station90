using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;



public class PlayerSettingsHandler : MonoBehaviour
{
    public static PlayerSettings Settings { get; private set; }


    [Header("Where is UI Parent")]
    [SerializeField] private RectTransform UITransform;
    private bool isSettingsMenuActive;

    [Header("Default settings if no save file found")]
    [SerializeField] private PlayerSettings defaultSettings;

    private const string SettingsFilePath = "SaveData/PlayerSettings";



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
                    PlayerDataLibrary.Controller.IsControlEnabled = true;

                    await SaveSettingsAsync(Settings);
                }
            }
            else
            {
                UITransform.gameObject.SetActive(true);
                isSettingsMenuActive = true;
                PlayerDataLibrary.Controller.IsControlEnabled = false;
            }
        }
    }


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
}