using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using SM.ServiceDomain;
using StructureMap;
using StructureMap.AutoMocking;

namespace SM.Cases._3_AutoMocking {
    [TestFixture]
    public class Mocking {
        [Test]
        public void CanSetMocks_Automocking() {
            // arrange
            var mock = new NSubstituteAutoMocker<BigService>();
            mock.Get<IFoo>().GetMessage().Returns("Hello from mock!");

            // act & assert
            mock.ClassUnderTest.GetMessage()
                 .Should()
                 .Be("Hello from mock!");
        }

         [Test]
        public void CanSetMocks_Manual() {
            var mockFoo = Substitute.For<IFoo>();
            var mockBoo = Substitute.For<IBoo>();
            var mockBar = Substitute.For<IBar>();

            mockFoo.GetMessage().Returns("Hello from mock!");

            var container = new Container(_ => {
                                              _.For<IFoo>().Use(mockFoo);
                                              _.For<IBoo>().Use(mockBoo);
                                              _.For<IBar>().Use(mockBar);
                                              _.For<BigService>().Use<BigService>();
                                          });

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