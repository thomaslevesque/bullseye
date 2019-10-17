#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace Bullseye.Internal
{
    using System.Collections.Generic;
    using System.Linq;

    public static class StringExtensions
    {
        public static string Spaced(this IEnumerable<string> strings) => string.Join(" ", strings);

        public static (List<string>, Options) Parse(this IEnumerable<string> args) =>
            (args.Where(arg => !arg.StartsWith("-")).ToList(), args.Where(arg => arg.StartsWith("-")).Select(arg => (arg, true)).ToOptions());

        // pad right printed
        public static string Prp(this string text, int totalWidth, char paddingChar) =>
            text.PadRight(totalWidth + (text.Length - Palette.StripColours(text).Length), paddingChar);
    }
}
