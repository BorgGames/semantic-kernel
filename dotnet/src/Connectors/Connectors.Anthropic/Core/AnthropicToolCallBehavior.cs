// Copyright (c) Microsoft. All rights reserved.

using System;

namespace Microsoft.SemanticKernel.Connectors.Anthropic.Core;

internal static class AnthropicToolCallBehavior
{
    public static void ConfigureRequest(Kernel? kernel, AnthropicRequest request)
    {
        var functionsMetadata = kernel?.Plugins?.GetFunctionsMetadata();
        if (functionsMetadata is null)
        {
            return;
        }

        foreach (var functionMetadata in functionsMetadata)
        {
            request.AddTool(ToAnthropicTool(functionMetadata));
        }
    }

    private static AnthropicTool ToAnthropicTool(KernelFunctionMetadata functionMetadata)
    {
        return !functionMetadata.AdditionalProperties.TryGetValue("tool", out var toolObj)
               || toolObj is not AnthropicTool tool
            ? throw new NotImplementedException("General tool/function use has not been implemented yet")
            : tool;
    }
}
