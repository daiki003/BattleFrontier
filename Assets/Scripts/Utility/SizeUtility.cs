public class SizeUtility
{
	// デッキ名のフォントサイズを調整する
	public static int getDeckNameFontSize(string deckName)
	{
		int fontSize = 70;
		if(deckName.Length < 7) fontSize = 70;
		else if(deckName.Length < 11) fontSize = 60 - ((deckName.Length - 7) * 5);
		else fontSize = 40;
		return fontSize;
	}

	// カード名のフォントサイズを調整する
	public static int getCardNameFontSize(string cardName)
	{
		if(cardName.Length < 9) return 20;
		else
		{
			switch (cardName.Length)
			{
				case 9: return 18;
				case 10: return 16;
				case 11: return 14;
				case 12: return 13;
				case 13: return 12;
				case 14:
				case 15: return 11;
				case 16:
				case 17: return 10;
				default: return 9;
			}
		}
	}

	// バフ短冊のフォントサイズを調整する
	public static int getBuffItemFontSize(string buffText)
	{
		if(buffText.Length <= 11)
		{
			return 35;
		}
		else if (buffText.Length >= 14 && buffText.Length <= 26)
		{
			return 29;
		}
		else
		{
			switch (buffText.Length)
			{
				case 12: return 33;
				case 13: return 30;
				case 27:
				case 28: return 28;
				case 29:
				case 30: return 26;
				case 31:
				case 32: return 25;
				default: return 20;
			}
		}
	}
}
