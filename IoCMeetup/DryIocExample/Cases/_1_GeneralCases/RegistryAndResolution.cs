using DryIoc;
using DryIocExample.ServiceDomain;
using FluentAssertions;
using NUnit.Framework;

namespace DryIocExample.Cases._1_GeneralCases {
    [TestFixture]
    public class RegistryAndResolution {
        [Test]
        public void NestedResoltionCase() {
            var container = new Container();

            container.Register<ServiceA, ServiceA>();
            container.Register<ServiceB, ServiceB>();
            container.Register<ServiceC, ServiceC>();
            container.Register<ServiceD, ServiceD>();

            // act
            var serviceD = container.Resolve<ServiceD>();

            // assert
            serviceD
                .Should()
                .NotBeNull();
        }
    }
}