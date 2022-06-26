using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnityEngine;

namespace TronLightCycleGameUdpTool
{
    public partial class Form1 : Form
    {
        private UdpClient _udp;
        private Data d = new Data();
        private DateTime _prevSentTime = DateTime.Now;

        private IPEndPoint Remote => new IPEndPoint(
            IPAddress.Parse(inputIRemotePAddress.Text),
            int.Parse(inputRemotePort.Text));
        private void SendData(Data d)
        {
            var bytes = Serialize(d);
            _udp.Send(bytes, bytes.Length, Remote);
            _prevSentTime = DateTime.Now;
        }

        private Vector3 CalculateMoveAmount(Direction dir)
        {
            var diffTime = DateTime.Now - _prevSentTime;
            var moveAmount = diffTime.TotalSeconds * d.speed;
            var vec = default(Vector3);
            switch (dir)
            {
                case Direction.Up:
                    vec = Vector3.up;
                    break;
                case Direction.Down:
                    vec = Vector3.down;
                    break;
                case Direction.Left:
                    vec = Vector3.left;
                    break;
                case Direction.Right:
                    vec = Vector3.right;
                    break;
            }
            return vec * (float)moveAmount;
        }

        public Form1()
        {
            InitializeComponent();
            _udp = new UdpClient();
            _prevSentTime = DateTime.Now;
            comboPlayerId.SelectedIndex = 1;
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            d.type = MessageType.Turn;
            d.playerId = byte.Parse(comboPlayerId.Text);
            d.position = d.position + CalculateMoveAmount(d.direction);
            d.direction = Direction.Up;
            SendData(d);
        }

        private void btnRight_Click(object sender, EventArgs e)
        {
            d.type = MessageType.Turn;
            d.playerId = byte.Parse(comboPlayerId.Text);
            d.position = d.position + CalculateMoveAmount(d.direction);
            d.direction = Direction.Right;
            SendData(d);
        }

        private void btnLeft_Click(object sender, EventArgs e)
        {
            d.type = MessageType.Turn;
            d.playerId = byte.Parse(comboPlayerId.Text);
            d.position = d.position + CalculateMoveAmount(d.direction);
            d.direction = Direction.Left;
            SendData(d);
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            d.type = MessageType.Turn;
            d.playerId = byte.Parse(comboPlayerId.Text);
            d.position = d.position + CalculateMoveAmount(d.direction);
            d.direction = Direction.Down;
            SendData(d);
        }

        private void btnSpawn_Click(object sender, EventArgs e)
        {
            d.type = MessageType.Spawn;
            d.playerId = byte.Parse(comboPlayerId.Text);
            d.direction = Direction.Left;
            d.position = new Vector3(6, -2 + 1 * d.playerId, 0);
            d.speed = 3;
            SendData(d);
        }

        private void btnDead_Click(object sender, EventArgs e)
        {
            d.type = MessageType.Dead;
            d.playerId = byte.Parse(comboPlayerId.Text);
            d.position = d.position + CalculateMoveAmount(d.direction);
            SendData(d);
        }


        public static T Deserialize<T>(byte[] bytes, int startIndex = 0) where T : struct
        {
            ref var src = ref Unsafe.As<byte, T>(ref bytes[startIndex]);
            return src;
        }
        public static byte[] Serialize<T>(T data, byte[] buffer = null, int startIndex = 0) where T : struct
        {
            var size = Marshal.SizeOf<T>();
            if (buffer == null) buffer = new byte[size];
            ref var dst = ref Unsafe.As<byte, T>(ref buffer[startIndex]);
            dst = data;
            return buffer;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Data
    {
        public MessageType type;
        public byte sessionId;
        public byte playerId;
        public Direction direction;
        public Vector3 position;
        public float speed;
    }

    public enum MessageType : byte
    {
        Spawn,
        Turn,
        Dead,
    }

    public enum Direction : byte
    {
        None,
        Up,
        Down,
        Left,
        Right,
    }
}
