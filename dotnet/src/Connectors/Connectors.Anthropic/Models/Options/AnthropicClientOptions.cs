// Copyright (c) Microsoft. All rights reserved.

using System;

namespace Microsoft.SemanticKernel.Connectors.Anthropic;

/// <summary>
/// Represents the options for configuring the Anthropic client with Anthropic provider.
/// </summary>
public sealed class AnthropicClientOptions : ClientOptions
{
    internal const ServiceVersion LatestVersion = ServiceVersion.V2023_06_01;

    internal string? Beta { get; init; }

    /// <summary> The version of the service to use. </summary>
    public enum ServiceVersion
    {
        /// <summary> Service version "2023-01-01". </summary>
        V2023_01_01,

        /// <summary> Service version "2023-06-01". </summary>
        V2023_06_01,
    }

    /// <summary>
    /// Initializes new instance of <see cref="AnthropicClientOptions"/>
    /// </summary>
    /// <param name="version">
    /// This parameter is optional.
    /// Default value is <see cref="AnthropicClientOptions.LatestVersion"/>.<br/>
    /// </param>
    /// <exception cref="NotSupportedException">Provided version is not supported.</exception>
    public AnthropicClientOptions(ServiceVersion version = LatestVersion, string? beta = null) : base(ToString(version))
    {
        this.Beta = beta;
    }

    private static string ToString(ServiceVersion version) => version switch
    {
        ServiceVersion.V2023_01_01 => "2023-01-01",
        ServiceVersion.V2023_06_01 => "2023-06-01",
        _ => throw new NotSupportedException("Unsupported service version")
    };
}
