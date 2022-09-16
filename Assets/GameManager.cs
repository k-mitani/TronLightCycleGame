using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [NonSerialized] public long clientId = DateTime.Now.ToBinary();
    public UdpManager udp;
    private Queue<Data> queueMessage = new();
    private List<Data> queuePingResponse;

    public UIManager ui;

    public LightCycle2D playerPrefab;
    public byte myPlayerId1;
    public byte myPlayerId2;
    public byte npcPlayerId;

    public GameObject player1InputHandlerPrefab;
    public GameObject player2InputHandlerPrefab;
    public PlayerInputHandler player1InputHandler;
    public PlayerInputHandler player2InputHandler;

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

        udp = new UdpManager(clientId, IPAddress.Parse("239.239.239.1"), 30000, 30000);
        udp.Receive += Udp_Receive;
        StartCoroutine(ApplySettingForStart());


        player1InputHandler = PlayerInputHandler.Initialize(this, player1InputHandlerPrefab, 0, () => myPlayer1);
        player2InputHandler = PlayerInputHandler.Initialize(this, player2InputHandlerPrefab, 1, () => myPlayer2);
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

    public void OnSettingChange(IPAddress remoteAddress, int remotePort, int localPort, bool autoPlayerId, byte playerId1, byte playerId2, byte playerIdNpc, int npcSpawnOnStart, int npcSpawnInterval)
    {
        // 通信設定を更新する。
        var oldUdp = udp;
        oldUdp.Receive -= Udp_Receive;
        oldUdp.Dispose();
        udp = new UdpManager(clientId, remoteAddress, remotePort, localPort);
        udp.Receive += Udp_Receive;

        StartCoroutine(Do());
        IEnumerator Do()
        {
            // 他のプレーヤーとIDが衝突していないか調べる。
            if (autoPlayerId)
            {
                var ping = new Data();
                ping.clientId = clientId;
                ping.type = MessageType.Ping;
                queuePingResponse = new List<Data>();
                udp.Send(ping);
                yield return new WaitForSeconds(0.3f);
                var list = queuePingResponse.Select(d => (d.clientId, d.playerId)).ToList();
                queuePingResponse = null;

                var remainingIds = Enumerable.Range(1, 254).Select(i => (byte)i).ToList();
                list.ForEach(d => remainingIds.Remove(d.playerId));
                // プレーヤー1は古いユーザーと被っていれば違うIDにする。
                var conflicted1 = list.Any(d => d.clientId < clientId && d.playerId == playerId1);
                if (conflicted1)
                {
                    playerId1 = remainingIds.Min();
                    remainingIds.Remove(playerId1);
                }
                // プレーヤー2とNPCは5以降を割り当てる。
                remainingIds.Remove(1);
                remainingIds.Remove(2);
                remainingIds.Remove(3);
                remainingIds.Remove(4);
                var conflicted2 = remainingIds.Contains(playerId2);
                if (conflicted2)
                {
                    playerId2 = remainingIds[UnityEngine.Random.Range(0, remainingIds.Count)];
                    remainingIds.Remove(playerId2);
                }
                var conflictedNpc = remainingIds.Contains(playerIdNpc);
                if (conflictedNpc)
                {
                    playerIdNpc = remainingIds[UnityEngine.Random.Range(0, remainingIds.Count)];
                    remainingIds.Remove(playerIdNpc);
                }
            }
            // プレーヤー設定を更新する。
            myPlayerId1 = playerId1;
            myPlayerId2 = playerId2;
            // NPC設定を更新する。
            var oldNpcPlayer = npcPlayer;
            oldNpcPlayer.StopNpcThread();
            npcPlayerId = playerIdNpc;
            npcPlayer.StartNpcThread(npcSpawnOnStart, npcSpawnInterval);

            yield break;
        }
    }

    private void FixedUpdate()
    {
        // 受信データを処理する。
        while (queueMessage.TryDequeue(out var d))
        {
            if (d.type == MessageType.Ping)
            {
                d.type = MessageType.PingResponse;
                d.clientId = clientId;
                d.playerId = myPlayerId1;
                udp.Send(d);
                d.playerId = myPlayerId2;
                udp.Send(d);
                d.playerId = npcPlayerId;
                udp.Send(d);
                return;
            }
            if (d.type == MessageType.PingResponse)
            {
                if (queuePingResponse != null) queuePingResponse.Add(d);
                return;
            }

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
