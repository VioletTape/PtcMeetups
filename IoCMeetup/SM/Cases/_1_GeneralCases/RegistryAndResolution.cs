using FluentAssertions;
using NUnit.Framework;
using SM.ServiceDomain;
using StructureMap;

namespace SM.Cases._1_GeneralCases {
    [TestFixture]
    public class RegistryAndResolution {
        [Test]
        public void NestedResultionCase() {
            var registry = new Registry();
            registry.For<ServiceA>().Use<ServiceA>();
            registry.For<ServiceB>().Use<ServiceB>();
            registry.For<ServiceC>().Use<ServiceC>();
            registry.For<ServiceD>().Use<ServiceD>();

            var container = new Container(registry);

            // act
            var serviceD = container.GetInstance<ServiceD>();

            // assert
            serviceD
                .Should()
                .NotBeNull();
        }
    }
}