namespace DomainLogic.Models
{
    public enum CheckerRole { Men, King }
    public record Checker(CheckerColor Color, CheckerRole Role = CheckerRole.Men);
}