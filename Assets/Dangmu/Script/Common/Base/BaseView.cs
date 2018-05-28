using System;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace Common
{
	/// <summary>
	/// Base view.
	/// TODO: add load process....
	/// </summary>
	public class BaseView: MonoBehaviour {
		//not need loading...
		public enum ViewState
		{
			//LoadFinish,
			Init,
			OpenTween,
			Open,
			CloseTween,
			Close
		}

		[HideInInspector] public ViewState currentState;/**当前面板状态**/
		public bool isTweenWin = false;//该界面打开关闭是否拥有动画.
		public float tweenDuration { //time of tweening...
			get;
			set;
		}

		private enum ViewTweenType
		{
			Hide = 0,
			Show,
			Delete
		}
		private ViewTweenType TweenType = ViewTweenType.Hide; //0为隐藏界面 1为显示界面 2为删除界面..

		protected bool isMakePixelPerfect = false;

		/// <summary>
		/// Start this instance.
		/// </summary>
		public virtual void Start()
		{
			tweenDuration = 0.5f;
			currentState = ViewState.Init;
			//not need load ,call directly...

			//init show..
			init ();
		}

		public virtual void init()
		{
			if (isMakePixelPerfect) {
				var sizeFitter = gameObject.GetComponent<ContentSizeFitter> ();
				if (!sizeFitter) {
					sizeFitter = gameObject.AddComponent<ContentSizeFitter> ();
				}

				//text should extend in horizontal
				sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
				sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
			}
			Show();
			Log.info("---------->> init /time=" + Time.time);
		}

		//resource load...
		/**如果界面是以隐藏的方式删除的话，再次OPEN会调用这里**/
		public virtual void VisibleShow()
		{

		}
		//TODO: add resources recycle...

		/**关闭且删除界面**/
		public virtual void close()
		{
			if( isTweenWin )
			{
				TweenType = ViewTweenType.Delete;
				AddTweenWin(false);
				currentState = ViewState.CloseTween;
				return;
			}
			currentState = ViewState.Close;
			//
			CloseWin();
		}

		/**隐藏界面**/
		public virtual void Hide()
		{
			if(isTweenWin)
			{
				TweenType = ViewTweenType.Hide;
				AddTweenWin(false);
				currentState = ViewState.CloseTween;
				return;
			}
			currentState = ViewState.Close;
			gameObject.SetActive(false);
		}

		/**在界面隐藏的情况下 显示界面**/
		public virtual void Show()
		{
			gameObject.SetActive(true);
			Log.info ("Show gameObject:" + gameObject.ToString());
			if( isTweenWin )
			{
				TweenType = ViewTweenType.Show;
				AddTweenWin(true);
				currentState = ViewState.OpenTween;
				return;
			}else
			{
				currentState = ViewState.Open;
				VisibleShow();
			}
		}

		/**删除界面**/
		/// <summary>
		/// Closes the window.
		/// TODO: add delete animation...
		/// </summary>
		private void CloseWin()
		{
			GameObject.Destroy(this.gameObject);
			//UIWIndowsManager.Instance.removeWin(viewLoadBase.GetType());
		}

		/**添加动画**/
		private void AddTweenWin(bool show)
		{
			/*tc = this.gameObject.AddComponent<TweenScale>();
			tc.SetOnFinished(TweenOnfinished);*/
			//when finished call TweenOnfinished..
			//tweenDuration
			//Vector3 from;
			Vector3 to;
			int t =System.Convert.ToInt32(show);
			//from = new Vector3(f,f,f);
			to = new Vector3(t,t,t);

			//TODO: add autoDelete...
			transform.DOScale(t, tweenDuration).OnComplete(TweenOnfinished).SetEase(Ease.Linear);
		}

		/**动画后执行**/
		private void TweenOnfinished()
		{
			if( TweenType == ViewTweenType.Delete )
			{
				CloseWin();
				currentState = ViewState.Close;
			} else { //open or hide...
				gameObject.SetActive( System.Convert.ToBoolean(TweenType) );
				if( TweenType == ViewTweenType.Show ){
					VisibleShow();
					currentState = ViewState.Open;
				} else {
					currentState = ViewState.Close;
				}
			}
		}
	}
}
