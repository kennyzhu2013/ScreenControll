using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CycleImageAnimation : MonoBehaviour {
	/// <summary>
	/// The pics to cycle animation.
	/// </summary>
	//public List<Image> _Images = new Queue<Sprite>();
	public float animateTime;
	public RectTransform _EndPos;
	public float delayTime = 0f;

	// Use this for initialization...
	void Start () {

		//_CurImage = _Images.Dequeue ();
		transform.DOLocalMoveY (_EndPos.localPosition.y, animateTime).SetDelay(delayTime).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart).SetRecyclable();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

}
