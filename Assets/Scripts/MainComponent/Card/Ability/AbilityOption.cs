using System;
using System.Linq;
using System.Text.RegularExpressions;

public class AbilityOption
{
	string optionName;

	public AbilityOption(string optionText)
	{
		if (!optionText.Contains("="))
		{
			optionName = optionText;
		}
		else
		{
			optionName = optionText.Substring(0, optionText.IndexOf("="));
		}
	}

	public bool tryGetOptionValueString(string optionText, string keyword, out string value)
	{
		if (optionText == null || !optionText.Contains(keyword + "="))
		{
			value = "";
			return false;
		}

		optionText = optionText.Replace(keyword + "=", "");
		value = optionText;
		return true;
	}

	public bool tryGetOptionValueInt(string optionText, string keyword, out CountBase value)
	{
		if (optionText == null || !optionText.Contains(keyword + "="))
		{
			value = new CountBase();
			return false;
		}

		optionText = optionText.Replace(keyword + "=", "");
		value = AbilityUtility.countFactory(optionText);
		return true;
	}

	public bool parseOptionBool(string optionText, string keyword)
	{
		return optionText == keyword;
	}
}