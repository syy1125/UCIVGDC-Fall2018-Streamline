using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[Serializable]
public class MusicClip
{
	public AudioClip Clip;
	public string Name;
	public string Author;
}

public class MusicController : MonoBehaviour
{
	public static MusicController Instance;

	public AudioClip AmbientMusic;
	public MusicClip[] CircuitPlaylist;

	public AudioSource Source { get; private set; }
	private int _clipIndex;
	private Coroutine _volumeFade;

	private GameObject _nowPlaying;
	private Coroutine _moveNowPlaying;

	private void Awake()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
			return;
		}

		DontDestroyOnLoad(gameObject);
		Instance = this;
	}

	private void Start()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
		SceneManager.sceneUnloaded += OnSceneUnloaded;
		Source = GetComponent<AudioSource>();
		_clipIndex = Random.Range(0, CircuitPlaylist.Length);
	}

	private void Update()
	{
		// If music ends and is in circuit scene, play another clip.
		if (!Source.isPlaying && IsCircuitScene(SceneManager.GetActiveScene()))
		{
			RandomizeIndex();
			Source.clip = CircuitPlaylist[_clipIndex].Clip;
			Source.Play();

			UpdateNowPlaying();
		}
	}

	private void RandomizeIndex()
	{
		_clipIndex += Random.Range(1, CircuitPlaylist.Length);
		_clipIndex %= CircuitPlaylist.Length;
	}

	private void OnSceneLoaded(Scene s, LoadSceneMode m)
	{
		if (IsCircuitScene(s))
		{
			Source.Stop();
			RandomizeIndex();
			Source.clip = CircuitPlaylist[_clipIndex].Clip;
			Source.loop = false;
			Source.volume = 0;
			Source.Play();
			VolumeFade(1);

			_nowPlaying = GameObject.Find("Now Playing Text");
			UpdateNowPlaying();
		}
	}

	private void OnSceneUnloaded(Scene s)
	{
		if (IsCircuitScene(s))
		{
			Source.Stop();
			Source.clip = AmbientMusic;
			Source.loop = true;
			Source.volume = 0;
			Source.Play();
			VolumeFade(1);

			StopCoroutine(_moveNowPlaying);
		}
	}

	public void VolumeFade(float targetVolume, float duration = 1f)
	{
		if (_volumeFade != null)
		{
			StopCoroutine(_volumeFade);
		}

		_volumeFade = StartCoroutine(ExecVolumeFade(targetVolume, duration));
	}

	private IEnumerator ExecVolumeFade(float targetVolume, float duration)
	{
		float startVolume = Source.volume;
		float startTime = Time.time;

		while (Time.time - startTime < duration)
		{
			Source.volume = Mathf.Lerp(startVolume, targetVolume, (Time.time - startTime) / duration);
			yield return null;
		}

		Source.volume = targetVolume;
	}

	private void UpdateNowPlaying()
	{
		_nowPlaying.GetComponent<Text>().text = "Now Playing: " + CircuitPlaylist[_clipIndex].Name + " - by "
		                                        + CircuitPlaylist[_clipIndex].Author;

		if (_moveNowPlaying != null) StopCoroutine(_moveNowPlaying);
		_moveNowPlaying = StartCoroutine(MoveNowPlaying());
	}

	private IEnumerator MoveNowPlaying()
	{
		RectTransform nowPlayingTransform = _nowPlaying.GetComponent<RectTransform>();
		nowPlayingTransform.offsetMin = new Vector2(0, nowPlayingTransform.offsetMin.y);
		Text nowPlayingText = _nowPlaying.GetComponent<Text>();
		nowPlayingText.CrossFadeAlpha(1, 0, true);

		Debug.Log(_nowPlaying.GetComponent<Text>().preferredWidth);
		Debug.Log(nowPlayingTransform.GetComponentInParent<RectTransform>().rect.width);

		float moveDistance = _nowPlaying.GetComponent<Text>().preferredWidth
		                     - nowPlayingTransform.GetComponentInParent<RectTransform>().rect.width;

		if (moveDistance <= 0) yield break;

		while (true)
		{
			yield return new WaitForSeconds(2);

			while (nowPlayingTransform.offsetMin.x > -moveDistance)
			{
				nowPlayingTransform.offsetMin += Vector2.left * Time.deltaTime * 20f;
				yield return null;
			}

			yield return new WaitForSeconds(2);

			nowPlayingText.CrossFadeAlpha(0, 1, false);
			yield return new WaitForSeconds(1);

			nowPlayingTransform.offsetMin = new Vector2(0, nowPlayingTransform.offsetMin.y);

			nowPlayingText.CrossFadeAlpha(1, 1, false);
			yield return new WaitForSeconds(1);
		}
	}

	private bool IsCircuitScene(Scene s)
	{
		return s.name.Equals("CircuitGrid");
	}

	private void OnDestroy()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
		SceneManager.sceneUnloaded -= OnSceneUnloaded;
	}
}