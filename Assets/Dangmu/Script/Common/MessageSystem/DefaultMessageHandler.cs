using System;
using System.Collections.Generic;

namespace Common
{
	public class DefaultMessageHandler : IMessageHandler {
		Dictionary<int, Action<IMessage> > _messageMap = new Dictionary<int, Action<IMessage> >();
		Queue<IMessage> _messageQueue = new Queue<IMessage>(); //运行列表.
		//List<IMessage> _tempMessageList = new List<IMessage>(); //临时列表.
		public virtual bool AddMessageHandler(int id, Action<IMessage> h)
		{
			if(_messageMap.ContainsKey(id) == true)
			{
				_messageMap[id] += h;
			}
			else
			{
				_messageMap.Add(id, h);
			}
			return true;
		}

		public virtual bool RemoveMessageHandler(int id, Action<IMessage> h)
		{
			if(_messageMap.ContainsKey(id) == true)
			{
				_messageMap[id] -= h;
				if(_messageMap[id] == null)
				{
					_messageMap.Remove(id);
				}
			}
			return true;
		}

		public virtual void PushMessage(IMessage msg)
		{
			//_tempMessageList.Add(msg);
			_messageQueue.Enqueue(msg);

		}

		/// <summary>
		/// Dispatch this instance.
		/// </summary>
		public virtual void Update()
		{
			//one message a time?...
			if(_messageQueue.Count > 0)
			{
				IMessage msg = _messageQueue.Dequeue ();
				{
					if( _messageMap.ContainsKey(msg.id) == false )
						return;
					
					Action<IMessage> h = _messageMap[msg.id];
					h.Invoke(msg);
				}
			}
		}

		//TODO: add dispatch..
	}
}
