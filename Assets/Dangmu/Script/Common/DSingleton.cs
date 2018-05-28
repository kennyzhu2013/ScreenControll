using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

public class DSingleton<T> where T : DSingleton<T> {

	protected static T instance = null;

	protected DSingleton()
	{
	}

	public static T Instance()
	{
		if (instance == null)
		{
			// 先获取所有非public的构造方法...
			ConstructorInfo[] ctors = typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);

			// 从ctors中获取无参的构造方法...
			ConstructorInfo ctor = Array.Find(ctors, c => c.GetParameters().Length == 0);

			if (ctor == null)
				throw new Exception("Non-public ctor() not found!");
			
			// 调用构造方法...
			instance = ctor.Invoke(null) as T;
		}

		return instance;
	}
}
