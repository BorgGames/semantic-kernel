// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.SemanticKernel.Connectors.Anthropic.Core.Models.Tools;

namespace Microsoft.SemanticKernel.Connectors.Anthropic.Core;

/// <summary>
/// Represents a computer tool.
/// https://docs.anthropic.com/en/docs/build-with-claude/computer-use
/// </summary>
[Obsolete("This functionality is in beta and may change in the future.")]
public static class ComputerUse_2024_10_22
{
    internal const string ComputerToolType = "computer_20241022";

    /// <summary>
    /// <see cref="AnthropicClientOptions.Beta"/>
    /// </summary>
    public const string Beta = "computer-use-2024-10-22";

    /// <summary>
    /// Creates a computer plugin.
    /// </summary>
    public static KernelPlugin ComputerPlugin(IComputer computer)
        => Computer(Wrap(computer), width: computer.DisplayWidth, height: computer.DisplayHeight);

    public interface IComputer
    {
        int DisplayWidth { get; }
        int DisplayHeight { get; }

        Task Key(string key);
        Task Type(string text);
        Task MouseMove(int x, int y);
        /// <summary>
        /// Press left mouse button and drag to the specified location, then release.
        /// </summary>
        Task LeftClickDrag(int x, int y);
        Task LeftClick();
        Task RightClick();
        Task MiddleClick();
        Task DoubleClick();
        /// <summary>
        /// Base64-encoded PNG image.
        /// </summary>
        Task<string> Screenshot();
        Task<(int, int)> GetCursorPosition();
    }

    private delegate Task<string?> ComputerDelegate(Action action, string? text, (int, int)? coord);

    private static KernelPlugin Computer(ComputerDelegate computer, int width, int height)
    {
        var computerFunction = KernelFunctionFactory.CreateFromMethod(
            method: computer.Method,
            target: computer.Target,
            new KernelFunctionFromMethodOptions
            {
                FunctionName = "computer",
                AdditionalMetadata = new(new Dictionary<string, object?>
                {
                    ["tool"] = new ComputerUseTool2024_10_22(width, height),
                }),
            });
        return KernelPluginFactory.CreateFromFunctions(
            pluginName: ComputerToolType,
            [computerFunction]);
    }

    private static ComputerDelegate Wrap(IComputer computer)
    {
        return async (action, text, coord) =>
        {
            switch (action)
            {
                case Action.mouse_move or Action.left_click_drag:
                    if (coord is not (var x, var y))
                    {
                        throw new ArgumentNullException(nameof(coord), "coord required for " + action);
                    }

                    if (text is not null)
                    {
                        throw new ArgumentOutOfRangeException(nameof(text), text, "text must be null for mouse actions");
                    }

                    if (x < 0 || y < 0 || x >= computer.DisplayWidth || y >= computer.DisplayHeight)
                    {
                        throw new ArgumentOutOfRangeException(nameof(coord), coord, $"coord out of {computer.DisplayWidth}x{computer.DisplayHeight}");
                    }

                    if (action is Action.mouse_move)
                    {
                        await computer.MouseMove(x, y).ConfigureAwait(false);
                    }
                    else
                    {
                        await computer.LeftClickDrag(x, y).ConfigureAwait(false);
                    }

                    return null;

                case Action.key or Action.type:
                    if (text is null)
                    {
                        throw new ArgumentNullException(nameof(text), "text required for " + action);
                    }

                    if (coord is not null)
                    {
                        throw new ArgumentOutOfRangeException(nameof(coord), coord, "coord must be null for keyboard actions");
                    }

                    if (action is Action.key)
                    {
                        await computer.Key(text).ConfigureAwait(false);
                    }
                    else
                    {
                        await computer.Type(text).ConfigureAwait(false);
                        return await computer.Screenshot().ConfigureAwait(false);
                    }
                    return null;

                case Action.left_click or Action.right_click or Action.middle_click
                   or Action.double_click
                   or Action.cursor_position
                   or Action.screenshot:
                    if (text is not null)
                    {
                        throw new ArgumentOutOfRangeException(nameof(text), text, "text must be null for " + action);
                    }

                    if (coord is not null)
                    {
                        throw new ArgumentOutOfRangeException(nameof(coord), coord, "coord must be null for " + action);
                    }

                    switch (action)
                    {
                        case Action.left_click:
                            await computer.LeftClick().ConfigureAwait(false);
                            break;
                        case Action.right_click:
                            await computer.RightClick().ConfigureAwait(false);
                            break;
                        case Action.middle_click:
                            await computer.MiddleClick().ConfigureAwait(false);
                            break;
                        case Action.double_click:
                            await computer.DoubleClick().ConfigureAwait(false);
                            break;
                        case Action.cursor_position:
                            (x, y) = await computer.GetCursorPosition().ConfigureAwait(false);
                            return FormattableString.Invariant($"X={x},Y={y}");
                        case Action.screenshot:
                            return await computer.Screenshot().ConfigureAwait(false);
                    }
                    return null;

                default:
                    throw new ArgumentOutOfRangeException(nameof(action), action, "invalid action");
            }
        };
    }

    internal enum Action
    {
        key,
        type,
        mouse_move,
        left_click,
        left_click_drag,
        right_click,
        middle_click,
        double_click,
        screenshot,
        cursor_position,
    }
}
