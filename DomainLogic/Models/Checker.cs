namespace DomainLogic.Models
{
    public enum CheckerRole { Men, King }
    public record Checker(string Color, CheckerRole Role = CheckerRole.Men);
}