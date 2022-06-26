using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private UdpManager udp;
    private Queue<Data> queueMessage = new Queue<Data>();

    public LightCycle2D playerPrefab;
    public byte myPlayerId;

    private Player[] players;
    private Player myPlayer
    {
        get => players[myPlayerId];
        set => players[myPlayerId] = value;
    }

    // Start is called before the first frame update
    void Start()
    {
        players = new Player[byte.MaxValue + 1];
        for (int i = 0; i < players.Length; i++)
        {
            var color = Color.cyan;
            if (i == 1) color = new Color(1, 0.65f, 0); // オレンジ
            if (i == 2) color = new Color(0, 0.3f, 1); // 青
            if (i == 3) color = new Color(1, 1, 0); // 黄
            if (i == 4) color = new Color(1, 0, 0); // 赤
            players[i] = new Player(this, playerPrefab, color);
        }

        udp = new UdpManager(30000);
        udp.Receive += Udp_Receive;
    }

    private void Udp_Receive(object sender, Data d)
    {
        Debug.Log("受信");
        queueMessage.Enqueue(d);
    }

    void Update()
    {
        // ローカルの操作をPlayerオブジェクトに伝える。
        if (Input.GetKey(KeyCode.S)) myPlayer.OnStartButtonPress();
        else if (Input.GetKeyDown(KeyCode.UpArrow)) myPlayer.OnUpKeyDown();
        else if (Input.GetKeyDown(KeyCode.DownArrow)) myPlayer.OnDownKeyDown();
        else if (Input.GetKeyDown(KeyCode.RightArrow)) myPlayer.OnRightKeyDown();
        else if (Input.GetKeyDown(KeyCode.LeftArrow)) myPlayer.OnLeftKeyDown();
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
