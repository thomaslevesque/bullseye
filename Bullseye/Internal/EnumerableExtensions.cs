#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace Bullseye.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Sanitize<T>(this IEnumerable<T> items) where T : class =>
            items?.Where(item => item != null) ?? Enumerable.Empty<T>();

        public static Options ToOptions(this IEnumerable<(string, bool)> values)
        {
            var options = new Options();

            var helpOptions = new[] { "--help", "-h", "-?" };

            foreach (var (name, isSet) in values ?? Enumerable.Empty<(string, bool)>())
            {
                switch (name)
                {
                    case "-c":
                    case "--clear":
                        options.Clear = isSet;
                        break;
                    case "-n":
                    case "--dry-run":
                        options.DryRun = isSet;
                        break;
                    case "-d":
                    case "--list-dependencies":
                        options.ListDependencies = isSet;
                        break;
                    case "-i":
                    case "--list-inputs":
                        options.ListInputs = isSet;
                        break;
                    case "-l":
                    case "--list-targets":
                        options.ListTargets = isSet;
                        break;
                    case "-t":
                    case "--list-tree":
                        options.ListTree = isSet;
                        break;
                    case "-N":
                    case "--no-color":
                        options.NoColor = isSet;
                        break;
                    case "-p":
                    case "--parallel":
                        options.Parallel = isSet;
                        break;
                    case "-s":
                    case "--skip-dependencies":
                        options.SkipDependencies = isSet;
                        break;
                    case "-v":
                    case "--verbose":
                        options.Verbose = isSet;
                        break;
                    case "--appveyor":
                        if (isSet)
                        {
                            options.Host = Host.Appveyor;
                        }

                        break;
                    case "--azure-pipelines":
                        if (isSet)
                        {
                            options.Host = Host.AzurePipelines;
                        }

                        break;
                    case "--github-actions":
                        if (isSet)
                        {
                            options.Host = Host.GitHubActions;
                        }

                        break;
                    case "--gitlab-ci":
                        if (isSet)
                        {
                            options.Host = Host.GitLabCI;
                        }

                        break;
                    case "--travis":
                        if (isSet)
                        {
                            options.Host = Host.Travis;
                        }

                        break;
                    case "--teamcity":
                        if (isSet)
                        {
                            options.Host = Host.TeamCity;
                        }

                        break;
                    default:
                        if (helpOptions.Contains(name, StringComparer.OrdinalIgnoreCase))
                        {
                            options.ShowHelp = isSet;
                        }
                        else
                        {
                            options.UnknownOptions.Add(name);
                        }

                        break;
                }
            }

            return options;
        }
    }
}
