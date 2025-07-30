namespace Gait.Configuration;

public class GitConfiguration
{
    public const string SectionName = "Git";
    public int MaxRecursiveDirectories { get; set; } = 10;
    public string? CommitSignature { get; set; }
}
