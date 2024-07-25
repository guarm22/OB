[System.Serializable]
public class PlayerData {
    public Stat ReportsMade;
    public Stat DivergencesReported;
    public Stat CreaturesReported;
    public Stat TimesCrouched;
    public Stat TimesPlayedOnEasy;
    public Stat TimesPlayedOnNormal;
    public Stat TimesPlayedOnHard;
    public Stat TimeInLevel;

    public PlayerData() {
        ReportsMade = new Stat("Reports Made", 0);
        DivergencesReported = new Stat("Divergences Reported", 0);
        CreaturesReported = new Stat("Creatures Reported", 0);
        TimesCrouched = new Stat("Times Crouched", 0);
        TimesPlayedOnEasy = new Stat("Levels Played on Easy", 0);
        TimesPlayedOnNormal = new Stat("Levels Played on Normal", 0);
        TimesPlayedOnHard = new Stat("Levels Played on Hard", 0);
        TimeInLevel = new Stat("Time In Level", 0, " seconds");
    }
}
