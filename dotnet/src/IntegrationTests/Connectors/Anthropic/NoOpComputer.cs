// Copyright (c) Microsoft. All rights reserved.

using System.Threading.Tasks;
using Microsoft.SemanticKernel.Connectors.Anthropic.Core;

namespace SemanticKernel.IntegrationTests.Connectors.Anthropic;

internal class NoOpComputer : ComputerUse_2024_10_22.IComputer
{
    public static NoOpComputer Instance { get; } = new();

    public int DisplayWidth => 1024;

    public int DisplayHeight => 768;

    public (int, int) CursorPosition { get; set; } = (0, 0);

    public Task<(int, int)> GetCursorPosition() => Task.FromResult(this.CursorPosition);

    public Task DoubleClick() => Task.CompletedTask;

    public Task Key(string key) => Task.CompletedTask;

    public Task LeftClick() => Task.CompletedTask;

    public Task LeftClickDrag(int x, int y) => Task.CompletedTask;

    public Task MiddleClick() => Task.CompletedTask;

    public Task MouseMove(int x, int y) => Task.CompletedTask;

    public Task RightClick() => Task.CompletedTask;

    public Task<string> Screenshot() => Task.FromResult(string.Empty);

    public Task Type(string text) => Task.CompletedTask;
}
