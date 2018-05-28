#region License
/*
 * Packet.cs
 *
 * The MIT License
 *
 * Copyright (c) 2016 Kennyzhu
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * For China mobile hongbao interface.
 */
#endregion

namespace HttpIO
{
	//TODO: get seirizlized..
	//Nothing need to do more..
	public class Packet
	{
		//public string nsp;
		//public int id;
		public JSONObject json;

		public Packet() : this(null) { }

		public Packet(JSONObject json)
		{
			//this.nsp = nsp;
			//this.id = id;
			this.json = json;
		}

		public override string ToString()
		{
			//return string.Format("[Packet: enginePacketType={0}, socketPacketType={1}, attachments={2}, nsp={3}, id={4}, json={5}]", enginePacketType, socketPacketType, attachments, nsp, id, json);
			return string.Format("[Packet: json={0}]", json);
		}
	}
}
