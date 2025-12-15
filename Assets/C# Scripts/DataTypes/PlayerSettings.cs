


[System.Serializable]
public struct PlayerSettings
{
#pragma warning disable UDR0001
    public float MouseSensitivity;
    public float AudioMain;
    public float AudioMusic;
    public float AudioSFX;
#pragma warning restore UDR0001


    public float GetSavedData(int id)
    {
        return id switch
        {
            0 => MouseSensitivity,
            1 => AudioMain,
            2 => AudioMusic,
            3 => AudioSFX,
            _ => -1,
        };
    }
    public void SetData(int id, float value)
    {
        switch (id)
        {
            case 0:
                MouseSensitivity = value;
                break;
            case 1:
                AudioMain = value;
                break;
            case 2:
                AudioMusic = value;
                break;
            case 3:
                AudioSFX = value;
                break;
            default:
                DebugLogger.LogError("Error asigning value in MatchSettings.cs");
                break;
        }
    }
}