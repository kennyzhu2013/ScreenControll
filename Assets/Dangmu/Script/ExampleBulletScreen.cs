using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using BulletScreen;
public class ExampleBulletScreen : MonoBehaviour {
	public BulletScreenDisplayer Displayer;
	public List<string> _textPool = new List<string>() {
		"ウワァン!!(ノДヽ) ・・(ノ∀・)チラ 実ゎ・・嘘泣き",
		"(╯#-_-)╯~~~~~~~~~~~~~~~~~╧═╧ ",
		"<(￣︶￣)↗[GO!]",
		"(๑•́ ₃ •̀๑) (๑¯ิε ¯ิ๑) ",
		"(≖͞_≖̥)",
		"(｀д′) (￣^￣) 哼！ <(｀^′)>",
		"o(*￣︶￣*)o",
		" ｡:.ﾟヽ(｡◕‿◕｡)ﾉﾟ.:｡+ﾟ",
		"号(┳Д┳)泣",
		"( ＾∀＾）／欢迎＼( ＾∀＾）",
		"ドバーッ（┬┬＿┬┬）滝のような涙",
		"(。┰ω┰。",
		"啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊"
	};

	// Use this for initialization
	void Start() {
		Displayer.Enable = true;
		StartCoroutine(StartDisplayBulletScreenEffect());
	}

	/// <summary>
	/// Starts the display bullet screen effect.
	/// </summary>
	/// <returns>The display bullet screen effect.</returns>
	private IEnumerator StartDisplayBulletScreenEffect() {
		while (Displayer.Enable) {
			Displayer.AddBullet(GetText(), CheckShowBox(), GetDirection());
			yield return new WaitForSeconds(0.2f);
		}
	}

	private string GetText() {
		int textIndex = Random.Range(0, _textPool.Count);
		var weightDict = new Dictionary<object, float>() {
			{"<color=yellow>{0}</color>", 10f},
			{"<color=red>{0}</color>", 2f},
			{"<color=white>{0}</color>", 80f}
		};
		string randomColor = (string)Utility.RandomObjectByWeight(weightDict);
		string text = string.Format(randomColor, _textPool[textIndex]);
		return text;
	}

	private bool CheckShowBox() {
		var weightDict = new Dictionary<object, float>() {
			{true, 20f},
			{false, 80f}
		};
		bool ret = (bool)Utility.RandomObjectByWeight(weightDict);
		return ret;
	}

	private ScrollDirection GetDirection() {
		var weightDict = new Dictionary<object, float>() {
			{ScrollDirection.LeftToRight, 5f},
			{ScrollDirection.RightToLeft, 80f}
		};
		ScrollDirection direction = (ScrollDirection)Utility.RandomObjectByWeight(weightDict);
		return direction;
	}

}