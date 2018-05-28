using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BulletScreen
{
	public class BulletScreenDisplayer : MonoBehaviour {

		public bool Enable { get; set; }
		public BulletTextInfo[] _currBulletTextInfoList; 
		[SerializeField]public BulletScreenDisplayerModel _info;
		public float ScrollDuration {
			get { return _info.ScrollDuration; }
		}

		private float _bulletScreenWidth;
		public float BulletScreenWidth {
			get { return _bulletScreenWidth; }
		}

		public GameObject TextElementPrefab {
			get { return _info.TextPrefab; }
		}

		public string TextBoxNodeName {
			get { return _info.TextBoxNodeName; }
		}

		public float KillBulletTextDelay {
			get { return _info.KillBulletTextDelay; }
		}

		public Transform ScreenRoot {
			get { return _info.ScreenRoot.transform; }
		}

		public static BulletScreenDisplayer Create(BulletScreenDisplayerModel displayerInfo) {
			BulletScreenDisplayer instance = displayerInfo.Owner.gameObject.AddComponent<BulletScreenDisplayer>();
			instance._info = displayerInfo;
			return instance;
		}

		public void AddBullet(string textContent, bool showBox = false, ScrollDirection direction = ScrollDirection.RightToLeft) {
			BulletScreenTextElement.Create(this, textContent, showBox, direction);
		}

		private void Start() {
			SetScrollScreen();
			InitRow();
		}

		private void InitRow() {
			Utility.DestroyAllChildren(_info.ScreenRoot.gameObject);
			_currBulletTextInfoList = new BulletTextInfo[_info.TotalRowCount];
			for (int rowIndex = 0; rowIndex < _info.TotalRowCount; rowIndex++) {
				_currBulletTextInfoList[rowIndex] = null;

				//创建子节点...
				string rowNodeName = string.Format("row_{0}", rowIndex);
				GameObject newRow = new GameObject(rowNodeName);
				var rt = newRow.AddComponent<RectTransform>();
				rt.SetParent(_info.ScreenRoot.transform, false);
			}
		}
		private void SetScrollScreen() {
			_info.ScreenRoot.childAlignment = TextAnchor.MiddleCenter;
			_info.ScreenRoot.cellSize = new Vector2(100F, _info.RowHeight);
			_bulletScreenWidth = _info.ScreenRoot.GetComponent<RectTransform>().rect.width;
		}
		public Transform GetTempRoot() {
			return _info.ScreenRoot.transform.Find(string.Format("row_{0}", 0));
		}
		public Transform GetRowRoot(BulletTextInfo newTextInfo) {
			const int notFoundRowIndex = -1;
			int searchedRowIndex = notFoundRowIndex;
			newTextInfo.SendTime = Time.realtimeSinceStartup;

			for (int rowIndex = 0; rowIndex < _currBulletTextInfoList.Length; rowIndex++) {
				var textInfo = _currBulletTextInfoList[rowIndex];
				//if no bullet text info exist in this row, create the new directly.
				if (textInfo == null) {
					searchedRowIndex = rowIndex;
					break;
				}
				float l1 = textInfo.TextWidth;
				float l2 = newTextInfo.TextWidth;
				float sentDeltaTime = newTextInfo.SendTime - textInfo.SendTime;
				var aheadTime = GetAheadTime(l1, l2);
				if (sentDeltaTime >= aheadTime) {//fit and add.
					searchedRowIndex = rowIndex;
					break;
				}
				//go on searching in next row.
			}
			if (searchedRowIndex == notFoundRowIndex) {//no fit but random one row.
				int repairRowIndex = Random.Range(0, _currBulletTextInfoList.Length);
				searchedRowIndex = repairRowIndex;
			}
			_currBulletTextInfoList[searchedRowIndex] = newTextInfo;
			Transform root = _info.ScreenRoot.transform.Find(string.Format("row_{0}", searchedRowIndex));
			return root;
		}

		/// <summary>
		/// Logic of last bullet text go ahead.
		/// </summary>
		/// <param name="lastBulletTextWidth">width of last bullet text</param>
		/// <param name="newCameBulletTextWidth">width of new came bullet text</param>
		/// <returns></returns>
		private float GetAheadTime(float lastBulletTextWidth, float newCameBulletTextWidth) {
			float aheadTime = 0f;
			if (lastBulletTextWidth <= newCameBulletTextWidth) {
				float s1 = lastBulletTextWidth + BulletScreenWidth + _info.MinInterval;
				float v1 = (lastBulletTextWidth + BulletScreenWidth) / _info.ScrollDuration;
				float s2 = BulletScreenWidth;
				float v2 = (newCameBulletTextWidth + BulletScreenWidth) / _info.ScrollDuration;
				aheadTime = s1 / v1 - s2 / v2;  
			}
			else {
				float aheadDistance = lastBulletTextWidth + _info.MinInterval;
				float v1 = (lastBulletTextWidth + BulletScreenWidth) / _info.ScrollDuration;
				aheadTime = aheadDistance / v1;
			}
			return aheadTime;
		}
	}
}
