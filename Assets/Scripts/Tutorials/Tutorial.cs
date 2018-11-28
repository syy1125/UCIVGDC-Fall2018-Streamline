using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
	private TutorialStep[] _steps;

	private int _index;
	private GameObject _outline;
	private GameObject _hintText;
	private TutorialManager _manager;

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
			!GameController.gameLevel.Tutorial.Equals(TutorialName)
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

		_index = 0;

		_outline = Instantiate(Resources.Load<GameObject>("TutorialOutline"), transform);
		MatchTransform(_outline.GetComponent<RectTransform>(), _steps[_index].Position);

		_hintText = Instantiate(Resources.Load<GameObject>("TutorialText"), transform);
		UpdateHintText();

		_manager = transform.parent.GetComponent<TutorialManager>();
	}

	private void UpdateHintText()
	{
		RectTransform t = _hintText.GetComponent<RectTransform>();
		t.parent = transform.GetChild(_index).GetComponent<RectTransform>();
		MatchTransform(t, _steps[_index].HintText.rectTransform);

		_hintText.GetComponent<Text>().alignment = _steps[_index].HintText.alignment;
		_hintText.GetComponent<Text>().text = _steps[_index].HintText.text;
	}

	protected void Next()
	{
		if (++_index < _steps.Length)
		{
			UpdateUI();
		}
		else
		{
			gameObject.SetActive(false);

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
		RectTransform t = _outline.GetComponent<RectTransform>();

		RectTransform start = Instantiate(Resources.Load<GameObject>("TutorialOutline"), transform)
			.GetComponent<RectTransform>();
		start.gameObject.SetActive(false);
		MatchTransform(start, t);
		RectTransform finish = _steps[_index].Position;

		float startTime = Time.time;

		while (Time.time - startTime < _manager.MoveDuration)
		{
			float proportion = _manager.OutlineCurve.Evaluate((Time.time - startTime) / _manager.MoveDuration);

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
		Text hintText = _hintText.GetComponent<Text>();

		float startTime = Time.time;

		// Fade out
		while (Time.time - startTime < _manager.TextFadeDuration)
		{
			hintText.color = new Color(
				hintText.color.r,
				hintText.color.g,
				hintText.color.b,
				1 - (Time.time - startTime) / _manager.TextFadeDuration
			);

			yield return null;
		}

		yield return new WaitForSeconds(_manager.MoveDuration - 2 * _manager.TextFadeDuration);

		UpdateHintText();

		// Fade in
		while (Time.time - startTime < _manager.MoveDuration)
		{
			hintText.color = new Color(
				hintText.color.r,
				hintText.color.g,
				hintText.color.b,
				1 - (startTime + _manager.MoveDuration - Time.time) / _manager.TextFadeDuration
			);

			yield return null;
		}
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
}