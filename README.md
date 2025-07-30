# Gait

Gait is a .NET application for automating and managing Git operations from the command line, with enhanced output and error handling. It is designed to simplify common Git workflows and provide clear feedback for each operation.


## Workflow Steps

1. **Add All**: Stages all changes in the project directory (`git add *`).
2. **Diff**: Shows the current staged diff (`git diff --staged`).
3. **AI Summary**: The application uses AI to automatically summarise the diff and generate a commit message.
4. **Commit**: Commits the staged changes using the AI-generated message.
5. **Push**: Pushes commits to the remote repository (`git push`).


## Configuration

Make sure to set your OpenAI API key in your environment before running the application:

```pwsh
$env:KEY_OPENAI = "your-api-key-here"
```

Settings are managed via `appsettings.json` and `appsettings.Production.json`.

Below is an example of a minimal `appsettings.json`:

```json
{
  "OpenAI": {
    "Model": "gpt-4o-mini",
    "ApiKeyVar": "KEY_OPENAI",
    "PromptPath": "diff-prompt.txt"
  },
  "Git": {
    "MaxRecursiveDirectories": 10
  }
}
```

### Descriptions

| Section | Setting                 | Description                                                              | Example Value     |
| ------- | ----------------------- | ------------------------------------------------------------------------ | ----------------- |
| OpenAI  | Model                   | The OpenAI model used for summarisation                                  | `gpt-4o-mini`     |
| OpenAI  | ApiKeyVar               | The name of the environment variable that stores your OpenAI API key     | `KEY_OPENAI`      |
| OpenAI  | PromptPath              | The path to the prompt file used for diff summarisation                  | `diff-prompt.txt` |
| Git     | MaxRecursiveDirectories | The maximum number of parent directories to traverse for a `.git` folder | `10`              |

## Requirements

- .NET 10.0 or later
- Git installed and available in PATH

## License

MIT License

## Author

Lee Laffan
