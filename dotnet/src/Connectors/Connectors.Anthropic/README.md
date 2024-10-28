# Computer Tool Use

## Example

```csharp
var chatHistory = new ChatHistory();
chatHistory.AddUserMessage("Get mouse coordinates, and tell me x+y as a single number, e.g. '1234'");

var sut = new AnthropicChatCompletionService("claude-3-5-sonnet-20241022", API_KEY, new(beta: ComputerUse_2024_10_22.Beta));

var kernel = new Kernel();
kernel.Plugins.Add(ComputerUse_2024_10_22.ComputerPlugin(new NoOpComputer() // plug your own
{
    CursorPosition = (120, 121),
}));

var response = await sut.GetChatMessageContentAsync(chatHistory, kernel: kernel,
                                                    executionSettings: new AnthropicPromptExecutionSettings()
                                                    {
                                                        MaxTokens = AnthropicPromptExecutionSettings.DefaultTextMaxTokens,
                                                        // invoke functions automatically
                                                        FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
                                                    });


Debug.Assert(response.Content?.EndsWith("241", StringComparison.InvariantCulture));
```

# Semantic Kernel Overview

- Learn more at the [documentation site](https://aka.ms/SK-Docs).
- Join the [Discord community](https://aka.ms/SKDiscord).
- Follow the team on [Semantic Kernel blog](https://aka.ms/sk/blog).
- Check out the [GitHub repository](https://github.com/microsoft/semantic-kernel) for the latest updates.
