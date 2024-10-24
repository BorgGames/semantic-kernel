// Copyright (c) Microsoft. All rights reserved.

using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Microsoft.SemanticKernel.Connectors.Anthropic.Core;

internal sealed class AnthropicContent
{
    /// <summary>
    /// Used to return tool call results.
    /// </summary>
    [JsonPropertyName("content")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Content { get; set; }

    [JsonPropertyName("id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ID { get; set; }

    [JsonPropertyName("name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Name { get; set; }

    [JsonPropertyName("input")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public JsonObject? Input { get; set; }

    [JsonPropertyName("tool_use_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ToolUseID { get; set; }

    /// <summary>
    /// Currently supported only base64.
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; }

    /// <summary>
    /// When type is "text", the text content.
    /// </summary>
    [JsonPropertyName("text")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Text { get; set; }

    /// <summary>
    /// When type is "image", the source of the image.
    /// </summary>
    [JsonPropertyName("source")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public SourceEntity? Source { get; set; }

    [JsonConstructor]
    public AnthropicContent(string type)
    {
        this.Type = type;
    }

    internal sealed class SourceEntity
    {
        /// <summary>
        /// Currently supported only base64.
        /// </summary>
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        /// <summary>
        /// The media type of the image.
        /// </summary>
        [JsonPropertyName("media_type")]
        public string? MediaType { get; set; }

        /// <summary>
        /// The base64 encoded image data.
        /// </summary>
        [JsonPropertyName("data")]
        public string? Data { get; set; }
    }
}
