using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
	[Serializable]
	public class Step
	{
		[HideInInspector]
		public RectTransform OutlinePosition;

		[TextArea]
		public string HintText;

		public Vector2Int TextOffset;

		[FormerlySerializedAs("Anchor")]
		public TextAnchor TextAlign;
	}

	public Step[] Steps;

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
			GameController.gameLevel.Name.Equals(TutorialName)
			|| Array.BinarySearch(completedTutorials, TutorialName) >= 0
		)
		{
			gameObject.SetActive(false);
			return;
		}

		for (int i = 0; i < transform.childCount; i++)
		{
			Steps[i].OutlinePosition = transform.GetChild(i).GetComponent<RectTransform>();
		}

		_outline = Instantiate(Resources.Load<GameObject>("TutorialOutline"), transform);
		_outline.GetComponent<RectTransform>().anchorMin = Steps[0].OutlinePosition.anchorMin;
		_outline.GetComponent<RectTransform>().anchorMax = Steps[0].OutlinePosition.anchorMax;
		_outline.GetComponent<RectTransform>().pivot = Steps[0].OutlinePosition.pivot;
		_outline.GetComponent<RectTransform>().offsetMin = Steps[0].OutlinePosition.offsetMin;
		_outline.GetComponent<RectTransform>().offsetMax = Steps[0].OutlinePosition.offsetMax;

		_hintText = Instantiate(Resources.Load<GameObject>("TutorialText"), transform);
		ResolveTextTransform(
			_hintText.GetComponent<RectTransform>(),
			_outline.GetComponent<RectTransform>(),
			Steps[0].TextOffset
		);
		_hintText.GetComponent<Text>().alignment = Steps[0].TextAlign;
		_hintText.GetComponent<Text>().text = Steps[0].HintText;

		_manager = transform.parent.GetComponent<TutorialManager>();
	}

	protected void Next()
	{
		if (++_index < Steps.Length)
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

		RectTransform start = Steps[_index - 1].OutlinePosition;
		RectTransform finish = Steps[_index].OutlinePosition;

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

		RectTransform outline = Steps[_index].OutlinePosition;
		Vector2Int textOffset = Steps[_index].TextOffset;

		RectTransform t = _hintText.GetComponent<RectTransform>();
		ResolveTextTransform(t, outline, textOffset);

		hintText.text = Steps[_index].HintText;
		hintText.alignment = Steps[_index].TextAlign;

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

	private static void ResolveTextTransform(
		RectTransform textTransform,
		RectTransform outlinePosition,
		Vector2Int textOffset
	)
	{
		Debug.Log(textOffset);
		
		textTransform.anchorMin = outlinePosition.anchorMin;
		textTransform.anchorMax = outlinePosition.anchorMax;
		textTransform.pivot = outlinePosition.pivot;
		
		textTransform.offsetMin = outlinePosition.offsetMin + textOffset * outlinePosition.rect.size;
		
		if (textOffset.x < 0)
		{
			Debug.Log("x<0");
			textTransform.offsetMin -= new Vector2(outlinePosition.rect.size.x * 9, 0);
		}

		if (textOffset.y < 0)
		{
			Debug.Log("y<0");
			textTransform.offsetMin -= new Vector2(0, outlinePosition.rect.size.y * 9);
		}

		textTransform.offsetMax = outlinePosition.offsetMax + textOffset * outlinePosition.rect.size;
		
		if (textOffset.x > 0)
		{
			Debug.Log("x>0");
			textTransform.offsetMax += new Vector2(outlinePosition.rect.size.x * 9, 0);
		}

		if (textOffset.y > 0)
		{
			Debug.Log("y>0");
			textTransform.offsetMax += new Vector2(0, outlinePosition.rect.size.y * 9);
		}
	}
}