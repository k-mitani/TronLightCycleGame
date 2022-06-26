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

public class UdpManager : IDisposable
{
	private Thread _thread;
	private UdpClient _udp;
	private IPEndPoint _local;

	public event EventHandler<Data> Receive;

	public UdpManager(int localPort)
    {
		var address = "0.0.0.0";
		_local = new IPEndPoint(IPAddress.Parse(address), localPort);
		_udp = new UdpClient(localPort);
		_thread = new Thread(DoLoop);
		_thread.Start();
	}

	private void DoLoop()
    {
        while (true)
        {
			var bytes = _udp.Receive(ref _local);
			var d = Deserialize<Data>(bytes);
			Receive?.Invoke(this, d);
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
