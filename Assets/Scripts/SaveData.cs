using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    public int gold;
    public int playerHp;
    public int currentStage;

    public List<CardSaveData> cards = new List<CardSaveData>();
}
