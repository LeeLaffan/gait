# Gait

Gait is a .NET application for automating and managing Git operations from the command line, with enhanced output and error handling. It is designed to simplify common Git workflows and provide clear feedback for each operation.


## Workflow Steps

1. **Add All**: Stages all changes in the project directory (`git add *`).
2. **Diff**: Shows the current staged diff (`git diff --staged`).
3. **AI Summary**: The application uses AI to automatically summarise the diff and generate a commit message.
4. **Commit**: Commits the staged changes using the AI-generated message.
5. **Push**: Pushes commits to the remote repository (`git push`).


## Configuration

Settings are managed via `appsettings.json` and `appsettings.Production.json`.

### OpenAI Settings

The application uses OpenAI to automatically summarise diffs and generate commit messages. The relevant settings in `appsettings.json` are:

- `Model`: The OpenAI model used for summarisation (e.g., `gpt-4o-mini`).
- `ApiKeyVar`: The name of the environment variable that stores your OpenAI API key (e.g., `KEY_OPENAI`).
- `PromptPath`: The path to the prompt file used for diff summarisation (e.g., `diff-prompt.txt`).

Make sure to set your OpenAI API key in your environment before running the application:

```pwsh
$env:KEY_OPENAI = "your-api-key-here"
```

## Requirements

- .NET 10.0 or later
- Git installed and available in PATH

## License

MIT License

## Author

Lee Laffan
