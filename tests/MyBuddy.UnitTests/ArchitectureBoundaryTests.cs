using MyBuddy.Domain;

namespace MyBuddy.UnitTests;

public class ArchitectureBoundaryTests
{
    [Fact]
    public void Domain_does_not_reference_infrastructure()
    {
        var references = typeof(AssemblyMarker).Assembly.GetReferencedAssemblies();

        Assert.DoesNotContain(references, assembly => assembly.Name == "MyBuddy.Infrastructure");
    }
}
