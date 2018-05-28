#region License
/*
 * Decoder.cs
 *
 * The MIT License
 *
* Copyright (c) 2016 Kennyzhu
 * For China mobile hongbao interface.
 */
#endregion

//#define SOCKET_IO_DEBUG			// Uncomment this for debug
using System;
using System.Collections;
using System.Text;
using UnityEngine;

namespace HttpIO
{
	public class Decoder
	{
		//TODO: modify .....
		//直接对象序列化,找第一个对象名字.....
		public Packet Decode(WebSocketSharp.MessageEventArgs e)
		{
			try
			{
				#if SOCKET_IO_DEBUG
				Debug.Log("[SocketIO] Decoding: " + e.Data);
				#endif

				string data = e.Data;
				Packet packet = new Packet();
				int offset = 0;

				// look up json data
				if (++offset < data.Length - 1) {
					try {
						#if SOCKET_IO_DEBUG
						Debug.Log("[SocketIO] Parsing JSON: " + data.Substring(offset));
						#endif
						packet.json = new JSONObject(data.Substring(offset));
					} catch (Exception ex) {
						Debug.LogException(ex);
					}
				}

				#if SOCKET_IO_DEBUG
				Debug.Log("[SocketIO] Decoded: " + packet);
				#endif

				return packet;

			} catch(Exception ex) {
				throw new SocketIOException("Packet decoding failed: " + e.Data ,ex);
			}
		}
	}
}
