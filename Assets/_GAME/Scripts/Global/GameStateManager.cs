using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameStateManager : Singleton<GameStateManager>
{
    public GameState _gameState;
}


public enum GameState
{
    NONE,
    Singleplayer,
    Multiplayer
}
