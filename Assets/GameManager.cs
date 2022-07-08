using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private UdpManager udp;
    private Queue<Data> queueMessage = new Queue<Data>();

    public UIManager ui;

    public LightCycle2D playerPrefab;
    public byte myPlayerId1;
    public byte myPlayerId2;
    public byte npcPlayerId;

    private Player[] players;
    private Player myPlayer1 => players[myPlayerId1];
    private Player myPlayer2 => players[myPlayerId2];
    private Player npcPlayer => players[npcPlayerId];

    // Start is called before the first frame update
    void Start()
    {
        players = new Player[byte.MaxValue + 1];
        for (int i = 0; i < players.Length; i++)
        {
            var color = Color.cyan;
            if (i == 1) color = new Color(1, 0.65f, 0); // オレンジ
            if (i == 2) color = new Color(0, 0.3f, 1); // 青
            if (i == 3) color = new Color(1, 0, 0); // 赤
            if (i == 4) color = new Color(0, 1, 0); // 緑
            players[i] = new Player(this, playerPrefab, color);
        }

        udp = new UdpManager(IPAddress.Parse("192.168.10.1"), 30000, 30000);
        udp.Receive += Udp_Receive;
    }

    private void Udp_Receive(object sender, Data d)
    {
        queueMessage.Enqueue(d);
    }

    public void OnSettingChange(IPAddress remoteAddress, int remotePort, int localPort, bool isEndless, byte playerId1, byte playerId2, byte playerIdNpc, int npcSpawnOnStart, int npcSpawnInterval)
    {
        // 通信設定を更新する。
        var oldUdp = udp;
        udp = new UdpManager(remoteAddress, remotePort, localPort);
        udp.Receive += Udp_Receive;
        oldUdp.Receive -= Udp_Receive;
        oldUdp.Dispose();

        // プレーヤー設定を更新する。
        myPlayerId1 = playerId1;
        myPlayerId2 = playerId2;
        // NPC設定を更新する。
        var oldNpcPlayer = npcPlayer;
        oldNpcPlayer.StopNpcThread();
        npcPlayerId = playerIdNpc;
        npcPlayer.StartNpcThread(npcSpawnOnStart, npcSpawnInterval);

        // TODO endless
    }

    void Update()
    {
        // ローカルの操作をPlayerオブジェクトに伝える。
        if (Input.GetKeyDown(KeyCode.Return)) myPlayer1.OnStartButtonPress();
        else if (Input.GetKeyDown(KeyCode.UpArrow)) myPlayer1.OnUpKeyDown();
        else if (Input.GetKeyDown(KeyCode.DownArrow)) myPlayer1.OnDownKeyDown();
        else if (Input.GetKeyDown(KeyCode.RightArrow)) myPlayer1.OnRightKeyDown();
        else if (Input.GetKeyDown(KeyCode.LeftArrow)) myPlayer1.OnLeftKeyDown();
        if (Input.GetKeyDown(KeyCode.Tab)) myPlayer2.OnStartButtonPress();
        else if (Input.GetKeyDown(KeyCode.W)) myPlayer2.OnUpKeyDown();
        else if (Input.GetKeyDown(KeyCode.S)) myPlayer2.OnDownKeyDown();
        else if (Input.GetKeyDown(KeyCode.D)) myPlayer2.OnRightKeyDown();
        else if (Input.GetKeyDown(KeyCode.A)) myPlayer2.OnLeftKeyDown();

        // ESCなら設定画面を開く。
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ui.gameObject.SetActive(!ui.gameObject.activeSelf);
        }
    }

    private void FixedUpdate()
    {
        // 受信データを処理する。
        while (queueMessage.TryDequeue(out var d))
        {
            var p = players[d.playerId];
            p.OnReceive(d);
        }
    }

    private void OnDestroy()
    {
        udp.Dispose();
    }
}
