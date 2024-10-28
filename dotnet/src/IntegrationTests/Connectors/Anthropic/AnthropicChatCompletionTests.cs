﻿// Copyright (c) Microsoft. All rights reserved.

using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Anthropic;
using Microsoft.SemanticKernel.Connectors.Anthropic.Core;
using xRetry;
using Xunit;
using Xunit.Abstractions;

namespace SemanticKernel.IntegrationTests.Connectors.Anthropic;

public sealed class AnthropicChatCompletionTests(ITestOutputHelper output) : TestBase(output)
{
    [RetryTheory]
    [InlineData(ServiceType.Anthropic, Skip = "This test is for manual verification.")]
    [InlineData(ServiceType.VertexAI, Skip = "This test is for manual verification.")]
    [InlineData(ServiceType.AmazonBedrock, Skip = "This test is for manual verification.")]
    public async Task ChatGenerationReturnsValidResponseAsync(ServiceType serviceType)
    {
        // Arrange
        var chatHistory = new ChatHistory();
        chatHistory.AddUserMessage("Hello, I'm Brandon, how are you?");
        chatHistory.AddAssistantMessage("I'm doing well, thanks for asking.");
        chatHistory.AddUserMessage("Call me by my name and expand this abbreviation: LLM");

        var sut = this.GetChatService(serviceType);

        // Act
        var response = await sut.GetChatMessageContentAsync(chatHistory);

        // Assert
        Assert.NotNull(response.Content);
        this.Output.WriteLine(response.Content);
        Assert.Contains("Large Language Model", response.Content, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Brandon", response.Content, StringComparison.OrdinalIgnoreCase);
    }

    [RetryTheory]
    [InlineData(ServiceType.Anthropic, Skip = "This test is for manual verification.")]
    [InlineData(ServiceType.VertexAI, Skip = "This test is for manual verification.")]
    [InlineData(ServiceType.AmazonBedrock, Skip = "This test is for manual verification.")]
    public async Task ChatStreamingReturnsValidResponseAsync(ServiceType serviceType)
    {
        // Arrange
        var chatHistory = new ChatHistory();
        chatHistory.AddUserMessage("Hello, I'm Brandon, how are you?");
        chatHistory.AddAssistantMessage("I'm doing well, thanks for asking.");
        chatHistory.AddUserMessage("Call me by my name and write a long story about my name.");

        var sut = this.GetChatService(serviceType);

        // Act
        var response =
            await sut.GetStreamingChatMessageContentsAsync(chatHistory).ToListAsync();

        // Assert
        Assert.NotEmpty(response);
        Assert.True(response.Count > 1);
        var message = string.Concat(response.Select(c => c.Content));
        Assert.False(string.IsNullOrWhiteSpace(message));
        this.Output.WriteLine(message);
    }

    [RetryTheory]
    [InlineData(ServiceType.Anthropic, Skip = "This test is for manual verification.")]
    [InlineData(ServiceType.VertexAI, Skip = "This test is for manual verification.")]
    [InlineData(ServiceType.AmazonBedrock, Skip = "This test is for manual verification.")]
    public async Task ChatGenerationVisionBinaryDataAsync(ServiceType serviceType)
    {
        // Arrange
        Memory<byte> image = await File.ReadAllBytesAsync("./TestData/test_image_001.jpg");
        var chatHistory = new ChatHistory();
        var messageContent = new ChatMessageContent(AuthorRole.User, items:
        [
            new TextContent("This is an image with a car. Which color is it? You can chose from red, blue, green, and yellow"),
            new ImageContent(image, "image/jpeg")
        ]);
        chatHistory.Add(messageContent);

        var sut = this.GetChatService(serviceType);

        // Act
        var response = await sut.GetChatMessageContentAsync(chatHistory);

        // Assert
        Assert.NotNull(response.Content);
        this.Output.WriteLine(response.Content);
        Assert.Contains("green", response.Content, StringComparison.OrdinalIgnoreCase);
    }

    [RetryTheory]
    [InlineData(ServiceType.Anthropic, Skip = "This test is for manual verification.")]
    [InlineData(ServiceType.VertexAI, Skip = "This test is for manual verification.")]
    [InlineData(ServiceType.AmazonBedrock, Skip = "This test is for manual verification.")]
    public async Task ChatStreamingVisionBinaryDataAsync(ServiceType serviceType)
    {
        // Arrange
        Memory<byte> image = await File.ReadAllBytesAsync("./TestData/test_image_001.jpg");
        var chatHistory = new ChatHistory();
        var messageContent = new ChatMessageContent(AuthorRole.User, items:
        [
            new TextContent("This is an image with a car. Which color is it? You can chose from red, blue, green, and yellow"),
            new ImageContent(image, "image/jpeg")
        ]);
        chatHistory.Add(messageContent);

        var sut = this.GetChatService(serviceType);

        // Act
        var responses = await sut.GetStreamingChatMessageContentsAsync(chatHistory).ToListAsync();

        // Assert
        Assert.NotEmpty(responses);
        var message = string.Concat(responses.Select(c => c.Content));
        Assert.False(string.IsNullOrWhiteSpace(message));
        this.Output.WriteLine(message);
        Assert.Contains("green", message, StringComparison.OrdinalIgnoreCase);
    }

    [RetryTheory]
    [InlineData(ServiceType.Anthropic, Skip = "This test needs setup first.")]
    [InlineData(ServiceType.VertexAI, Skip = "This test needs setup first.")]
    [InlineData(ServiceType.AmazonBedrock, Skip = "This test needs setup first.")]
    public async Task ChatGenerationVisionUriAsync(ServiceType serviceType)
    {
        // Arrange
        Uri imageUri = new("gs://generativeai-downloads/images/scones.jpg"); // needs setup
        var chatHistory = new ChatHistory();
        var messageContent = new ChatMessageContent(AuthorRole.User, items:
        [
            new TextContent("This is an image with a car. Which color is it? You can chose from red, blue, green, and yellow"),
            new ImageContent(imageUri) { MimeType = "image/jpeg" }
        ]);
        chatHistory.Add(messageContent);

        var sut = this.GetChatService(serviceType);

        // Act
        var response = await sut.GetChatMessageContentAsync(chatHistory);

        // Assert
        Assert.NotNull(response.Content);
        this.Output.WriteLine(response.Content);
        Assert.Contains("green", response.Content, StringComparison.OrdinalIgnoreCase);
    }

    [RetryTheory]
    [InlineData(ServiceType.Anthropic, Skip = "This test needs setup first.")]
    [InlineData(ServiceType.VertexAI, Skip = "This test needs setup first.")]
    [InlineData(ServiceType.AmazonBedrock, Skip = "This test needs setup first.")]
    public async Task ChatStreamingVisionUriAsync(ServiceType serviceType)
    {
        // Arrange
        Uri imageUri = new("gs://generativeai-downloads/images/scones.jpg"); // needs setup
        var chatHistory = new ChatHistory();
        var messageContent = new ChatMessageContent(AuthorRole.User, items:
        [
            new TextContent("This is an image with a car. Which color is it? You can chose from red, blue, green, and yellow"),
            new ImageContent(imageUri) { MimeType = "image/jpeg" }
        ]);
        chatHistory.Add(messageContent);

        var sut = this.GetChatService(serviceType);

        // Act
        var responses = await sut.GetStreamingChatMessageContentsAsync(chatHistory).ToListAsync();

        // Assert
        Assert.NotEmpty(responses);
        var message = string.Concat(responses.Select(c => c.Content));
        Assert.False(string.IsNullOrWhiteSpace(message));
        this.Output.WriteLine(message);
        Assert.Contains("green", message, StringComparison.OrdinalIgnoreCase);
    }

    [RetryTheory]
    [InlineData(ServiceType.Anthropic, Skip = "This test is for manual verification.")]
    [InlineData(ServiceType.VertexAI, Skip = "This test is for manual verification.")]
    [InlineData(ServiceType.AmazonBedrock, Skip = "This test is for manual verification.")]
    public async Task ChatGenerationReturnsUsedTokensAsync(ServiceType serviceType)
    {
        // Arrange
        var chatHistory = new ChatHistory();
        chatHistory.AddUserMessage("Hello, I'm Brandon, how are you?");
        chatHistory.AddAssistantMessage("I'm doing well, thanks for asking.");
        chatHistory.AddUserMessage("Call me by my name and expand this abbreviation: LLM");

        var sut = this.GetChatService(serviceType);

        // Act
        var response = await sut.GetChatMessageContentAsync(chatHistory);

        // Assert
        var metadata = response.Metadata as AnthropicMetadata;
        Assert.NotNull(metadata);
        foreach ((string? key, object? value) in metadata)
        {
            this.Output.WriteLine($"{key}: {JsonSerializer.Serialize(value)}");
        }

        Assert.True(metadata.TotalTokenCount > 0);
        Assert.True(metadata.InputTokenCount > 0);
        Assert.True(metadata.OutputTokenCount > 0);
    }

    [RetryTheory]
    [InlineData(ServiceType.Anthropic, Skip = "This test is for manual verification.")]
    [InlineData(ServiceType.VertexAI, Skip = "This test is for manual verification.")]
    [InlineData(ServiceType.AmazonBedrock, Skip = "This test is for manual verification.")]
    public async Task ChatStreamingReturnsUsedTokensAsync(ServiceType serviceType)
    {
        // Arrange
        var chatHistory = new ChatHistory();
        chatHistory.AddUserMessage("Hello, I'm Brandon, how are you?");
        chatHistory.AddAssistantMessage("I'm doing well, thanks for asking.");
        chatHistory.AddUserMessage("Call me by my name and expand this abbreviation: LLM");

        var sut = this.GetChatService(serviceType);

        // Act
        var responses = await sut.GetStreamingChatMessageContentsAsync(chatHistory).ToListAsync();

        // Assert
        var metadata = responses.Last().Metadata as AnthropicMetadata;
        Assert.NotNull(metadata);
        this.Output.WriteLine($"TotalTokenCount: {metadata.TotalTokenCount}");
        this.Output.WriteLine($"InputTokenCount: {metadata.InputTokenCount}");
        this.Output.WriteLine($"OutputTokenCount: {metadata.OutputTokenCount}");
        Assert.True(metadata.TotalTokenCount > 0);
        Assert.True(metadata.InputTokenCount > 0);
        Assert.True(metadata.OutputTokenCount > 0);
    }

    [RetryTheory]
    [InlineData(ServiceType.Anthropic, Skip = "This test is for manual verification.")]
    [InlineData(ServiceType.VertexAI, Skip = "This test is for manual verification.")]
    [InlineData(ServiceType.AmazonBedrock, Skip = "This test is for manual verification.")]
    public async Task ChatGenerationReturnsStopFinishReasonAsync(ServiceType serviceType)
    {
        // Arrange
        var chatHistory = new ChatHistory();
        chatHistory.AddUserMessage("Hello, I'm Brandon, how are you?");
        chatHistory.AddAssistantMessage("I'm doing well, thanks for asking.");
        chatHistory.AddUserMessage("Call me by my name and expand this abbreviation: LLM");

        var sut = this.GetChatService(serviceType);

        // Act
        var response = await sut.GetChatMessageContentAsync(chatHistory);

        // Assert
        var metadata = response.Metadata as AnthropicMetadata;
        Assert.NotNull(metadata);
        this.Output.WriteLine($"FinishReason: {metadata.FinishReason}");
        Assert.Equal(AnthropicFinishReason.Stop, metadata.FinishReason);
    }

    [Fact(Skip = "This test is for manual verification.")]
    public async Task ChatGenerationComputerUseAsync()
    {
        // Arrange
        var chatHistory = new ChatHistory();
        chatHistory.AddUserMessage("Open Notepad and type Hi!");

        var sut = new AnthropicChatCompletionService(this.AnthropicGetModel(), this.AnthropicGetApiKey(), new(beta: ComputerUse_2024_10_22.Beta));

        var kernel = new Kernel();
        kernel.Plugins.Add(ComputerUse_2024_10_22.ComputerPlugin(new NoOpComputer()));

        // Act
        var response = await sut.GetChatMessageContentsAsync(chatHistory, kernel: kernel);

        // Assert
        var lastMessage = response[^1];
        var metadata = lastMessage.Metadata as AnthropicMetadata;
        Assert.NotNull(metadata);
        this.Output.WriteLine($"FinishReason: {metadata.FinishReason}");
        Assert.Equal(AnthropicFinishReason.ToolUse, metadata.FinishReason);

        var supposedlyScreenshotCall = lastMessage.Items
            .OfType<FunctionCallContent>()
            .FirstOrDefault();
        Assert.NotNull(supposedlyScreenshotCall);
        Assert.Equal("computer", supposedlyScreenshotCall.FunctionName);
        if (supposedlyScreenshotCall.Arguments?["action"] is not JsonElement action)
        {
            Assert.Fail("action is not supplied or wrong type");
            return;
        }
        Assert.Equal("screenshot", action.Deserialize<string>());
    }

    [RetryFact(Skip = "This test is for manual verification.")]
    public async Task ChatGenerationComputerUseUnderstandsResult()
    {
        // Arrange
        var chatHistory = new ChatHistory
        {
            new(AuthorRole.Assistant, content: null)
            {
                Items = {
                    new FunctionCallContent("computer", id: "1", arguments: new KernelArguments()
                    {
                        ["action"] = "cursor_position",
                    }),
                },
            },
            new(AuthorRole.User, content: null)
            {
                Items =
                {
                    new FunctionResultContent("computer", callId: "1", result: "x=120,y=121"),
                },
            },
        };
        chatHistory.AddUserMessage("Now that you got the coordinates, tell me x+y as a single number, e.g. '1234'");

        var sut = new AnthropicChatCompletionService(this.AnthropicGetModel(), this.AnthropicGetApiKey(), new(beta: ComputerUse_2024_10_22.Beta));

        var kernel = new Kernel();
        kernel.Plugins.Add(ComputerUse_2024_10_22.ComputerPlugin(new NoOpComputer()));

        // Act
        var response = await sut.GetChatMessageContentAsync(chatHistory, kernel: kernel);

        // Assert
        var metadata = response.Metadata as AnthropicMetadata;
        Assert.NotNull(metadata);
        this.Output.WriteLine($"FinishReason: {metadata.FinishReason}");
        Assert.Equal(AnthropicFinishReason.Stop, metadata.FinishReason);
        Assert.True(response.Content?.EndsWith("241", StringComparison.InvariantCulture));
    }

    [Fact]
    public async Task ChatGenerationComputerUseAutoCallAsync()
    {
        // Arrange
        var chatHistory = new ChatHistory();
        chatHistory.AddUserMessage("Get mouse coordinates, and tell me x+y as a single number, e.g. '1234'");

        var sut = new AnthropicChatCompletionService(this.AnthropicGetModel(), this.AnthropicGetApiKey(), new(beta: ComputerUse_2024_10_22.Beta));

        var kernel = new Kernel();
        kernel.Plugins.Add(ComputerUse_2024_10_22.ComputerPlugin(new NoOpComputer()
        {
            CursorPosition = (120, 121),
        }));

        // Act
        var response = await sut.GetChatMessageContentAsync(chatHistory, kernel: kernel,
                                                            executionSettings: new AnthropicPromptExecutionSettings()
                                                            {
                                                                MaxTokens = AnthropicPromptExecutionSettings.DefaultTextMaxTokens,
                                                                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
                                                            });

        // Assert
        var metadata = response.Metadata as AnthropicMetadata;
        Assert.NotNull(metadata);
        this.Output.WriteLine($"FinishReason: {metadata.FinishReason}");
        Assert.Equal(AnthropicFinishReason.Stop, metadata.FinishReason);
        Assert.True(response.Content?.EndsWith("241", StringComparison.InvariantCulture));
    }

    [RetryTheory]
    [InlineData(ServiceType.Anthropic, Skip = "This test is for manual verification.")]
    [InlineData(ServiceType.VertexAI, Skip = "This test is for manual verification.")]
    [InlineData(ServiceType.AmazonBedrock, Skip = "This test is for manual verification.")]
    public async Task ChatStreamingReturnsStopFinishReasonAsync(ServiceType serviceType)
    {
        // Arrange
        var chatHistory = new ChatHistory();
        chatHistory.AddUserMessage("Hello, I'm Brandon, how are you?");
        chatHistory.AddAssistantMessage("I'm doing well, thanks for asking.");
        chatHistory.AddUserMessage("Call me by my name and expand this abbreviation: LLM");

        var sut = this.GetChatService(serviceType);

        // Act
        var responses = await sut.GetStreamingChatMessageContentsAsync(chatHistory).ToListAsync();

        // Assert
        var metadata = responses.Last().Metadata as AnthropicMetadata;
        Assert.NotNull(metadata);
        this.Output.WriteLine($"FinishReason: {metadata.FinishReason}");
        Assert.Equal(AnthropicFinishReason.Stop, metadata.FinishReason);
    }

    [RetryTheory]
    [InlineData(ServiceType.Anthropic, Skip = "This can fail. Anthropic does not support this feature yet.")]
    [InlineData(ServiceType.VertexAI, Skip = "This can fail. Anthropic does not support this feature yet.")]
    [InlineData(ServiceType.AmazonBedrock, Skip = "This can fail. Anthropic does not support this feature yet.")]
    public async Task ChatGenerationOnlyAssistantMessagesAsync(ServiceType serviceType)
    {
        // Arrange
        var chatHistory = new ChatHistory();
        chatHistory.AddAssistantMessage("I'm very thirsty.");
        chatHistory.AddAssistantMessage("Could you give me a glass of...");

        var sut = this.GetChatService(serviceType);

        // Act
        var response = await sut.GetChatMessageContentAsync(chatHistory);

        // Assert
        string[] words = ["water", "juice", "milk", "soda", "tea", "coffee", "beer", "wine"];
        this.Output.WriteLine(response.Content);
        Assert.Contains(words, word => response.Content!.Contains(word, StringComparison.OrdinalIgnoreCase));
    }

    [RetryTheory]
    [InlineData(ServiceType.Anthropic, Skip = "This can fail. Anthropic does not support this feature yet.")]
    [InlineData(ServiceType.VertexAI, Skip = "This can fail. Anthropic does not support this feature yet.")]
    [InlineData(ServiceType.AmazonBedrock, Skip = "This can fail. Anthropic does not support this feature yet.")]
    public async Task ChatStreamingOnlyAssistantMessagesAsync(ServiceType serviceType)
    {
        // Arrange
        var chatHistory = new ChatHistory();
        chatHistory.AddAssistantMessage("I'm very thirsty.");
        chatHistory.AddAssistantMessage("Could you give me a glass of...");

        var sut = this.GetChatService(serviceType);

        // Act
        var responses = await sut.GetStreamingChatMessageContentsAsync(chatHistory).ToListAsync();

        // Assert
        string[] words = ["water", "juice", "milk", "soda", "tea", "coffee", "beer", "wine"];
        Assert.NotEmpty(responses);
        var message = string.Concat(responses.Select(c => c.Content));
        this.Output.WriteLine(message);
        Assert.Contains(words, word => message.Contains(word, StringComparison.OrdinalIgnoreCase));
    }

    [RetryTheory]
    [InlineData(ServiceType.Anthropic, Skip = "This can fail. Anthropic does not support this feature yet.")]
    [InlineData(ServiceType.VertexAI, Skip = "This can fail. Anthropic does not support this feature yet.")]
    [InlineData(ServiceType.AmazonBedrock, Skip = "This can fail. Anthropic does not support this feature yet.")]
    public async Task ChatGenerationOnlyUserMessagesAsync(ServiceType serviceType)
    {
        // Arrange
        var chatHistory = new ChatHistory();
        chatHistory.AddUserMessage("I'm very thirsty.");
        chatHistory.AddUserMessage("Could you give me a glass of...");

        var sut = this.GetChatService(serviceType);

        // Act
        var response = await sut.GetChatMessageContentAsync(chatHistory);

        // Assert
        string[] words = ["water", "juice", "milk", "soda", "tea", "coffee", "beer", "wine"];
        this.Output.WriteLine(response.Content);
        Assert.Contains(words, word => response.Content!.Contains(word, StringComparison.OrdinalIgnoreCase));
    }

    [RetryTheory]
    [InlineData(ServiceType.Anthropic, Skip = "This can fail. Anthropic does not support this feature yet.")]
    [InlineData(ServiceType.VertexAI, Skip = "This can fail. Anthropic does not support this feature yet.")]
    [InlineData(ServiceType.AmazonBedrock, Skip = "This can fail. Anthropic does not support this feature yet.")]
    public async Task ChatStreamingOnlyUserMessagesAsync(ServiceType serviceType)
    {
        // Arrange
        var chatHistory = new ChatHistory();
        chatHistory.AddUserMessage("I'm very thirsty.");
        chatHistory.AddUserMessage("Could you give me a glass of...");

        var sut = this.GetChatService(serviceType);

        // Act
        var responses = await sut.GetStreamingChatMessageContentsAsync(chatHistory).ToListAsync();

        // Assert
        string[] words = ["water", "juice", "milk", "soda", "tea", "coffee", "beer", "wine"];
        Assert.NotEmpty(responses);
        var message = string.Concat(responses.Select(c => c.Content));
        this.Output.WriteLine(message);
        Assert.Contains(words, word => message.Contains(word, StringComparison.OrdinalIgnoreCase));
    }
}
