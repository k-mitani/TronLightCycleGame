using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

    internal void StartNpcThread(int npcSpawnOnStart, int npcSpawnInterval)
    {
        throw new NotImplementedException();
    }

    internal void StopNpcThread()
    {
        throw new NotImplementedException();
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

        var initialDirection = Direction.Right;
        _cycle = InstantiateCycle(initialDirection, true);
        _cycle.transform.position = new Vector3(-6, 0, 0);

        Data d = GetCurrentData(MessageType.Spawn);
        d.direction = initialDirection;
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
