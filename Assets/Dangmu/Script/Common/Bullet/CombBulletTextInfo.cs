using System;
using BulletScreen;

namespace BulletScreen {
	/// <summary>
	/// Bullet text info.
	/// </summary>
	public class BulletTextInfo {
		public float TextWidth;
		public float SendTime;
	}

	/// <summary>
	/// Comb bullet text info.
	/// 在inspector界面显示...
	/// </summary>
	[System.Serializable]
	public class CombBulletTextInfo: BulletTextInfo {
		public float TextHeight; //added for vertical view...
		public float ImageWidth;
		public float ImageHeight;
		public bool  IsText;
		public bool Is3DEffect; //Text字体是否开启3d特效....
		//TODO:add video length
	}
}
