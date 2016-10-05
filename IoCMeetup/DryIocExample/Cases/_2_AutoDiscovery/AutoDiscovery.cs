using System;
using System.Linq;
using DryIoc;
using DryIocExample.MaterialDomain;
using FluentAssertions;
using NUnit.Framework;

namespace DryIocExample.Cases._2_AutoDiscovery {
    [TestFixture]
    public class AutoDiscovery {
        [Test]
        public void MaterialTests() {
            var container = new Container();

            // act
            container.RegisterMany(new[] {
                                       typeof(IMaterial).Assembly
                                   }, type => type == typeof(IMaterial));

            // assert
            var materials = container.ResolveMany<IMaterial>().ToList();

            materials
                    .Cast<object>()
                     .ToList()
                     .ForEach(i => Console.WriteLine(i.GetType().FullName));

            materials.Count
                    .Should()
                    .Be(7);
        }


        [Test]
        public void GetInstanceByType_WithManualSearch() {
            var container = new Container();

            // act
            container.RegisterMany(new[] {
                                       typeof(IMaterial).Assembly
                                   }, type => type == typeof(IMaterial));

            // assert
            var gold = container.ResolveMany<IMaterial>().SingleOrDefault(i => i.GetType() == typeof(Gold));

            gold.Should().NotBeNull();
        }

        /// <summary>
        /// Intented to fail. I want to show that it's not obvious
        /// how to setup Discovery feature
        /// </summary>
        [Test]
        public void GetInstanceByType_Directly() {
            var container = new Container();

            // act
            container.RegisterMany(new[] {
                                       typeof(IMaterial).Assembly
                                   },
                                   type => type == typeof(IMaterial));

            // assert
            container.IsRegistered<Gold>()
                      .Should().BeTrue();
        }

        [Test]
        public void GetInstanceByType_WithManualRegisterInstructions() {
            var container = new Container();

            // act
            container.RegisterMany(new[] {
                                       typeof(IMaterial).Assembly
                                   },
                                   (registrator, serviceTypes, implType) => {
                                       if (implType.IsAssignableTo(typeof(IMaterial)))
                                           registrator.Register(implType);
                                   });

            // assert
            container.Resolve<Gold>().Should().NotBeNull();
        }
    }
}