using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
	[Header("Object Containers")]
	[SerializeField]
	private GameObject _menuContainer;
	[SerializeField]
	private GameObject _gameContainer;
	[SerializeField]
	private GameObject _gameUIContainer;

	[SerializeField]
	private List<Mole> _moles;

	[Header("Gameplay values")]
	[SerializeField, Tooltip("The number of hits needed to activate an extra mole.")]
	private int _activateExtraMoleThreshold = 10;
	[SerializeField]
	private int _defaultScoreValue = 1;
	[SerializeField]
	private int _timePenaltyMissedMole = 2;

	[Header("UI Fields")]
	[SerializeField]
	private UIViewsManager _uiManager;

	private float _playTimeSecs = 30f;
	private float _timeRemainingSecs;
	
	private HashSet<Mole> _activeMoles = new HashSet<Mole>();

	private int _score;
	private bool _playing = false;

	private void Awake()
	{
		SetScreenState(false);
	}

	private void Start()
	{
		// If this threshold is set to 0 it will result in an error 
		if(_activateExtraMoleThreshold <= 0)
		{
			Debug.LogWarning($"You have set the {nameof(_activateExtraMoleThreshold)} to '0' or less, resulting it in automatically being set to 1.");
			_activateExtraMoleThreshold = 1;
		}
	}

	private void Update()
	{
		if(!_playing)
		{
			return;
		}

		_timeRemainingSecs -= Time.deltaTime;
		if(_timeRemainingSecs < 0)
		{
			_timeRemainingSecs = 0;
			StopGame();
		}
		_uiManager.UpdateTimer(_timeRemainingSecs);

		// Activate more moles every X hits
		if(_activeMoles.Count <= _score / _activateExtraMoleThreshold)
		{
			int index = Random.Range(0, _moles.Count);
			// For now doens't matter if that mole is already active.
			// In that case, a new random mole will be tried in the next frame
			if(!_activeMoles.Contains(_moles[index]))
			{
				Mole newMole = _moles[index];
				_activeMoles.Add(newMole);
				newMole.Activate();
			}
		}
	}

	private void StartGame()
	{
		// Make sure all mole are hidden and have a unique identifier
		for(int i = 0; i < _moles.Count; i++)
		{
			_moles[i].Initialize(this, i);
		}
		
		// Reset the game's state
		_activeMoles.Clear();
		_timeRemainingSecs = _playTimeSecs;
		_score = 0;
		_uiManager.UpdateScore(_score);
		_uiManager.UpdateTimer(_timeRemainingSecs);
		
		SetScreenState(true);
		
		_playing = true;
	}

	private void StopGame()
	{
		for(var i = 0; i < _moles.Count; i++)
		{
			_moles[i].Deactivate();
		}
		
		_playing = false;
		SetScreenState(false);
	}

	private void SetScreenState(bool showGame)
	{
		_menuContainer.SetActive(!showGame);
		_gameContainer.SetActive(showGame);
		_gameUIContainer.SetActive(showGame);
	}

	public void HandleMoleHit(int index)
	{
		_score += _defaultScoreValue;
		_uiManager.UpdateScore(_score);
		_activeMoles.Remove(_moles[index]);
	}

	public void HandleMoleMiss(int index, bool isMole)
	{
		_timeRemainingSecs -= _timePenaltyMissedMole;
		_activeMoles.Remove(_moles[index]);
		
		// For later: reset streaks
	}
	
	public void OnStartButtonClicked()
	{
		StartGame();
	}
}