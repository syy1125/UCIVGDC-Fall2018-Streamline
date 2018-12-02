using System.Collections;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicController : MonoBehaviour
{
	public static MusicController Instance;

	public AudioClip AmbientMusic;
	public AudioClip[] CircuitPlaylist;

	public AudioSource Source { get; private set; }
	private int _clipIndex;
	private Coroutine _volumeFade;

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
			Source.clip = CircuitPlaylist[_clipIndex];
			Source.Play();
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
			Source.clip = CircuitPlaylist[_clipIndex];
			Source.loop = false;
			Source.volume = 0;
			Source.Play();
			VolumeFade(1);
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
		}
	}

	public void VolumeFade(float targetVolume, float duration = 1f)
	{
		Debug.Log(System.Environment.StackTrace);

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