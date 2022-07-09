using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class Player
{
    private GameManager _gm;
    private LightCycle2D _prefab;
    private Color _color;
    private LightCycle2D _cycle;

    private byte playerId;

    public Player(GameManager gm, byte playerId, LightCycle2D prefab, Color color)
    {
        _gm = gm;
        _prefab = prefab;
        _color = color;

        this.playerId = playerId;
    }

    public void OnReceive(Data d)
    {
        // 生成
        if (d.type == MessageType.Spawn)
        {
            // 何らかの理由で前の自機が残っていたら削除する。
            if (_cycle != null)
            {
                _cycle.Abort();
                _cycle = null;
            }
            _cycle = InstantiateCycle(d.direction, false);
            _cycle.transform.position = d.position;
            _cycle.speed = d.speed;
        }
        // 方向転換
        else if (d.type == MessageType.Turn)
        {
            // 何らかの理由で自機が存在しないなら作る。
            if (_cycle == null)
            {
                _cycle = InstantiateCycle(d.direction, false);
            }
            _cycle.transform.position = d.position;
            _cycle.speed = d.speed;
            _cycle.ChangeDirection(d.direction);
        }
        // 
        else if (d.type == MessageType.Dead)
        {
            if (_cycle == null) return;
            _cycle.OnDead();
            _cycle = null;
        }
    }

    private LightCycle2D InstantiateCycle(Direction d, bool isLocal)
    {
        var c = GameObject.Instantiate(_prefab);
        c.initialDirection = d;
        c.isLocal = isLocal;
        c.color = _color;
        c.owner = this;
        return c;
    }

    private Thread _threadNpc;
    private AutoResetEvent _signalLocalStart = new AutoResetEvent(false);
    private int _npcSpawnOnStart;
    private int _npcSpawnInterval;
    internal void StartNpcThread(int npcSpawnOnStart, int npcSpawnInterval)
    {
        _npcSpawnOnStart = npcSpawnOnStart;
        _npcSpawnInterval = npcSpawnInterval;
        _threadNpc = new Thread(DoNpcLoop);
        _threadNpc.Start();
    }

    public void OnPcStart()
    {
        if (_npcSpawnOnStart > 0)
        {
            _gm.StartCoroutine(NotifyPcStart());
        }
    }
    public System.Collections.IEnumerator NotifyPcStart()
    {
        yield return new WaitForSeconds(_npcSpawnOnStart / 1000f);
        _signalLocalStart.Set();
    }

    private void DoNpcLoop()
    {
        while (true)
        {
            try
            {
                _signalLocalStart.Reset();
                if (_npcSpawnInterval != 0)
                {
                    // プレーヤーのSpawn通知が来るか、生成インターバル終了まで待機する。
                    _signalLocalStart.WaitOne(_npcSpawnInterval);
                }
                else
                {
                    _signalLocalStart.WaitOne();
                }

                // 自機を生成する。
                _gm.Invoke(_ =>
                {
                    OnStartButtonPress();
                });

                // Deadになるまで適当に移動する。
                while (_cycle != null)
                {
                    var randomWait = 0;
                    _gm.Invoke(_ => randomWait = Random.Range(200, 300));

                    Thread.Sleep(randomWait);
                    if (_cycle == null) break;
                    _gm.Invoke(_ =>
                    {
                        // 適当な方向に方向転換する。
                        var dir = Random.Range(0, 4);
                        if (dir == 0 && _cycle.currentDirection != Direction.Up) OnUpKeyDown();
                        else if (dir == 1 && _cycle.currentDirection != Direction.Down) OnDownKeyDown();
                        else if (dir == 2 && _cycle.currentDirection != Direction.Right) OnRightKeyDown();
                        else if (dir == 3 && _cycle.currentDirection != Direction.Left) OnLeftKeyDown();
                    });
                }
            }
            catch (System.Exception ex)
            {
                if (ex is ThreadAbortException)
                {
                    // 自機があれば消す。
                    if (_cycle != null)
                    {
                        _gm.Invoke(_ =>
                        {
                            _cycle.OnDead();
                            OnLocalDead();
                        });
                    }
                    Debug.Log("NpcThread Aborted");
                    return;
                }
                Debug.Log("NpcThread Error: " + ex.ToString());
            }
        }
    }

    internal void StopNpcThread()
    {
        if (_threadNpc != null) _threadNpc.Abort();
    }

    private Data GetCurrentData(MessageType t)
    {
        var d = new Data();
        d.type = t;
        d.playerId = playerId;
        d.direction = _cycle.currentDirection;
        d.position = _cycle.transform.position;
        d.speed = _cycle.speed;
        return d;
    }

    public void OnStartButtonPress()
    {
        // すでに自機が存在するなら何もしない。
        if (_cycle != null) return;

        // プレーヤー1234は決め打ち。その他はランダム
        var (position, direction) =
            playerId == 1 ? (new Vector3(-6, +3, 0), Direction.Right) :
            playerId == 2 ? (new Vector3(+6, -3, 0), Direction.Left) :
            playerId == 3 ? (new Vector3(-6, -3, 0), Direction.Up) :
            playerId == 4 ? (new Vector3(+6, +3, 0), Direction.Down) :
            (new Vector3(Random.Range(-5f, +5f), Random.Range(-2f, +2f), 0),
            (Direction)Random.Range(1, 5));
        _cycle = InstantiateCycle(direction, true);
        _cycle.transform.position = position;

        Data d = GetCurrentData(MessageType.Spawn);
        d.direction = direction;
        _gm.udp.Send(d);
    }

    internal void OnUpKeyDown()
    {
        if (_cycle == null) return;
        if (_cycle.currentDirection == Direction.Down) return;
        _cycle.ChangeDirection(Direction.Up);

        Data d = GetCurrentData(MessageType.Turn);
        _gm.udp.Send(d);
    }

    internal void OnDownKeyDown()
    {
        if (_cycle == null) return;
        if (_cycle.currentDirection == Direction.Up) return;
        _cycle.ChangeDirection(Direction.Down);

        Data d = GetCurrentData(MessageType.Turn);
        _gm.udp.Send(d);
    }

    internal void OnRightKeyDown()
    {
        if (_cycle == null) return;
        if (_cycle.currentDirection == Direction.Left) return;
        _cycle.ChangeDirection(Direction.Right);

        Data d = GetCurrentData(MessageType.Turn);
        _gm.udp.Send(d);
    }

    internal void OnLeftKeyDown()
    {
        if (_cycle == null) return;
        if (_cycle.currentDirection == Direction.Right) return;
        _cycle.ChangeDirection(Direction.Left);

        Data d = GetCurrentData(MessageType.Turn);
        _gm.udp.Send(d);
    }

    internal void OnLocalDead()
    {
        Data d = GetCurrentData(MessageType.Dead);
        _gm.udp.Send(d);
        _cycle = null;
    }
}
