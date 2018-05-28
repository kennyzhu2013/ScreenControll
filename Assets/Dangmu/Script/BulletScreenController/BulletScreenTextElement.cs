using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace BulletScreen
{
	public enum ScrollDirection {
		RightToLeft = 0,
		LeftToRight = 1
	}

	/// <summary>
	/// Bullet screen text element.
	/// </summary>
	public class BulletScreenTextElement : MonoBehaviour {
		[SerializeField]private BulletScreenDisplayer   _displayer;
		[SerializeField]private string                  _textContent;
		[SerializeField]private bool                    _showBox;
		[SerializeField]private ScrollDirection         _scrollDirection;
		[SerializeField]private Text                    _text;
		[SerializeField]private float                   _textWidth;
		[SerializeField]private Vector3                 _startPos;
		[SerializeField]private Vector3                 _endPos;

		/// <summary>
		/// Create the specified displayer, textContent, showBox and direction.
		/// </summary>
		/// <param name="displayer">Displayer.</param>
		/// <param name="textContent">Text content.</param>
		/// <param name="showBox">If set to <c>true</c> show box.</param>
		/// <param name="direction">Direction.</param>
		public static BulletScreenTextElement Create(BulletScreenDisplayer displayer, string textContent,
			bool showBox = false,
			ScrollDirection direction = ScrollDirection.RightToLeft) {
			BulletScreenTextElement instance = null;
			if (displayer == null) {
				Debug.Log("BulletScreenTextElement.Create(), displayer can not be null !");
				return null;
			}

			GameObject go = Instantiate(displayer.TextElementPrefab) as GameObject;
			go.transform.SetParent(displayer.GetTempRoot());
			go.transform.localPosition = Vector3.up*10000F;
			go.transform.localScale = Vector3.one;
			//RectTransform rect = go.GetComponent<RectTransform> ();
			//rect.rect.height = 100;
			//rect.rect.width = 100;
			instance = go.AddComponent<BulletScreenTextElement>();
			instance._displayer = displayer;
			instance._textContent = textContent;
			instance._showBox = showBox;
			instance._scrollDirection = direction;
			return instance;
		}


		private IEnumerator Start() {
			SetBoxView();
			SetText();
			//get correct text width in next frame.
			yield return new WaitForSeconds(0.2f); 
			RecordTextWidthAfterFrame();
			SetRowInfo();
			SetTweenStartPosition();
			SetTweenEndPosition();
			StartMove();
		}

		/// <summary>
		/// The outer box view of text
		/// </summary>
		private void SetBoxView() {
			Transform boxNode = transform.Find(_displayer.TextBoxNodeName);
			if (boxNode == null) {
				//Debug.LogErrorFormat("BulletScreenTextElement.SetBoxView(), boxNode == null. boxNodeName: {0}",
				//_displayer.TextBoxNodeName);
				return;
			}
			boxNode.gameObject.SetActive(_showBox);
		}

		/// <summary>
		/// Sets the text.
		/// </summary>
		private void SetText() {
			_text = GetComponentInChildren<Text>();
			//_text.enabled = false;
			if (_text == null) {
				Debug.Log("BulletScreenTextElement.SetText(), not found Text!");
				return;
			}
			_text.alignment = _scrollDirection == ScrollDirection.RightToLeft ? TextAnchor.MiddleLeft : TextAnchor.MiddleRight;
			//make sure there exist ContentSizeFitter componet for extend text width
			var sizeFitter = _text.GetComponent<ContentSizeFitter>();
			if (!sizeFitter) {
				sizeFitter = _text.gameObject.AddComponent<ContentSizeFitter>();
			}
			//text should extend in horizontal
			sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
			_text.text = _textContent;
		}

		private void RecordTextWidthAfterFrame() {
			_textWidth = _text.GetComponent<RectTransform>().sizeDelta.x;
		}

		private void SetTweenStartPosition() {
			Vector3 nor = _scrollDirection == ScrollDirection.RightToLeft ? Vector3.right : Vector3.left;
			_startPos = nor * (_displayer.BulletScreenWidth / 2F + _textWidth / 2F);
			transform.localPosition = _startPos;
		}

		private void SetTweenEndPosition() {
			Vector3 nor = _scrollDirection == ScrollDirection.RightToLeft ? Vector3.left : Vector3.right;
			_endPos = nor * (_displayer.BulletScreenWidth / 2F + _textWidth / 2F);
		}

		private void SetRowInfo() {
			var bulletTextInfo = new BulletTextInfo() {
				SendTime = Time.realtimeSinceStartup,
				TextWidth = _textWidth
			};
			var rowRoot = _displayer.GetRowRoot(bulletTextInfo);
			transform.SetParent(rowRoot, false);
			transform.localScale = Vector3.one;
		}

		private void StartMove() {
			//make sure the text is active.
			//the default ease of DoTewwen is not Linear.
			transform.DOLocalMoveX(_endPos.x, _displayer.ScrollDuration).OnComplete(OnTweenFinished).SetEase(Ease.Linear);
		}

		private void OnTweenFinished() {
			Destroy(gameObject, _displayer.KillBulletTextDelay);
		}
	}
}
