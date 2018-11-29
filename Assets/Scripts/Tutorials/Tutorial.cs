using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
	private TutorialStep[] _steps;

	private int _index;
	protected GameObject Outline { get; private set; }
	protected GameObject HintText { get; private set; }
	protected TutorialManager Manager { get; private set; }

	private GameObject _backdropTL;
	private GameObject _backdropTC;
	private GameObject _backdropTR;
	private GameObject _backdropML;
	private GameObject _backdropMR;
	private GameObject _backdropBL;
	private GameObject _backdropBC;
	private GameObject _backdropBR;

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
			|| GameController.gameLevel.Tutorial == null
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
		Manager = transform.parent.GetComponent<TutorialManager>();

		Outline = Instantiate(Resources.Load<GameObject>("TutorialOutline"), transform);
		HintText = Instantiate(Resources.Load<GameObject>("TutorialText"), transform);

		GameObject backdropPrefab = Resources.Load<GameObject>("TutorialBackdrop");
		Transform backdropTransform = Manager.BackdropCanvas.transform;
		_backdropTL = Instantiate(backdropPrefab, backdropTransform);
		_backdropTC = Instantiate(backdropPrefab, backdropTransform);
		_backdropTR = Instantiate(backdropPrefab, backdropTransform);
		_backdropML = Instantiate(backdropPrefab, backdropTransform);
		_backdropMR = Instantiate(backdropPrefab, backdropTransform);
		_backdropBL = Instantiate(backdropPrefab, backdropTransform);
		_backdropBC = Instantiate(backdropPrefab, backdropTransform);
		_backdropBR = Instantiate(backdropPrefab, backdropTransform);

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
		RectTransform o = Outline.GetComponent<RectTransform>();
		RectTransform tl = _backdropTL.GetComponent<RectTransform>();
		RectTransform tc = _backdropTC.GetComponent<RectTransform>();
		RectTransform tr = _backdropTR.GetComponent<RectTransform>();
		RectTransform ml = _backdropML.GetComponent<RectTransform>();
		RectTransform mr = _backdropMR.GetComponent<RectTransform>();
		RectTransform bl = _backdropBL.GetComponent<RectTransform>();
		RectTransform bc = _backdropBC.GetComponent<RectTransform>();
		RectTransform br = _backdropBR.GetComponent<RectTransform>();

		RectTransform start = Instantiate(Resources.Load<GameObject>("TutorialOutline"), transform)
			.GetComponent<RectTransform>();
		start.gameObject.SetActive(false);
		MatchTransform(start, o);
		RectTransform finish = _steps[_index].Position;

		float startTime = Time.time;

		while (Time.time - startTime < Manager.MoveDuration)
		{
			float proportion = Manager.OutlineCurve.Evaluate((Time.time - startTime) / Manager.MoveDuration);

			o.anchorMin = Vector2.Lerp(start.anchorMin, finish.anchorMin, proportion);
			o.anchorMax = Vector2.Lerp(start.anchorMax, finish.anchorMax, proportion);
			o.offsetMin = Vector2.Lerp(start.offsetMin, finish.offsetMin, proportion);
			o.offsetMax = Vector2.Lerp(start.offsetMax, finish.offsetMax, proportion);

			tl.anchorMin = new Vector2(0, o.anchorMax.y);
			tl.anchorMax = new Vector2(o.anchorMin.x, 1);
			tl.offsetMin = new Vector2(0, o.offsetMax.y);
			tl.offsetMax = new Vector2(o.offsetMin.x, 0);

			tc.anchorMin = new Vector2(o.anchorMin.x, o.anchorMax.y);
			tc.anchorMax = new Vector2(o.anchorMax.x, 1);
			tc.offsetMin = new Vector2(o.offsetMin.x, o.offsetMax.y);
			tc.offsetMax = new Vector2(o.offsetMax.x, 0);

			tr.anchorMin = o.anchorMax;
			tr.anchorMax = Vector2.one;
			tr.offsetMin = o.offsetMax;
			tr.offsetMax = Vector2.zero;

			ml.anchorMin = new Vector2(0, o.anchorMin.y);
			ml.anchorMax = new Vector2(o.anchorMin.x, o.anchorMax.y);
			ml.offsetMin = new Vector2(0, o.offsetMin.y);
			ml.offsetMax = new Vector2(o.offsetMin.x, o.offsetMax.y);

			mr.anchorMin = new Vector2(o.anchorMax.x, o.anchorMin.y);
			mr.anchorMax = new Vector2(1, o.anchorMax.y);
			mr.offsetMin = new Vector2(o.offsetMax.x, o.offsetMin.y);
			mr.offsetMax = new Vector2(0, o.offsetMax.y);

			bl.anchorMin = Vector2.zero;
			bl.anchorMax = o.anchorMin;
			bl.offsetMin = Vector2.zero;
			bl.offsetMax = o.offsetMin;

			bc.anchorMin = new Vector2(o.anchorMin.x, 0);
			bc.anchorMax = new Vector2(o.anchorMax.x, o.anchorMin.y);
			bc.offsetMin = new Vector2(o.offsetMin.x, 0);
			bc.offsetMax = new Vector2(o.offsetMax.x, o.offsetMin.y);

			br.anchorMin = new Vector2(o.anchorMax.x, 0);
			br.anchorMax = new Vector2(1, o.anchorMin.y);
			br.offsetMin = new Vector2(o.offsetMax.x, 0);
			br.offsetMax = new Vector2(0, o.offsetMin.y);

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
		lvalue.offsetMin = rvalue.offsetMin;
		lvalue.offsetMax = rvalue.offsetMax;
	}

	private IEnumerator FadeOut()
	{
		Outline.GetComponent<Image>().CrossFadeAlpha(0, Manager.TextFadeDuration, false);
		HintText.GetComponent<Text>().CrossFadeAlpha(0, Manager.TextFadeDuration, false);

		yield return new WaitForSeconds(Manager.TextFadeDuration);

		gameObject.SetActive(false);
	}
}