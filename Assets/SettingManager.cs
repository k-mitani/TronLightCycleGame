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
    public Toggle checkAutoPlayerId;
    public TMP_InputField inputPlayerId1;
    public TMP_InputField inputPlayerId2;
    public TMP_InputField inputPlayerIdNpc;
    public TMP_InputField inputNpcSpawnOnStart;
    public TMP_InputField inputNpcSpawnInterval;

    public TMP_InputField errorText;

    public Toggle toggleSpawnNpc;

    public void OnApplyButtonClick()
    {
        try
        {
            var remoteAddress = IPAddress.Parse(inputRemoteAddress.text);
            var remotePort = int.Parse(inputRemotePort.text);
            var localPort = int.Parse(inputLocalPort.text);
            var isAutoPlayerId = checkAutoPlayerId.isOn;
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
                isAutoPlayerId,
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

    public void Toggle()
    {
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
            return;
        }

        var gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        inputRemoteAddress.text = gm.udp._remote.Address.ToString();
        inputRemotePort.text = gm.udp._remote.Port.ToString();
        inputLocalPort.text = gm.udp._local.Port.ToString();
        //checkAutoPlayerId.isOn = true;
        inputPlayerId1.text = gm.myPlayerId1.ToString();
        inputPlayerId2.text = gm.myPlayerId2.ToString();
        inputPlayerIdNpc.text = gm.npcPlayerId.ToString();
        //inputNpcSpawnOnStart.text = ...;
        //inputNpcSpawnInterval.text = ...;

        gameObject.SetActive(true);
    }
}
