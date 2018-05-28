using UnityEngine;
using UnityEngine.UI;
namespace BulletScreen {
	[System.Serializable]
	public class BulletScreenDisplayerModel {
		[Header("组件挂接的节点")]
		public Transform Owner;

		[Header("文本预制")]
		public GameObject TextPrefab; //for text and multimedia..

		[Header("弹幕布局组件")]
		public GridLayoutGroup ScreenRoot;

		[Header("初始化行数")]
		public int TotalRowCount = 2;

		[Header("行高（单位：像素）")]
		public float RowHeight;

		[Header("字体边框的节点名字")]
		public string TextBoxNodeName;

		[Header("从屏幕一侧到另外一侧用的时间")]
		public float ScrollDuration = 8F;

		[Header("两弹幕文本之间的最小间隔")]
		public float MinInterval = 20F;

		[Header("移动完成后的销毁延迟")]
		public float KillBulletTextDelay = 0F;

		/// <summary>
		/// Initializes a new instance of the <see cref="BulletScreen.BulletScreenDisplayerInfo"/> class.
		/// </summary>
		/// <param name="owner">Owner.</param>
		/// <param name="textPrefab">Text prefab.</param>
		/// <param name="screenRoot">Screen root.</param>
		/// <param name="initialRowCount">Initial row count.</param>
		/// <param name="rowHeight">Row height.</param>
		/// <param name="textBoxNodeName">Text box node name.</param>
		public BulletScreenDisplayerModel(Transform owner, GameObject textPrefab, GridLayoutGroup screenRoot,
			int initialRowCount = 1,
			float rowHeight = 100F,
			string textBoxNodeName = "text_box_node_name") {
			Owner = owner;
			TextPrefab = textPrefab;
			ScreenRoot = screenRoot;
			TotalRowCount = initialRowCount;
			RowHeight = rowHeight;
			TextBoxNodeName = textBoxNodeName;
		}
	}
}

