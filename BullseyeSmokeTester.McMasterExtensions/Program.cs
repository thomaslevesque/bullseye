namespace BullseyeSmokeTester.McMasterExtensions
{
    using System.Linq;
    using McMaster.Extensions.CommandLineUtils;
    using static Bullseye.Targets;

    internal static class Program
    {
        private static void Main(string[] args)
        {
            var app = new CommandLineApplication();
            app.HelpOption();
            var foo = app.Option<string>("--foo", "A value used for something.", CommandOptionType.SingleValue);

            // translate from Bullseye to McMaster.Extensions.CommandLineUtils
            var targets = app.Argument("targets", "The targets to run or list.", true);
            foreach (var option in Options)
            {
                app.Option(option.LongName, option.Description, CommandOptionType.NoValue);
            }

            app.OnExecute(() =>
            {
                // translate from McMaster.Extensions.CommandLineUtils to Bullseye
                var targets = app.Arguments[0].Values;
                var options = Options.Select(option => (option.LongName, app.Options.Single(o => "--" + o.LongName == option.LongName).HasValue()));

                Target("build", () => System.Console.WriteLine($"foo = {foo.Value()}"));

                Target("default", DependsOn("build"));

                RunTargetsAndExit(targets, options);
            });

            app.Execute(args);
        }
    }
}
