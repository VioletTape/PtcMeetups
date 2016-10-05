using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SM.MaterialDomain;
using StructureMap;

namespace SM.Cases._2_AutoDiscovery {
    [TestFixture]
    public class AutoDiscovery {
        [Test]
        public void GetAllMaterials() {
            var registry = new Registry();

            // act
            registry.Scan(x => {
                              x.TheCallingAssembly();
                              x.AddAllTypesOf<IMaterial>();
                          });
            var container = new Container(registry);

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

        [Test]
        public void GetInstanceByType() {
            var registry = new Registry();

            // act
            registry.Scan(x => {
                              x.TheCallingAssembly();
                              x.AddAllTypesOf<IMaterial>();
                          });
            var container = new Container(registry);

            // assert
            var gold = container.GetInstance<Gold>();

            gold.Should().NotBeNull();
        }
    }
}