using FluentAssertions;
using LightInject;
using LightInject.AutoMoq;
using LightInjectExample.ServiceDomain;
using Moq;
using NUnit.Framework;

namespace LightInjectExample.Cases._3_AutoMocking {
    [TestFixture]
    public class Mocking {
        [Test]
        public void CanSetMocks_Automocking() {
            // arrange
            var container = new MockingServiceContainer();
            container.Register<BigService, BigService>();

            container.GetMock<IFoo>()
                .Setup(_ => _.GetMessage())
                .Returns("Hello from mock!");

            // act
            var service = container.GetInstance<BigService>();

            // assert
            service.Should()
                .NotBeNull();

            service.GetMessage()
                .Should()
                .Be("Hello from mock!");
        }

        [Test]
        public void CanSetMocks_Manual() {
            var mockFoo = new Mock<IFoo>();
            var mockBoo = new Mock<IBoo>();
            var mockBar = new Mock<IBar>();

            mockFoo.Setup(_ => _.GetMessage())
                .Returns("Hello from mock!");

            var container = new ServiceContainer();
            container.RegisterInstance(typeof(IFoo), mockFoo.Object);
            container.RegisterInstance(typeof(IBoo), mockBoo.Object);
            container.RegisterInstance(typeof(IBar), mockBar.Object);
            container.Register<BigService>();

            // act
            var service = container.GetInstance<BigService>();

            // assert
            service.Should()
                .NotBeNull();

            service.GetMessage()
                .Should()
                .Be("Hello from mock!");
        }
    }
}