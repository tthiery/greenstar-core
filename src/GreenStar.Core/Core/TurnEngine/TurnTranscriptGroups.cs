namespace GreenStar.Core.TurnEngine
{
    public static class TurnTranscriptGroups
    {
        public const int InitTurn = 0;
        public const int StartAdministration = 10;
        
        public const int UniverseLife = 100; // e.g. population grow, star rotation, ..
        public const int Moves = 200;
        public const int MovementDone = 210;

        public const int UnverseLifeAfterUnrest = 300;

        public const int EndAdministration = 400;
        public const int EndTurn = 410;
    }
}