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
            container.RegisterMany(new[] {typeof(IMaterial).Assembly}, serviceTypeCondition: type => type == typeof(IMaterial));

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
    }
}