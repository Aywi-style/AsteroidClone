using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Client;

public class GameState
{
    private static GameState _gameState = null;

    private int _playerScore = 0;

    #region Configs
    public PlayerShipConfig PlayerShipConfig { get; private set; }
    public AsteroidsConfig AsteroidsConfig { get; private set; }
    public UFOsConfig UFOsConfig { get; private set; }
    #endregion

    public AllPools AllEnabledPools;

    public static int NULL_ENTITY = -1;
    public int InterfaceEntity;

    #region SystemsBools
    public bool PlayFightSystems = true;
    public bool PlaySpawnSystems = true;
    public bool PlayControleAndMovableSystems = true;
    #endregion

    private GameState(in EcsStartup ecsStartup)
    {
        PlayerShipConfig = ecsStartup.PlayerShipConfig;
        AsteroidsConfig = ecsStartup.AsteroidsConfig;
        UFOsConfig = ecsStartup.UFOsConfig;
    }

    public static GameState Initialize(in EcsStartup ecsStartup)
    {
        if (_gameState is null)
        {
            _gameState = new GameState(in ecsStartup);
        }

        return _gameState;
    }

    public static GameState Get()
    {
        return _gameState;
    }

    public static void Clear()
    {
        _gameState = null;
    }

    public void AddScore(int value)
    {
        if (value < 0)
        {
            Debug.LogError("Has come incorrect score value!");
            return;
        }

        if (value == 0)
        {
            Debug.LogWarning("Has come 0 score value!");
        }

        _playerScore += value;
    }

    public int GetScore()
    {
        return _playerScore;
    }
}
