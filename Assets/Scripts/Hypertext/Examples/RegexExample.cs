/*
 * uGUI-Hypertext (https://github.com/setchi/uGUI-Hypertext)
 * Copyright (c) 2019 setchi
 * Licensed under MIT (https://github.com/setchi/uGUI-Hypertext/blob/master/LICENSE)
 */

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Hypertext
{
	public class RegexExample : MonoBehaviour
	{
		[SerializeField] RegexHypertext text = default;

		// トークン
		List<string> tokenList = new List<string>();

		void Start()
		{
			foreach (KeywordMaster keywordMaster in GameManager.instance.masterManager.keywordData)
			{
				text.OnClick(keywordMaster.keyword, Color.blue, only => showDescribe(keywordMaster));	
			}

			tokenList = new List<string>(GameManager.instance.masterManager.getCardName());
			foreach (string token in tokenList)
			{
				string tokenText = "「" + token + "」";
				text.OnClick(tokenText, Color.blue, token => tokenCallBack(token.Trim('「', '」')));
			}
		}

		private void tokenCallBack(string cardName)
		{
			CardMaster cardMaster = GameManager.instance.masterManager.getCardEntityByName(cardName);
			GameManager.instance.showDescribe(cardMaster, isSub: true);
		}

		private void showDescribe(KeywordMaster keyword)
		{
			GameManager.instance.showDescribe(keyword, isSub: true);
		}
	}
}