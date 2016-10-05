using FluentAssertions;
using LightInject;
using LightInjectExample.ServiceDomain;
using NUnit.Framework;

namespace LightInjectExample.Cases._1_GeneralCases {
    [TestFixture]
    public class RegisterAndReuse {
        [Test]
        public void RegisterAndResolve() {
            var container = new ServiceContainer();

            container.Register<ServiceA, ServiceA>();
            container.Register<ServiceB, ServiceB>();
            container.Register<ServiceC, ServiceC>();
            container.Register<ServiceD, ServiceD>();

            // act
            var serviceD = container.GetInstance<ServiceD>();

            // assert
            serviceD
                .Should()
                .NotBeNull();
        }
    }
}