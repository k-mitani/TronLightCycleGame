using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class PlayerInputHandler : MonoBehaviour
{
    private GameManager gm;
    private Func<Player> player;
    public static PlayerInputHandler Initialize(GameManager gm, GameObject prefab, int playerId, Func<Player> player)
    {
        var playerInput = PlayerInput.Instantiate(prefab);
        playerInput.TryGetComponent(out PlayerInputHandler handler);
        
        // まず全部のデバイスとのペアリングを切る。
        playerInput.user.UnpairDevices();
        
        // 接続されているデバイスとペアリングする。
        var devices = InputSystem.devices;
        var foundGamepadCount = 0;
        for (int i = 0; i < devices.Count; i++)
        {
            var device = devices[i];
            // キーボードはすべてのユーザーとペアリングする。
            var shouldConnect = false;
            if (device is Keyboard) shouldConnect = true;
            // マウスはユーザー0とのみペアリングする。
            else if (device is Mouse && playerId == 0) shouldConnect = true;
            else if (device is Gamepad)
            {
                // OnScreenならユーザー0にペアリングする。
                var isOnScreen = device.usages.Any(x => x.ToString().Equals("OnScreen"));
                if (isOnScreen)
                {
                    if (playerId == 0) shouldConnect = true;
                    // ゲームパッドにはカウントしない。
                }
                // 普通のゲームパッドならユーザー0から順番に一人づつペアリングする。
                else
                {
                    if (playerId == foundGamepadCount)
                    {
                        shouldConnect = true;
                    }
                    foundGamepadCount++;
                }
            }
            if (shouldConnect)
            {
                InputUser.PerformPairingWithDevice(device, playerInput.user);
            }
        }

        handler.gm = gm;
        handler.player = player;
        return handler;
    }

    public void OnSpawn()
    {
        player().OnStartButtonPress();
    }
    public void OnUp()
    {
        player().OnUpKeyDown();
    }
    public void OnDown()
    {
        player().OnDownKeyDown();
    }
    public void OnLeft()
    {
        player().OnLeftKeyDown();
    }
    public void OnRight()
    {
        player().OnRightKeyDown();
    }
    public void OnEscape()
    {
        gm.ui.settings.Toggle();
    }
}
