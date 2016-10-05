using System;
using System.Linq;
using FluentAssertions;
using LightInject;
using LightInjectExample.MaterialDomain;
using NUnit.Framework;

namespace LightInjectExample.Cases._2_AutoDiscovery {
    [TestFixture]
    public class AutoDiscovery {
        [Test]
        public void MaterialTests() {
            var container = new ServiceContainer();

            // act 
            container.RegisterAssembly(typeof(IMaterial).Assembly);

            // assert 
            var materials = container.GetAllInstances<IMaterial>().ToList();

            materials
                .Cast<object>()
                .ToList()
                .ForEach(i => Console.WriteLine(i.GetType().FullName));

            materials.Count
                .Should()
                .Be(21);
        }

        [Test]
        public void MaterialTests2() {
            var container = new ServiceContainer();

            // act 
            container.RegisterAssembly(typeof(IMaterial).Assembly, (tA, tB) => tA == typeof(IMaterial));

            // assert 
            var materials = container.GetAllInstances<IMaterial>().ToList();

            materials
                .Cast<object>()
                .ToList()
                .ForEach(i => Console.WriteLine(i.GetType().FullName));

            materials.Count
                .Should()
                .Be(7);
        }
    }
}