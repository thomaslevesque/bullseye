namespace Bullseye.Internal
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class Target
    {
        public Target(TargetName name, List<TargetName> dependencies)
        {
            this.Name = name ?? throw new BullseyeException("Target name cannot be null.");
            this.Dependencies = dependencies;
        }

        public TargetName Name { get; }

        public List<TargetName> Dependencies { get; }

        public virtual Task RunAsync(bool dryRun, bool parallel, Logger log) => log.Succeeded(this.Name, null);
    }
}
