using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    public TMP_InputField inputRemoteAddress;
    public TMP_InputField inputRemotePort;
    public TMP_InputField inputLocalPort;
    public Toggle checkEndless;
    public TMP_InputField inputPlayerId1;
    public TMP_InputField inputPlayerId2;
    public TMP_InputField inputPlayerIdNpc;
    public TMP_InputField inputNpcSpawnOnStart;
    public TMP_InputField inputNpcSpawnInterval;

    public TMP_InputField errorText;

    public void OnApplyButtonClick()
    {
        try
        {
            var remoteAddress = IPAddress.Parse(inputRemoteAddress.text);
            var remotePort = int.Parse(inputRemotePort.text);
            var localPort = int.Parse(inputLocalPort.text);
            var isEndless = checkEndless.isOn;
            var playerId1 = byte.Parse(inputPlayerId1.text);
            var playerId2 = byte.Parse(inputPlayerId2.text);
            var playerIdNpc = byte.Parse(inputPlayerIdNpc.text);
            var npcSpawnOnStart = int.Parse(inputNpcSpawnOnStart.text);
            var npcSpawnInterval = int.Parse(inputNpcSpawnInterval.text);
            var gm = GameObject.Find("GameManager").GetComponent<GameManager>();
            gm.OnSettingChange(
                remoteAddress,
                remotePort,
                localPort,
                isEndless,
                playerId1,
                playerId2,
                playerIdNpc,
                npcSpawnOnStart,
                npcSpawnInterval);
            errorText.text = "Applied at " + System.DateTime.Now.ToString("mm:ss");
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex);
            errorText.text = ex.ToString();
        }
    }
}
