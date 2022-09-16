using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public UdpManager udp;
    private Queue<Data> queueMessage = new Queue<Data>();

    public UIManager ui;

    public LightCycle2D playerPrefab;
    public byte myPlayerId1;
    public byte myPlayerId2;
    public byte npcPlayerId;

    public PlayerInputHandler player1Input;
    public PlayerInputHandler player2Input;

    private Player[] players;
    private Player myPlayer1 => players[myPlayerId1];
    private Player myPlayer2 => players[myPlayerId2];
    private Player npcPlayer => players[npcPlayerId];

    private SynchronizationContext _context;
    public void Invoke(SendOrPostCallback action) => _context.Send(action, null);

    // Start is called before the first frame update
    void Start()
    {
        _context = SynchronizationContext.Current;
        players = new Player[byte.MaxValue + 1];
        for (int i = 0; i < players.Length; i++)
        {
            var color = Color.cyan;
            if (i == 1) color = new Color(1, 0.65f, 0); // オレンジ
            if (i == 2) color = new Color(0, 0.3f, 1); // 青
            if (i == 3) color = new Color(1, 0, 0); // 赤
            if (i == 4) color = new Color(0, 1, 0); // 緑
            players[i] = new Player(this, (byte)i, playerPrefab, color);
        }

        udp = new UdpManager(IPAddress.Parse("192.168.10.1"), 30000, 30000);
        udp.Receive += Udp_Receive;
        StartCoroutine(ApplySettingForStart());

        player1Input.Initialize(this, myPlayer1);
        player2Input.Initialize(this, myPlayer2);
    }

    IEnumerator ApplySettingForStart()
    {
        yield return null;
        ui.settings.OnApplyButtonClick();
    }


    private void Udp_Receive(object sender, Data d)
    {
        queueMessage.Enqueue(d);
    }

    public void OnSettingChange(IPAddress remoteAddress, int remotePort, int localPort, bool isEndless, byte playerId1, byte playerId2, byte playerIdNpc, int npcSpawnOnStart, int npcSpawnInterval)
    {
        // 通信設定を更新する。
        var oldUdp = udp;
        oldUdp.Receive -= Udp_Receive;
        oldUdp.Dispose();
        udp = new UdpManager(remoteAddress, remotePort, localPort);
        udp.Receive += Udp_Receive;

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
        npcPlayer.StopNpcThread();
    }
}
