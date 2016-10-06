using System;
using System.Linq;
using NSubstitute;

namespace StructureMap.AutoMocking {
    public class NSubstituteAutoMocker<T> : AutoMocker<T>
        where T : class {
        private NSubstituteAutoMocker(ServiceLocator serviceLocator = null) : base(null) {
            ServiceLocator = new NSubstituteServiceLocator();
            Container = new AutoMockedContainer(ServiceLocator);
        }

        public NSubstituteAutoMocker() : this(null) {
            ServiceLocator = new NSubstituteServiceLocator();
            Container = new AutoMockedContainer(ServiceLocator);
        }
    }

    public class NSubstituteServiceLocator : ServiceLocator {
        private readonly SubstituteFactory substituteFactory = new SubstituteFactory();

        public T Service<T>() where T : class {
            return (T) substituteFactory.CreateMock(typeof(T));
        }

        public object Service(Type serviceType) {
            return substituteFactory.CreateMock(serviceType);
        }

        public T PartialMock<T>(params object[] args) where T : class {
            return (T) substituteFactory.CreateMock(typeof(T));
        }
    }

    public class SubstituteFactory {
        private readonly Func<Type, object> factory;

        public SubstituteFactory() {
            var type = typeof(Substitute);

            var method =
                type.GetMethods().First(x => x.ContainsGenericParameters && (x.GetGenericArguments().Length == 1));
            factory = typeToMock => method.MakeGenericMethod(typeToMock).Invoke(null, new object[] {null});
        }

        public object CreateMock(Type type) {
            return factory(type);
        }
    }
}