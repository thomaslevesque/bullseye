namespace BullseyeSmokeTester.SystemCommandLine
{
    using System.CommandLine;
    using System.CommandLine.Invocation;
    using System.Linq;
    using static Bullseye.Targets;

    internal static class Program
    {
        private static int Main(string[] args)
        {
            var cmd = new RootCommand()
            {
                new Option(new[] { "--foo", "-f" }, "A foo.") { Argument = new Argument<string>("foo") }
            };

            // translate from Bullseye to System.CommandLine.Experimental
            cmd.Add(new Argument("targets") { Arity = ArgumentArity.ZeroOrMore, Description = "The targets to run or list." });
            foreach (var option in Options)
            {
                cmd.Add(new Option(option.LongName, option.Description));
            }

            cmd.Handler = CommandHandler.Create<string>(foo =>
            {
                // translate from System.CommandLine.Experimental to Bullseye
                var cmdLine = cmd.Parse(args);
                var targets = cmdLine.CommandResult.Tokens.Select(token => token.Value);
                var options = Options.Select(option => (option.LongName, cmdLine.ValueForOption<bool>(option.LongName)));

                Target("build", () => System.Console.WriteLine($"foo = {foo}"));

                Target("default", DependsOn("build"));

                RunTargetsAndExit(targets, options);
            });

            return cmd.Invoke(args);
        }
    }
}
