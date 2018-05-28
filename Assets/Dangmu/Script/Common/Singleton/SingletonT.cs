using System;

namespace Common
{
	/// <summary>
	/// Singleton t.For no monobehavior
	/// </summary>
	public class SingletonT<T> where T : new()
	{
		//声明初始化...
		private static T instance = (default(T) == null) ? Activator.CreateInstance<T>() : default(T);
		public static T Instance
		{
			get
			{
				return SingletonT<T>.instance;
			}
		}
		protected SingletonT()
		{
		}
	}
}