using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Time scale manager.
/// TODelete: ... ...
/// </summary>
public class TimeScaleManager : Common.SingletonT<TimeScaleManager>
{
	//发出TimeScale请求的上下文.
	public enum GameTimeScaleContext
	{
		None,
		PlayerSkill, //由玩家攻击造成的全屏减速.
		BossKilled, //Boss被杀死时造成减速.
		GamePause, //全局减速.
		BreakArmor, //破甲减速.
		PlayerVisible, //主角技能现身.

		Max,
		
	}


	public System.Action<float> OnTimeScaleChange;

	bool DebugMsg = true;

	//TimeScale上下文数据.
	public class TimeScaleContextData
	{
		public float duration = 0; //持续时间.
		public float scale = 1; //
		public GameTimeScaleContext context;
		public float beginRealTime = 0; // 起始时间.
		public System.Action<TimeScaleContextData> OnUpdate = null;
	}

	/// <summary>
	/// 当前Scale数据.
	/// </summary>
	TimeScaleContextData _currentScaleData = new TimeScaleContextData();

	public TimeScaleContextData CurrentScaleData {
		get {
			return _currentScaleData;
		}
	}

	Stack<TimeScaleContextData> _stack = new Stack<TimeScaleContextData>();
	/*
	public void Awake()
	{
		if(s_instance == null)
			s_instance = this;
	}*/
	/// <summary>
	/// 请求系统减速.
	/// </summary>
	/// <returns><c>true</c>, if time scale was reqed, <c>false</c> otherwise.</returns>
	/// <param name="scale">Scale.</param>
	/// <param name="duration">Duration.</param>
	/// <param name="context">Context.</param>
	public bool ReqTimeScale(float scale, float duration, GameTimeScaleContext context, System.Action<TimeScaleContextData> OnUpdate = null)
	{
		if(duration <= 0)
		{
			CancelTimeScale(context);
			return false;
		}

		ReqTimeScaleResult policy = CheckTimeScalePolicy(context);
//		if(_currentScaleData != null)
//			Log.info(this, "[Check Time scale] currentContexgt " + _currentScaleData.context +" context "  + context + " result " + policy);
//		else
//			Log.info(this, "[Check Time scale] currentContexgt null  context "  + context + " result " + policy);
		switch(policy )
		{
		case ReqTimeScaleResult.Override:
			 OverrideCurScale(scale, duration, context, OnUpdate);  
			break;
		case ReqTimeScaleResult.Refused:
			RefuseScale(scale, duration, context, OnUpdate);
			return false;
		case ReqTimeScaleResult.Break:
			BreakCurScale(scale, duration, context, OnUpdate);
			break;
		case ReqTimeScaleResult.Queued:
			break;
		default:
			break;
		}
		return true;
	}
	/// <summary>
	/// 延迟启动系统减速.
	/// </summary>
	/// <returns><c>true</c>, if time scale delayed was reqed, <c>false</c> otherwise.</returns>
	/// <param name="scale">Scale.</param>
	/// <param name="duration">Duration.</param>
	/// <param name="context">Context.</param>
	/// <param name="delayTime">Delay time.</param>

	public bool ReqTimeScaleDelayed(float scale, float duration, GameTimeScaleContext context, float delayTime)
	{
		if(duration <= 0)
		{
			CancelTimeScale(context);
			return false;
		}
		AppMain.Instance().StartCoroutine(DelayStart(scale, duration, context, delayTime));
		return true;
	}
	IEnumerator DelayStart(float scale, float duration, GameTimeScaleContext context,float delayed)
	{
		if(delayed > 0)
			yield return new WaitForSeconds(delayed);
		ReqTimeScale(scale, duration, context);
	}
	/// <summary>
	/// 取消当前的TimeScale.
	/// </summary>
	/// <returns><c>true</c>是否取消成功 <c>false</c>.</returns>
	/// <param name="contex">Contex.</param>
	public bool CancelTimeScale(GameTimeScaleContext contex)
	{
		if(_currentScaleData == null)
			return true;
		if(_currentScaleData.context == contex)
		{
			Time.timeScale = 1;
			if(OnTimeScaleChange != null)
				OnTimeScaleChange(1);
			_currentScaleData = null;
			return true;
		}

		return false;

	}
	/// <summary>
	/// 覆盖当前Scale.
	/// </summary>
	/// <param name="scale">Scale.</param>
	/// <param name="duration">Duration.</param>
	/// <param name="context">Context.</param>
	void OverrideCurScale(float scale, float duration, GameTimeScaleContext context, System.Action<TimeScaleContextData> OnUpdate = null)
	{
		if(_currentScaleData == null)
			_currentScaleData = new TimeScaleContextData();
		_currentScaleData.scale = scale;
		_currentScaleData.duration = duration;
		_currentScaleData.context = context;
		_currentScaleData.beginRealTime = Time.realtimeSinceStartup;
		_currentScaleData.OnUpdate = OnUpdate;
		Time.timeScale = scale;
	}


	void RefuseScale(float scale, float duration, GameTimeScaleContext context, System.Action<TimeScaleContextData> OnUpdate = null)
	{
		return ;
	}
	void BreakCurScale(float scale, float duration, GameTimeScaleContext context, System.Action<TimeScaleContextData> OnUpdate = null)
	{
		if(_currentScaleData != null)
		{
			_stack.Push(_currentScaleData);
		}

		_currentScaleData = new TimeScaleContextData()
		{
			scale = scale,
			duration = duration,
			context = context,
			OnUpdate = OnUpdate,
		};
	}

	public void Update()
	{
		if(_currentScaleData != null)
		{
			if(_currentScaleData.duration + _currentScaleData.beginRealTime > Time.realtimeSinceStartup)
			{
//				_currentScaleData.duration -= (Time.realtimeSinceStartup - _currentScaleData.beginRealTime);

				if(_currentScaleData.OnUpdate != null)
				{
					_currentScaleData.OnUpdate(_currentScaleData);
				}
				if(Time.timeScale != _currentScaleData.scale)
				{
					Time.timeScale = _currentScaleData.scale;
					if(OnTimeScaleChange != null)
						OnTimeScaleChange( _currentScaleData.scale);
				}
					
			}
			else
			{
				Time.timeScale = 1;

				if(OnTimeScaleChange != null)
					OnTimeScaleChange( 1 );
				_currentScaleData = null;
			}
		}

		if(_currentScaleData == null)
		{
			if(_stack.Count <= 0)
				return ;
			_currentScaleData = _stack.Pop();
			if(_currentScaleData == null)
				return;
			_currentScaleData.beginRealTime = Time.realtimeSinceStartup;
		}
	}

	public enum ReqTimeScaleResult
	{
		None,
		Override, //覆盖当前.
		Refused, //被拒绝.
		Break, //中断当前TimeScale，完成后再回复.
		Queued, //等待当前完成，再执行，以后在实现.
		Max,
	}
	/// <summary>
	/// Checks the time scale policy.
	/// </summary>
	/// <returns>The time scale policy.</returns>
	/// <param name="context">Context.</param>
	public ReqTimeScaleResult CheckTimeScalePolicy(GameTimeScaleContext context)
	{
		if(_currentScaleData == null)
		{
			return ReqTimeScaleResult.Override;
		}
		ReqTimeScaleResult ret = ReqTimeScaleResult.None;
		switch(_currentScaleData.context)
		{
		case GameTimeScaleContext.BossKilled:
			switch(context)
			{
			case GameTimeScaleContext.BossKilled:
				ret = ReqTimeScaleResult.Override;
				break;
			case GameTimeScaleContext.GamePause:
				ret = ReqTimeScaleResult.Break;
				break;
			case GameTimeScaleContext.PlayerSkill:
				ret = ReqTimeScaleResult.Refused;
				break;
			case GameTimeScaleContext.BreakArmor:
				ret = ReqTimeScaleResult.Refused;
				break;
			case GameTimeScaleContext.PlayerVisible:
				ret = ReqTimeScaleResult.Refused;
				break;
			default:
				ret = ReqTimeScaleResult.Override;
				break;
			}
			break;

		case GameTimeScaleContext.GamePause:
			switch(context)
			{
			case GameTimeScaleContext.BossKilled:
				ret = ReqTimeScaleResult.Refused;
				break;
			case GameTimeScaleContext.GamePause:
				ret = ReqTimeScaleResult.Override;
				break;
			case GameTimeScaleContext.PlayerSkill:
				ret = ReqTimeScaleResult.Refused;
				break;
			case GameTimeScaleContext.BreakArmor:
				ret = ReqTimeScaleResult.Refused;
				break;
			case GameTimeScaleContext.PlayerVisible:
				ret = ReqTimeScaleResult.Refused;
				break;
			default:
				ret = ReqTimeScaleResult.Override;
				break;
			}
			break;
		case GameTimeScaleContext.PlayerSkill:
			switch(context)
			{
			case GameTimeScaleContext.BossKilled:
				ret = ReqTimeScaleResult.Override;
				break;
			case GameTimeScaleContext.GamePause:
				ret = ReqTimeScaleResult.Break;
				break;
			case GameTimeScaleContext.PlayerSkill:
				ret = ReqTimeScaleResult.Override;
				break;
			case GameTimeScaleContext.BreakArmor:
				ret = ReqTimeScaleResult.Override;
				break;
			case GameTimeScaleContext.PlayerVisible:
				ret = ReqTimeScaleResult.Override;
				break;
			default:
				ret = ReqTimeScaleResult.Override;
				break;
			}
			break;
		case GameTimeScaleContext.BreakArmor:
			switch(context)
			{
			case GameTimeScaleContext.BossKilled:
				ret = ReqTimeScaleResult.Override;
				break;
			case GameTimeScaleContext.GamePause:
				ret = ReqTimeScaleResult.Break;
				break;
			case GameTimeScaleContext.PlayerSkill:
				ret = ReqTimeScaleResult.Refused;
				break;
			case GameTimeScaleContext.BreakArmor:
				ret = ReqTimeScaleResult.Override;
				break;
			case GameTimeScaleContext.PlayerVisible:
				ret = ReqTimeScaleResult.Refused;
				break;
			default:
				ret = ReqTimeScaleResult.Override;
				break;
			}
			break;
		case GameTimeScaleContext.PlayerVisible:
			switch(context)
			{
			case GameTimeScaleContext.BossKilled:
				ret = ReqTimeScaleResult.Override;
				break;
			case GameTimeScaleContext.GamePause:
				ret = ReqTimeScaleResult.Break;
				break;
			case GameTimeScaleContext.PlayerSkill:
				ret = ReqTimeScaleResult.Refused;
				break;
			case GameTimeScaleContext.BreakArmor:
				ret = ReqTimeScaleResult.Refused;
				break;
			case GameTimeScaleContext.PlayerVisible:
				ret = ReqTimeScaleResult.Override;
				break;
			default:
				ret = ReqTimeScaleResult.Override;
				break;
			}
			break;
		default:
			ret = ReqTimeScaleResult.Override;
			break;
		}
		return ret;
	}




	void OnLevelWasLoaded (int level) {
		Time.timeScale = 1;
		while(_stack.Count > 0)
			_stack.Pop();
		_currentScaleData = null;
	}

#if false
#if UNITY_EDITOR
	void OnGUI()
	{
		GUILayout.BeginVertical();
		if(GUILayout.Button("Set Game Pause"))
		{
			TimeScaleManager.Instance.ReqTimeScale(0, 100000, GameTimeScaleContext.GamePause);
		}

		if(GUILayout.Button("Cancel Game Pause"))
		{
			TimeScaleManager.Instance.ReqTimeScale(0, 0, GameTimeScaleContext.GamePause);
		}
		if(GUILayout.Button("Set Boss Killed"))
		{
			TimeScaleManager.Instance.ReqTimeScale(0.1f, 100000, GameTimeScaleContext.BossKilled);
		}
		
		if(GUILayout.Button("Cancel Boss Killed"))
		{
			TimeScaleManager.Instance.ReqTimeScale(0, 0, GameTimeScaleContext.BossKilled);
		}

		if(GUILayout.Button("Set Player skill"))
		{
			TimeScaleManager.Instance.ReqTimeScale(0.3f, 0.2f, GameTimeScaleContext.PlayerSkill);
		}
		
		if(GUILayout.Button("Cancel Player skill"))
		{
			TimeScaleManager.Instance.ReqTimeScale(0, 0, GameTimeScaleContext.PlayerSkill);
		}
		GUILayout.EndVertical();
	}
#endif
#endif
}

