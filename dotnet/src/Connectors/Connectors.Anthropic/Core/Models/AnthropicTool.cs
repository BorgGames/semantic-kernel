// Copyright (c) Microsoft. All rights reserved.

using System.Text.Json.Serialization;
using Microsoft.SemanticKernel.Connectors.Anthropic.Core.Models.Tools;

namespace Microsoft.SemanticKernel.Connectors.Anthropic.Core;

[JsonDerivedType(typeof(ComputerUseTool2024_10_22))]
internal class AnthropicTool
{
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }
}
