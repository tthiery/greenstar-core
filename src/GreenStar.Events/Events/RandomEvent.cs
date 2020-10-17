namespace GreenStar.Events
{
    public class RandomEvent
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Argument { get; set; }
        public bool IsReturning { get; set; }
        public double Prohability { get; set; }
        public string[] RequiredTechnologies { get; set; }
        public string[] BlockingTechnologies { get; set; }
        public string Text { get; set; }
    }
}