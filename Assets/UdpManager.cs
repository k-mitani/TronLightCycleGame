using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct Data
{
	public long clientId;
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
	Ping,
}

public enum Direction : byte
{
	None,
	Up,
	Down,
	Left,
	Right,
}

public class UdpManager : IDisposable
{
	private long _clientId;
	private Thread _thread;
	private UdpClient _udp;
	private IPEndPoint _local;
	private IPEndPoint _remote;

	public event EventHandler<Data> Receive;

	public UdpManager(long clientId, IPAddress remoteAddress, int remotePort, int localPort)
    {
		_clientId = clientId;
		_local = new IPEndPoint(IPAddress.Parse("0.0.0.0"), localPort);
		_remote = new IPEndPoint(remoteAddress, remotePort);
		_udp = new UdpClient(localPort);
		_udp.JoinMulticastGroup(remoteAddress);
		_thread = new Thread(DoLoop);
		_thread.Start();
	}

	private byte[] _buff = new byte[4096];
	private int _dataSize = Marshal.SizeOf<Data>();
	public void Send(Data d)
    {
		Serialize(d, _buff);
		_udp.Send(_buff, _dataSize, _remote);
    }

	private void DoLoop()
    {
        while (true)
        {
            try
            {
				var bytes = _udp.Receive(ref _local);
				var d = Deserialize<Data>(bytes);
				if (d.clientId != _clientId)
				{
					Receive?.Invoke(this, d);
				}
			}
			catch (Exception ex)
            {
				if (ex is ThreadAbortException) return;
				Debug.Log(ex.Message);
				Thread.Sleep(100);
            }
		}
    }

	public static unsafe T Deserialize<T>(byte[] bytes, int startIndex = 0) where T : struct
	{
		fixed (byte* ptr = &bytes[startIndex])
		{
			return Marshal.PtrToStructure<T>((IntPtr)ptr);
		}
	}

	public static byte[] Serialize<T>(T data, byte[] buffer = null, int startIndex = 0) where T : struct
	{
		var size = Marshal.SizeOf<T>();
		if (buffer == null) buffer = new byte[size];
		IntPtr ptr = Marshal.AllocHGlobal(size);
		Marshal.StructureToPtr(data, ptr, true);
		Marshal.Copy(ptr, buffer, startIndex, size);
		Marshal.FreeHGlobal(ptr);
		return buffer;
	}

    public void Dispose()
    {
		_thread.Abort();
		_udp.Dispose();
    }
}
