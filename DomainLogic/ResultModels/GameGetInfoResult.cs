﻿using DomainLogic.Models;

namespace DomainLogic.ResultModels
{
    public record GameGetInfoResult(
        bool Success = true,
        string Message = "",
        GameState? State = null,
        AwaitableMove? AwaitableMove = null);
}