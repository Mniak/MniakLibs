﻿using System;
using System.Threading;
using EasySocket.Filters;
using System.Collections.Generic;
using EasySocket.Wrappers;
using System.Net.Sockets;

namespace EasySocket
{
	public class HeaderedWrapper : FilterableWrapper
	{
		private int headerLength = 4;
		public int HeaderLength
		{
			get { return headerLength; }
			set
			{
				if (value < 1 || value > 4)
					throw new ArgumentOutOfRangeException("The value of HeaderLength must be in the range of 1 to 4.");
				headerLength = value;
			}
		}

		protected override void InnerSend(byte[] bytes)
		{
			bytes = ProcessFiltersSend(bytes);
			byte[] header = bytes.GetHeader(HeaderLength);
			byte[] all = header.Union(bytes);
			//FIX: Quando dá queda de conexão não está tratando exception
			socket.Send(all);
		}
		protected override byte[] Receive()
		{
			ulong length = ReadHeader();
			byte[] bytes = ProcessFiltersReceive(ReadBody(length));
			return bytes;
		}

		private ulong ReadHeader()
		{
			byte[] header = new byte[HeaderLength];
			while (running
					&& !socket.Poll(10, SelectMode.SelectRead) || socket.Available > 0
					&& socket.Available < HeaderLength)
			{
				Thread.Sleep(100);
			}

			// If it is not running then abort
			if (!running)
				return 0;
			int n = socket.Receive(header);

			if (n < HeaderLength)
				return 0;
			ulong tamanho = header.AsInteger();

			return tamanho;
		}
		private byte[] ReadBody(ulong length)
		{
			// If it is not running then abort
			if (!running || length < 1)
				return null;

			// Wait for all bytes available
			Helper.TimeoutLoop(() => running && (ulong)socket.Available < length, 5000);

			// If it is not running then abort
			if (!running)
				return null;

			byte[] buffer = new byte[length];
			int n = socket.Receive(buffer);
			if (n < HeaderLength)
				return null;

			return buffer;
		}
	}
}
