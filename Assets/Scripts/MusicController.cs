using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicController : MonoBehaviour
{
	public AudioClip AmbientMusic;
	public AudioClip[] CircuitPlaylist;

	private AudioSource _source;
	private int _clipIndex;

	private void Awake()
	{
		DontDestroyOnLoad(gameObject);
	}

	private void Start()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
		SceneManager.sceneUnloaded += OnSceneUnloaded;
		_source = GetComponent<AudioSource>();
		_clipIndex = Random.Range(0, CircuitPlaylist.Length);
	}

	private void Update()
	{
		// If music ends and is in circuit scene, play another clip.
		if (!_source.isPlaying && IsCircuitScene(SceneManager.GetActiveScene()))
		{
			RandomizeIndex();
			_source.clip = CircuitPlaylist[_clipIndex];
			_source.Play();
		}
	}

	private void RandomizeIndex()
	{
		_clipIndex += Random.Range(1, CircuitPlaylist.Length);
	}

	private void OnSceneLoaded(Scene s, LoadSceneMode m)
	{
		if (IsCircuitScene(s))
		{
			_source.Stop();
			RandomizeIndex();
			_source.clip = CircuitPlaylist[_clipIndex];
			_source.loop = false;
			_source.Play();
		}
	}

	private void OnSceneUnloaded(Scene s)
	{
		if (IsCircuitScene(s))
		{
			_source.Stop();
			_source.clip = AmbientMusic;
			_source.loop = true;
			_source.Play();
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