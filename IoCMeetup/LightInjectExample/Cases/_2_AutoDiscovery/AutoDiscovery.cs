using System;
using System.Linq;
using FluentAssertions;
using LightInject;
using LightInjectExample.MaterialDomain;
using NUnit.Framework;

namespace LightInjectExample.Cases._2_AutoDiscovery {
    [TestFixture]
    public class AutoDiscovery {
        /// <summary>
        ///     Intented to fail. I want to show that it's not obvious
        ///     how to setup Discovery feature
        /// </summary>
        [Test]
        [Ignore("Comment attribute to see that it not works")]
        public void GetAllMaterials_Naive() {
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
        public void GetAllMaterials_WithHints() {
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

        [Test]
        public void GetInstanceByType_BaseAndName() {
            var container = new ServiceContainer();

            // act 
            container.RegisterAssembly(typeof(IMaterial).Assembly, (tA, tB) => tA == typeof(IMaterial));

            // assert 
            var gold = container.GetInstance<IMaterial>(typeof(Gold).Name);

            gold.Should()
                .NotBeNull();
        }


        /// <summary>
        ///     Intented to fail. I want to show that it's not obvious
        ///     how to setup Discovery feature
        /// </summary>
        [Test]
        [Ignore("Comment attribute to see that it not works")]
        public void GetInstanceByType_Naive() {
            var container = new ServiceContainer();

            // act 
            container.RegisterAssembly(typeof(IMaterial).Assembly, (tA, tB) => tA == typeof(IMaterial));

            // assert 
            var gold = container.GetInstance<Gold>();

            gold.Should()
                .NotBeNull();
        }
    }
}