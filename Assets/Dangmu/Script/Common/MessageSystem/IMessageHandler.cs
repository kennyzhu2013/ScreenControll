using System;

namespace Common
{
	//public delegate void MessageHandle(IMessage message);
	public interface IMessageHandler {
		bool AddMessageHandler(int id, Action<IMessage> h);//增加消息处理器，建立映射关系
		bool RemoveMessageHandler(int id, Action<IMessage> h);//删除消息处理器，解除映射关系
		void PushMessage(IMessage msg);//发送一个消息
		void Update();//派发消息
	}
}

