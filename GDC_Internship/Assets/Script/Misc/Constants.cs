public static class Constants
{
    public const float Max_Time = 15 * 60;
}
[System.Serializable]
public class DialogueScript
{
    public DialogueLine[] Prelogue;
    public DialogueLine[] Chapter1;
    public DialogueLine[] Chapter2;
    public DialogueLine[] Chapter3;
    public DialogueLine[] Endgame;
}
[System.Serializable]
public struct DialogueLine
{
    public string name;
    public string line;
}
[System.Serializable]
public class SaveState
{
    public ChapterData[] ChapterStates;
    public int ActiveChapter;
    public void init()
    {
        ChapterStates = new ChapterData[5];
        for (int i = 0; i < ChapterStates.Length; i++)
        {
            float time = 0;
            int herb = 0;
            bool unlocked = false;
            if (i == 0)
            {
                herb = 3;
                time = Constants.Max_Time / 2;
                unlocked = true;
            }
            if (i == 1)
            {
                unlocked = true;
            }
            ChapterStates[i] = new ChapterData(i, herb, time, unlocked);
        }
    }
}
[System.Serializable]
public struct ChapterData
{
    public int Id;
    public int Herb;
    public float Time;
    public bool Unlocked;
    public ChapterData(int id, int herb, float time, bool unlocked)
    {
        Id = id;
        Herb = herb;
        Time = time;
        Unlocked = unlocked;
    }
}
