using ApprovalTests.Reporters;
using Xunit;

[assembly: UseReporter(typeof(DiffReporter), typeof(ClipboardReporter))]
[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly, DisableTestParallelization = true)]