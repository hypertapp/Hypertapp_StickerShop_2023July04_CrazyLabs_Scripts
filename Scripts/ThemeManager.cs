using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThemeManager : MonoBehaviour
{
	public static int currentTheme;

	void Awake ()
	{
	}

	public void Init ()
	{
	}

	void SetTheme (int theme_)
    {
		currentTheme = theme_;
		PlayerPrefs.SetInt ("CurrentTheme", currentTheme);
	}
}
