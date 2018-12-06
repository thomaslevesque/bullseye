namespace Bullseye.Internal
{
    public class TargetName
    {
        private readonly string name;

        private TargetName(string name) => this.name = name;

#pragma warning disable CA2225 // Operator overloads have named alternates
        public static explicit operator TargetName(string name) =>
#pragma warning restore CA2225 // Operator overloads have named alternates
            new TargetName(name ?? throw new BullseyeException("Target name cannot be null."));

#pragma warning disable CA2225 // Operator overloads have named alternates
        public static implicit operator string(TargetName targetName) => targetName.name;
#pragma warning restore CA2225 // Operator overloads have named alternates
    }
}
