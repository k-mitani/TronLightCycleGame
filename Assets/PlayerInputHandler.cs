using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private GameManager gm;
    private Player player;
    public void Initialize(GameManager gm, Player player)
    {
        this.gm = gm;
        this.player = player;
    }

    public void OnSpawn()
    {
        player.OnStartButtonPress();
    }
    public void OnUp()
    {
        player.OnUpKeyDown();
    }
    public void OnDown()
    {
        player.OnDownKeyDown();
    }
    public void OnLeft()
    {
        player.OnLeftKeyDown();
    }
    public void OnRight()
    {
        player.OnRightKeyDown();
    }
    public void OnEscape()
    {
        gm.ui.settings.gameObject.SetActive(!gm.ui.settings.gameObject.activeSelf);
    }
}
