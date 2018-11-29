using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Experimental.UIElements;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class Tutorial : MonoBehaviour
{
	private TutorialStep[] _steps;

	private int _index;
	protected GameObject Outline { get; private set; }
	protected GameObject HintText { get; private set; }
	protected TutorialManager Manager { get; private set; }

	private Coroutine _outlineTransition;
	private Coroutine _textTransition;

	private string TutorialName
	{
		get { return gameObject.name; }
	}

	private void Awake()
	{
		string[] completedTutorials = PlayerPrefs.GetString("TutorialProgress", "").Split(':');
		if (
			GameController.gameLevel == null
			|| !GameController.gameLevel.Tutorial.Equals(TutorialName)
			|| Array.BinarySearch(completedTutorials, TutorialName) >= 0
		)
		{
			gameObject.SetActive(false);
			return;
		}

		_steps = new TutorialStep[transform.childCount];

		for (int i = 0; i < transform.childCount; i++)
		{
			Transform child = transform.GetChild(i);
			_steps[i] = child.GetComponent<TutorialStep>();
			foreach (Transform t in child)
			{
				t.gameObject.SetActive(false);
			}
		}

		_index = -1;

		Outline = Instantiate(Resources.Load<GameObject>("TutorialOutline"), transform);
		HintText = Instantiate(Resources.Load<GameObject>("TutorialText"), transform);
		Manager = transform.parent.GetComponent<TutorialManager>();

		Next();
	}

	private void UpdateHintText()
	{
		RectTransform t = HintText.GetComponent<RectTransform>();
		t.SetParent(transform.GetChild(_index).GetComponent<RectTransform>());
		MatchTransform(t, _steps[_index].HintText.rectTransform);

		HintText.GetComponent<Text>().alignment = _steps[_index].HintText.alignment;
		HintText.GetComponent<Text>().text = _steps[_index].HintText.text;
	}

	protected void Next()
	{
		if (++_index < _steps.Length)
		{
			UpdateUI();
		}
		else
		{
			StartCoroutine(FadeOut());

			string[] completedTutorials = PlayerPrefs.GetString("TutorialProgress", "").Split(':');
			Array.Resize(ref completedTutorials, completedTutorials.Length + 1);
			completedTutorials[completedTutorials.Length - 1] = TutorialName;
			Array.Sort(completedTutorials);
			PlayerPrefs.SetString("TutorialProgress", string.Join(":", completedTutorials));
		}
	}

	private void UpdateUI()
	{
		if (_outlineTransition != null)
		{
			StopCoroutine(_outlineTransition);
			StopCoroutine(_textTransition);
		}

		_outlineTransition = StartCoroutine(MoveOutline());
		_textTransition = StartCoroutine(MoveText());
	}

	private IEnumerator MoveOutline()
	{
		RectTransform t = Outline.GetComponent<RectTransform>();

		RectTransform start = Instantiate(Resources.Load<GameObject>("TutorialOutline"), transform)
			.GetComponent<RectTransform>();
		start.gameObject.SetActive(false);
		MatchTransform(start, t);
		RectTransform finish = _steps[_index].Position;

		float startTime = Time.time;

		while (Time.time - startTime < Manager.MoveDuration)
		{
			float proportion = Manager.OutlineCurve.Evaluate((Time.time - startTime) / Manager.MoveDuration);

			t.anchorMin = Vector2.Lerp(start.anchorMin, finish.anchorMin, proportion);
			t.anchorMax = Vector2.Lerp(start.anchorMax, finish.anchorMax, proportion);
			t.pivot = Vector2.Lerp(start.pivot, finish.pivot, proportion);
			t.offsetMin = Vector2.Lerp(start.offsetMin, finish.offsetMin, proportion);
			t.offsetMax = Vector2.Lerp(start.offsetMax, finish.offsetMax, proportion);

			yield return null;
		}

		Destroy(start.gameObject);
	}

	private IEnumerator MoveText()
	{
		HintText.GetComponent<Text>().CrossFadeAlpha(0, Manager.TextFadeDuration, false);

		yield return new WaitForSeconds(Manager.MoveDuration - Manager.TextFadeDuration);

		UpdateHintText();
		
		HintText.GetComponent<Text>().CrossFadeAlpha(1, Manager.TextFadeDuration, false);
	}
	
	private static void MatchTransform(
		RectTransform lvalue,
		RectTransform rvalue
	)
	{
		lvalue.anchorMin = rvalue.anchorMin;
		lvalue.anchorMax = rvalue.anchorMax;
		lvalue.pivot = rvalue.pivot;
		lvalue.offsetMin = rvalue.offsetMin;
		lvalue.offsetMax = rvalue.offsetMax;
	}

	private IEnumerator FadeOut()
	{
		Outline.GetComponent<Image>().CrossFadeAlpha(0,Manager.TextFadeDuration, false);
		HintText.GetComponent<Text>().CrossFadeAlpha(0,Manager.TextFadeDuration, false);

		yield return new WaitForSeconds(Manager.TextFadeDuration);

		gameObject.SetActive(false);
	}
}