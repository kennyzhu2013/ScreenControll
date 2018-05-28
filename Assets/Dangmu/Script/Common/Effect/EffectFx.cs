using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectFx : MonoBehaviour {
	bool _isDestroy=false;
	public bool isDestroy
	{
		get{return _isDestroy;}
		set{_isDestroy=value;}
	}

	bool _isDone = false;
	public bool IsDone {
		get{return _isDone;}
		set{
			_isDone = value;
		}
	}
		
	private GameObject root;
	private float rootScaleX;
	private int currentRotationX;

	public System.Action<EffectFx> onLoadFinishCallBack; 

	/// <summary>
	/// The life of the effect.
	/// </summary>
	public float life;
	public float Effect_Time{
		get { return life; }
	}

	public static EffectFx CreateSceneFx(string name, float life)
	{
		GameObject fxObj = new GameObject ("FX_" + name);
		EffectFx entity  = fxObj.AddComponent<EffectFx> ();
		entity._bodyObject = fxObj;
		entity.life = life;
		return entity; 
	}

	public string prefabPath ; //Prefab路径
	public bool initVisible = false; //是否可见
	public bool isParticle = false;

	protected  GameObject _bodyObject; //实例化后的模型.
	public GameObject bodyObject {
		get {
			return _bodyObject;
		}
		set {
			_bodyObject = value;
		}
	}

	public float scale = 1;

	//模型本身，实例化后的模型...
	public void SetVisible(bool b)
	{
		initVisible = true;
		if (null == _bodyObject)
			return;
		_bodyObject.SetActive(b);
	}

	// Update is called once per frame
	void Update () 
	{
		if( _isDone == false )
			return;
		
		if( isParticle )
		{
			//TODO: 粒子系统的效果..
		}

	}

	private void SetPRotationX()
	{
		ParticleSystem ps;
		ParticleSystem[] pss = root.transform.GetComponentsInChildren<ParticleSystem>();
		for(int i=0;i<pss.Length;i++)
		{
			ps = pss[i];
			ps.transform.localEulerAngles = new Vector3(ps.transform.localEulerAngles.x*currentRotationX,
				ps.transform.localEulerAngles.y,ps.transform.localEulerAngles.z);
		}
	}

	void OnDestroy()
	{
		EffectManager.Instance().DestroyEffect(this);
	}
}
