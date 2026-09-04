public static class GameResultData
{
    public static bool isClear {  get; private set; }

    public static int FinalScore {  get; private set; }
    public static int ReachedStage {  get; private set; }
    public static int PlayerHp {  get; private set; }
    public static int Gold {  get; private set; }
    public static int CardCount {  get; private set; }

    public static void SetClearResult(
        int finalScore,
        int reachedStage,
        int playerHp,
        int gold,
        int cardCount)
    {
        isClear = true;
        FinalScore = finalScore;
        ReachedStage = reachedStage;
        PlayerHp = playerHp;
        Gold = gold;
        CardCount = cardCount;
    }

    public static void SetGameOverResult(int reachedStage)
    {
        isClear = false;
        FinalScore = 0;
        ReachedStage=reachedStage;
        PlayerHp = 0;
        Gold = 0;
        CardCount = 0;
    }
}
