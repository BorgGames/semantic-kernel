// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Text.Json.Serialization;

namespace Microsoft.SemanticKernel.Connectors.Anthropic.Core.Models.Tools;

internal sealed class ComputerUseTool2024_10_22 : AnthropicTool
{
    [JsonPropertyName("display_width_px")]
    public int DisplayWidth { get; init; }

    [JsonPropertyName("display_height_px")]
    public int DisplayHeight { get; init; }

    public ComputerUseTool2024_10_22(int displayWidth, int displayHeight)
    {
        this.Type = "computer_20241022";
        this.Name = "computer";
        if (displayWidth <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(displayWidth), "Display width must be greater than 0.");
        }
        if (displayHeight <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(displayHeight), "Display height must be greater than 0.");
        }
        this.DisplayWidth = displayWidth;
        this.DisplayHeight = displayHeight;
    }
}
