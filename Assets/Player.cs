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

    public Player(GameManager gm, LightCycle2D prefab, Color color)
    {
        _gm = gm;
        _prefab = prefab;
        _color = color;
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
            _cycle = GameObject.Instantiate(_prefab);
            _cycle.transform.position = d.position;
            _cycle.initialDirection = d.direction;
            _cycle.speed = d.speed;
            _cycle.isLocal = false;
            _cycle.color = _color;
            _cycle.owner = this;
        }
        // 方向転換
        else if (d.type == MessageType.Turn)
        {
            // 何らかの理由で自機が存在しないなら作る。
            if (_cycle == null)
            {
                _cycle = GameObject.Instantiate(_prefab);
                _cycle.initialDirection = d.direction;
                _cycle.isLocal = false;
                _cycle.color = _color;
                _cycle.owner = this;
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

    public void OnStartButtonPress()
    {
        // すでに自機が存在するなら何もしない。
        if (_cycle != null) return;

        _cycle = GameObject.Instantiate(_prefab);
        _cycle.transform.position = new Vector3(-6, 0, 0);
        _cycle.initialDirection = Direction.Right;
        _cycle.isLocal = true;
        _cycle.color = _color;
        _cycle.owner = this;
    }

    internal void StartNpcThread(int npcSpawnOnStart, int npcSpawnInterval)
    {
        throw new NotImplementedException();
    }

    internal void StopNpcThread()
    {
        throw new NotImplementedException();
    }

    internal void OnUpKeyDown()
    {
        if (_cycle == null) return;
        _cycle.ChangeDirection(Direction.Up);
    }

    internal void OnDownKeyDown()
    {
        if (_cycle == null) return;
        _cycle.ChangeDirection(Direction.Down);
    }

    internal void OnRightKeyDown()
    {
        if (_cycle == null) return;
        _cycle.ChangeDirection(Direction.Right);
    }

    internal void OnLeftKeyDown()
    {
        if (_cycle == null) return;
        _cycle.ChangeDirection(Direction.Left);
    }

    internal void OnLocalDead()
    {
        _cycle = null;
    }
}
