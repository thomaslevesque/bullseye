#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace Bullseye.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public static class TargetCollectionExtensions
    {
        public static Task RunAsync(this TargetCollection targets, IEnumerable<string> args, Func<Exception, bool> messageOnly, string logPrefix)
        {
            var argList = args.Sanitize().ToList();
            var (names, options) = argList.Parse();
            return RunAsync(targets ?? new TargetCollection(), names, options, argList, messageOnly ?? (_ => false), logPrefix);
        }

        public static Task RunAndExitAsync(this TargetCollection targets, IEnumerable<string> args, Func<Exception, bool> messageOnly, string logPrefix)
        {
            var argList = args.Sanitize().ToList();
            var (names, options) = argList.Parse();
            return RunAndExitAsync(targets ?? new TargetCollection(), names, options, argList, messageOnly ?? (_ => false), logPrefix);
        }

        public static Task RunAsync(this TargetCollection targets, IEnumerable<string> names, IEnumerable<(string, bool)> options, Func<Exception, bool> messageOnly, string logPrefix) =>
            RunAsync(targets ?? new TargetCollection(), names.Sanitize().ToList(), options.ToOptions(), null, messageOnly ?? (_ => false), logPrefix);

        public static Task RunAndExitAsync(this TargetCollection targets, IEnumerable<string> names, IEnumerable<(string, bool)> options, Func<Exception, bool> messageOnly, string logPrefix) =>
            RunAndExitAsync(targets ?? new TargetCollection(), names.Sanitize().ToList(), options.ToOptions(), null, messageOnly ?? (_ => false), logPrefix);

        private static async Task RunAsync(this TargetCollection targets, List<string> names, Options options, List<string> args, Func<Exception, bool> messageOnly, string logPrefix)
        {
            var (output, log) = await ConsoleExtensions.Initialize(options, logPrefix).Tax();

            await RunAsync(targets, names, options, output, log, messageOnly, args).Tax();
        }

        private static async Task RunAndExitAsync(this TargetCollection targets, List<string> names, Options options, List<string> args, Func<Exception, bool> messageOnly, string logPrefix)
        {
            var (output, log) = await ConsoleExtensions.Initialize(options, logPrefix).Tax();

            try
            {
                await RunAsync(targets, names, options, output, log, messageOnly, args).Tax();
            }
            catch (InvalidUsageException ex)
            {
                await log.Error(ex.Message).Tax();
                Environment.Exit(2);
            }
            catch (TargetFailedException)
            {
                Environment.Exit(1);
            }

            Environment.Exit(0);
        }

        private static async Task RunAsync(this TargetCollection targets, List<string> names, Options options, Output output, Logger log, Func<Exception, bool> messageOnly, List<string> args)
        {
            if (options.UnknownOptions.Count > 0)
            {
                throw new InvalidUsageException($"Unknown option{(options.UnknownOptions.Count > 1 ? "s" : "")} {options.UnknownOptions.Spaced()}. \"--help\" for usage.");
            }

            if (args != null)
            {
                await log.Verbose($"Args: {string.Join(" ", args)}").Tax();
            }

            if (options.ShowHelp)
            {
                await output.Usage(targets).Tax();
                return;
            }

            if (options.ListTree || options.ListDependencies || options.ListInputs || options.ListTargets)
            {
                var rootTargets = names.Any() ? names : targets.Select(target => target.Name).OrderBy(name => name).ToList();
                var maxDepth = options.ListTree ? int.MaxValue : options.ListDependencies ? 1 : 0;
                var maxDepthToShowInputs = options.ListTree ? int.MaxValue : 0;
                await output.Targets(targets, rootTargets, maxDepth, maxDepthToShowInputs, options.ListInputs).Tax();
                return;
            }

            if (names.Count == 0)
            {
                names.Add("default");
            }

            await targets.RunAsync(names, options.SkipDependencies, options.DryRun, options.Parallel, log, messageOnly).Tax();
        }
    }
}
