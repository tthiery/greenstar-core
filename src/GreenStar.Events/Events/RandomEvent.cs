namespace GreenStar.Events;

public record RandomEvent(string Name, string Type, string Argument, bool IsReturning, double Prohability, string[] RequiredTechnologies, string[] BlockingTechnologies, string Text);
