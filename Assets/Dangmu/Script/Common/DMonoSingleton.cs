using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// D mono singleton.示例不会释放》。。
/// </summary>
public class DMonoSingleton<T>: MonoBehaviour where T : DMonoSingleton<T>
{
	protected static T instance = null;

	public static T Instance()
	{
		if (instance == null)
		{
			instance = FindObjectOfType<T>();

			if (FindObjectsOfType<T>().Length > 1)
			{
				//Debug.LogError ("More than 1!");
				Log.error ("DMonoSingleton","More than 1!");
				return instance;
			}

			if (instance == null)
			{
				string instanceName = typeof(T).Name;
				Debug.Log ("Instance Name: " + instanceName); 
				GameObject instanceGO = GameObject.Find(instanceName);

				if (instanceGO == null)
					instanceGO = new GameObject(instanceName);
				instance = instanceGO.AddComponent<T>();
				DontDestroyOnLoad(instanceGO);  //保证实例不会被释放
				Debug.Log ("Add New Singleton " + instance.name + " in Game!");
			}
			else
			{
				//Debug.Log ("Already exist: " + instance.name);
			}
		}

		return instance;
	}


	protected virtual void OnDestroy()
	{
		instance = null;
	}
}

