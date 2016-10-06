using System;
using DryIoc;
using DryIocExample.ServiceDomain;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Arg = DryIoc.Arg;


namespace DryIocExample.Cases._3_AutoMocking {
    [TestFixture]
    public class Mocking {
        [Test]
        public void CanSetMocks_Automocking() {
            var container = new Container(rules => rules.WithUnknownServiceResolvers(request => {
                var serviceType = request.ServiceType;
                if (!serviceType.IsAbstract)
                    return null; // Mock interface or abstract class only.

                return new ReflectionFactory(made: Made.Of(
                    () => Substitute.For(Arg.Index<Type[]>(0), Arg.Index<object[]>(1)),
                    _ => new[] {serviceType}, _ => (object[]) null));
            }));

            container.Resolve<IFoo>()
                .GetMessage().Returns("Hello from mock!");

            var message = container.Resolve<IFoo>().GetMessage();

            container.Register<BigService, BigService>();

            // act
            var service = container.Resolve<BigService>();

            // assert
            service.Should()
                .NotBeNull();

//            service.GetMessage()
//                    .Should()
//                    .Be("Hello from mock!");
        }
    }
}