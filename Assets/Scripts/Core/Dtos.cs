using System;
using System.Collections.Generic;

[Serializable]
public class CreateRoomResponse
{
    public string roomId;
}

[Serializable]
public class StartGameRequest
{
    public List<string> playerIds;
}

[Serializable]
public class UseCardRequest
{
    public string userId;
    public string targetId;
    public string cardId;
}

[Serializable]
public class StateResponse
{
    public int round;
    public string phase;
    public PlayerSelfView self;
    public List<PlayerPublicView> publics;
}

[Serializable]
public class PlayerPublicView
{
    public string playerId;
    public string displayName;
    public bool isAlive;
    public bool hasInspectableCorpse;
}

[Serializable]
public class CardClientView
{
    public string cardId;
    public int type;
}

[Serializable]
public class PhaseNextResponse
{
    public int round;
    public string phase;
    public bool isGameOver;
}

[Serializable]
public class DrawCardRequest
{
    public string userId;
    public bool isRedChest;
}

[Serializable]
public class NightIntentRequest
{
    public string userId;
    public bool intendToSteal;
}

[Serializable]
public class PlayerSelfView
{
    public string playerId;
    public string displayName;
    public int currentHp;
    public int maxHp;
    public int currentAp;
    public int? poisonStacks;
    public bool hasBulletProofBuff;
    public List<CardClientView> hand;
    public bool isAlive;
    public bool holdsTreasure;

    // 新增（后端没返回也不会报错，默认 null）
    public string role;
}