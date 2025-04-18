using Microsoft.Extensions.Options;

namespace Bardez.EmptyFileFinder.Configuration
{
    public class CheckerOptions : IOptions<CheckerOptions>
    {
        public static readonly string ConfigurationSection = nameof(CheckerOptions);

        public string Path { get; set; } = string.Empty;

        public CheckerOptions Value => this;
    }
}
