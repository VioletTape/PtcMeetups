namespace SM.ServiceDomain {
    public class ServiceA {}

    public class ServiceB {
        public ServiceB(ServiceA a) {}
    }

    public class ServiceC {
        public ServiceC(ServiceB b) {}
    }

    public class ServiceD {
        public ServiceD(ServiceC c) {}
    }

     public class BigService {
        private readonly IFoo foo;
        public BigService(IFoo foo, IBoo boo, IBar bar) {
            this.foo = foo;
        }

        public string GetMessage() {
            return foo.GetMessage();
        }
    }

    public interface IFoo {
        string GetMessage();
    }
    public interface IBoo {}
    public interface IBar {}
}