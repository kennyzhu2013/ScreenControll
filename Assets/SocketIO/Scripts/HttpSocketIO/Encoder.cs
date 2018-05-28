#region License
/*
 * Encoder.cs
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
	//json object,...
	public class Encoder
	{
		public string Encode(Packet packet)
		{
			try
			{
				#if SOCKET_IO_DEBUG
				Debug.Log("[SocketIO] Encoding: " + packet.json);
				#endif

				StringBuilder builder = new StringBuilder();

				// first is type
				if (packet.json != null && !packet.json.ToString().Equals("null")) {
					builder.Append(packet.json.ToString());
				}

				#if SOCKET_IO_DEBUG
				Debug.Log("[SocketIO] Encoded: " + builder);
				#endif
				return builder.ToString();
			
			} catch(Exception ex) {
				throw new SocketIOException("Packet encoding failed: " + packet ,ex);
			}
		}
	}
}
