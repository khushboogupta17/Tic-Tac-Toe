using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Playingoptions
{
    cross,
    circle
}
[System.Serializable]
[CreateAssetMenu(fileName = "New Player data",menuName = " Player Data")]
public class PlayerData : ScriptableObject
{
    [SerializeField]
    private string _playerName;
    [SerializeField]
    private float _score;
    [SerializeField]
    private Sprite _turnSprite;
    [SerializeField]
    private Playingoptions _playingOption;

    public string PlayerName
    {
        get
        {
            return _playerName;
        }
    }

    public float Score
    {
        get
        {
            return _score;

        }
        set
        {
            _score=value;
        }
    }

    public Sprite TurnSprite
    {
        get
        {
            return _turnSprite;
        }
    }

    public Playingoptions PlayingOptions
    {
        get
        {
            return _playingOption;
        }
    }

    private void OnEnanle()
    {
        Debug.Log("awake called");
        Score = 0;
    }
}
