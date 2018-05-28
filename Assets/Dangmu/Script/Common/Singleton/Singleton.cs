using UnityEngine;
//y一个简单的单例模板，使用时注意，对于在场景里面创建好的Manager，最好在Awake的时候执行如下语句
//if(s_instance == null) s_instance = this;
//这样可以避免FindObjectsOfType，稍微提高一点性能
namespace Common
{
	public class Singleton<T> : MonoBehaviour where T : Singleton<T>
	{
		public static T Instance
		{
			get
			{
				if (s_instance == null)
				{
					T[] managers = Object.FindObjectsOfType(typeof(T)) as T[];
					if (managers.Length != 0)
					{
						if (managers.Length == 1)
						{
							s_instance = managers[0];
							s_instance.gameObject.name = typeof(T).Name;
							DontDestroyOnLoad(s_instance.gameObject);
							return s_instance;
						}
						else
						{
//							Debug.LogError("Class " + typeof(T).Name + " exists multiple times in violation of singleton pattern. Please check it out");
							/* 只做警告，做破坏性处理
							foreach (T manager in managers)
							{
								Destroy(manager.gameObject);
							}
							*/
						}
					}
//					Debug.LogError("[________________!!!!!!!!!!!!!s_instance0________"+s_instance+" managers"+ managers.Length + " " + typeof(T).Name); 
//					var go = new GameObject(typeof(T).Name, typeof(T));
//					s_instance = go.GetComponent<T>();
//					DontDestroyOnLoad(go);
				}
				return s_instance;
			}
			set
			{
				s_instance = value as T;
			}
		}
		protected static T s_instance;
	}
}
