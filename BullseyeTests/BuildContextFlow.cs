namespace BullseyeTests
{
    using System.Collections.Generic;
    using Bullseye;
    using Bullseye.Internal;
    using Xbehave;
    using Xunit;
    using static BullseyeTests.Infra.Helper;

    public class BuildContextFlow
    {
        public class TestContext : IBuildContext
        {
            public Dictionary<string, object> Items { get; } = new Dictionary<string, object>();
        }

        [Scenario]
        public void FlowGlobal(TargetCollection targets)
        {
            var testContext = new TestContext();

            "Given a target that sets IBuildContext.Items['Foo'] = 42"
                .x(() => Ensure(ref targets).Add(
                    CreateTarget("first", ctx => ctx.Items.Add("foo", 42), testContext)));

            "And another target which depends on the first target"
                .x(() => targets.Add(
                    CreateTarget(
                        "second",
                        "first".Split(),
                        ctx =>
                        {
                            Assert.True(ctx.Items.TryGetValue("foo", out var num));
                            Assert.Equal(42, (int)num);
                        },
                        testContext)));

            "When I run the second target, it shuld see IBuildContext.Items['Foo'] = 42"
                .x(() => targets.RunAsync("second".Split(), default, default));
        }

        [Scenario]
        public void FlowImplicitSegregated(TargetCollection targets)
        {
            "Given a target Foo1 that sets IBuildContext.Items['Foo'] = 42"
                .x(() => Ensure(ref targets).Add(
                    CreateTarget("Foo1", ctx => ctx.Items.Add("foo", 42), default)));

            "And another target Foo2 which depends on Foo1"
                .x(() => targets.Add(
                    CreateTarget(
                        "Foo2",
                        "Foo1".Split(),
                        ctx =>
                        {
                            Assert.True(ctx.Items.TryGetValue("foo", out var num));
                            Assert.Equal(42, (int)num);
                        },
                        new TestContext())));

            "When I run the second target, it shuld see IBuildContext.Items['Foo'] = 42"
                .x(() => targets.RunAsync("Foo2".Split(), default, default));
        }
    }
}
