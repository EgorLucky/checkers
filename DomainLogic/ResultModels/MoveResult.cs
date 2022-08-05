namespace DomainLogic.Models
{
    public record MoveResult(
        bool Success = false, 
        string Message = "",
        BoardState NewBoardState = null,
        GamePlayer? AwaitableMove = null);
}