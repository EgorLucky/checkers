using System;

namespace DomainLogic.Models
{
    public record PlayerGameData(
        Guid GameId,
        Guid PlayerCode);
}
