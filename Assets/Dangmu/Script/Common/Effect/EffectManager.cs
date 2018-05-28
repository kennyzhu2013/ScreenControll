using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Effect manager. 必须单独一个管理节点...
/// And effect resources manage..
/// </summary>
public class EffectManager : DMonoSingleton<EffectManager> {
	//这个是纯资源的prefab列表，上面不挂EffectFx脚本...
	[SerializeField]
	Dictionary<string, GameObject> _effectPrefabList = new Dictionary<string, GameObject>();

	[SerializeField]
	List<EffectFx> _playingEffect = new List<EffectFx>();//运行中特效列表.

	void Awake()
	{
		if(instance == null)
			instance  = this;
		//DkColorLight.newShader = Shader.Find("Dk/DkColorLight");
	}

	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {
		CleanExpiredEffect();
	}

	public bool existEffectOrNot(string key){
		if(_effectPrefabList.ContainsKey(key))
			return true;
		return false;
	}

	int _freq = 20;

	//清理过期的特效.
	void CleanExpiredEffect()
	{
		_freq --;
		if(_freq > 0)
			return ;
		_freq = 20;

		List<EffectFx> delList = new List<EffectFx>();

		//扫描过期特效.
		foreach(EffectFx fx in _playingEffect)
		{
			if(fx == null)
			{
				Log.error(this,"[EffectManger][CleanExpiredEffect] Found null effect, Count " + _playingEffect.Count);
				continue;
			}

			//
			float delta = Time.deltaTime * 20;
			fx.life -= delta;
			if(fx.life <= -delta)
			{
				delList.Add(fx);
			}
		}

		//从缓存列表中清除.
		foreach(EffectFx fx in delList)
		{
			_playingEffect.Remove(fx);
		}

		//销毁特效.
		foreach(EffectFx fx in delList)
		{
			Log.info(this,"[EffectManger][CleanExpiredEffect]  Destory effect " + fx.bodyObject.name);
			Destroy(fx.gameObject);
		}
		delList.Clear();
	}


	//缓存特效Prefab.
	public void AddEffectPrefab(string prefabName, GameObject effect)
	{
		Log.info(this,"[EffectManager][AddEffectPrefab] assetBundleName " + prefabName + " prefab " + effect);
		if(_effectPrefabList.ContainsKey(prefabName))
		{
			Log.error(this,"[EffectManger][AddEffectPrefab] " + prefabName + " already exists");
			return ;
		}
		_effectPrefabList.Add(prefabName, effect);
	}

	public GameObject GetEffectPrefab(string assetBundleName)
	{
		if(_effectPrefabList.ContainsKey(assetBundleName))
			return _effectPrefabList[assetBundleName];
		return null;
	}

	//播放特效，parent 特效对象的父对象， scale 特效缩放比,.
	public EffectFx PlayEffect(string prefabName, GameObject parent, string extraName = "", float scale = 1, float life = 1, 
		System.Action<EffectFx> loadFinishCallBack = null)
	{
		Log.debug(this, "[EffectManger][PlayEffect] play " + prefabName + " parent " + parent + "extraName " + extraName + " scale " + scale + " life " + life);
		EffectFx fx = EffectManager.CreateSceneFx(extraName, life);
		fx.transform.parent = null;

		fx.gameObject.transform.localScale = scale * Vector3.one;
		fx.scale = scale;
		fx.gameObject.transform.localRotation = Quaternion.identity;
		if(parent != null)
			fx.transform.parent = parent.transform;
		fx.gameObject.transform.localPosition = Vector3.zero;
		fx.onLoadFinishCallBack = loadFinishCallBack;


		fx.prefabPath = prefabName;
		_playingEffect.Add(fx);
		if(_effectPrefabList.ContainsKey(prefabName) == false)
		{
			LoadEffect(prefabName);
			return fx;
		}

		GameObject baseObject = _effectPrefabList[prefabName];
		AttachBodyObject(fx, baseObject);
		if(loadFinishCallBack != null)
			loadFinishCallBack(fx);
		return fx;
	}



	//创建SceneFx...
	public static EffectFx CreateSceneFx(string name, float life)
	{		
		GameObject g = new GameObject ("FX_"+name);
		//g.layer = (int)LayerEnum.FX;
		EffectFx entity  = g.AddComponent<EffectFx> ();
		entity.life = life;
		return entity; 
	}

	//加载特效.
	/// <summary>
	/// Loads the effect.
	/// TODO: https://www.cnblogs.com/lixiang-share/p/4639288.html,改用abs加载...
	/// </summary>
	/// <param name="prefabPath">Prefab path.</param>
	public void LoadEffect(string prefabPath)
	{
		Log.info(this,"[EffectManager][LoadEffect] assetBundleFilePath " + prefabPath);

		//把资源加载到内存中  
		//SceneResourceManager.Instance.StartLoad(c, OnEffectLoaded);
		Object cubePreb = Resources.Load(prefabPath, typeof(GameObject));
		GameObject baseObject = cubePreb as GameObject;

		//update box collider of effectManager panel...
		GameObject g = (GameObject)baseObject;
		BoxCollider box0 = g.GetComponent<BoxCollider>();
		if(null != box0)
		{
			float _time = 1f;
			if(box0.size.x > 10)
			{
				_time = 0.01f;
				g.transform.localScale = g.transform.localScale *_time;
			}
			BoxCollider box = gameObject.GetComponent<BoxCollider>();
			if(null == box)
				box = gameObject.AddComponent<BoxCollider>();
			box.isTrigger = true;
			box.center = box0.center*_time;
			box.size = box0.size*_time;
			box0.enabled = false;
		}

		//缓存Prefab.
		AddEffectPrefab(prefabPath, baseObject);
		CheckLoadingEffect(prefabPath, baseObject);
	}

	/// <summary>
	/// Attachs the body object to the fx.gameobject.
	/// </summary>
	/// <param name="fx">Fx.</param>
	/// <param name="baseObject">Base object.</param>
	void AttachBodyObject(EffectFx fx, GameObject baseObject)
	{
		Log.debug(this,"[EffectManager][AttachBodyObject] SceneFx " + fx.prefabPath + " prefab " + baseObject);
		GameObject bodyObject = GameObject.Instantiate (baseObject) as GameObject;
		bodyObject.transform.parent = fx.transform;
		bodyObject.transform.localPosition = Vector3.zero;
		bodyObject.transform.localScale = Vector3.one;
		bodyObject.transform.localRotation = Quaternion.identity;

		fx.bodyObject = bodyObject;
		fx.IsDone = true;

		//EffectScale..
		//TODO: some sprite anmation script...
		CycleImageAnimation [] ps = fx.gameObject.GetComponentsInChildren<CycleImageAnimation>();
		if(ps != null && ps.Length > 0)
		{
			//change render camera or something else to do...
		}

		//CommonFunction.ChangeLayer(fx.gameObject, fx.gameObject.layer);
	}

	//在加载完资源后,这里检查是否有正在加载的SceneFx.
	void CheckLoadingEffect(string assetBundleName, GameObject baseObject)
	{
		foreach(EffectFx fx in _playingEffect)
		{
			if(fx.IsDone == true)
				continue;
			if(fx.prefabPath != assetBundleName)
				continue;

			AttachBodyObject (fx, baseObject);	
			if(fx.onLoadFinishCallBack != null)
			{
				fx.onLoadFinishCallBack(fx);
			}
		}
	}
	//清理特效缓存，这里一般是在场景切换时.
	public void Clear()
	{
		_effectPrefabList.Clear();
		Resources.UnloadUnusedAssets();
	}

	public void DestroyEffect(EffectFx fx)
	{
		if(fx == null)
			return;
		
		Log.debug(this, "[EffectManger][DestroyEffect] " + fx.prefabPath);
		_playingEffect.Remove(fx);
		Destroy(fx.gameObject);
	}
	/// <summary>
	/// 删除指定特效..
	/// </summary>
	/// <param name="fx">Fx.</param>
	public void RemoveEffect(EffectFx fx)
	{
		if(fx != null)
			fx.life = 0;
	}
}
