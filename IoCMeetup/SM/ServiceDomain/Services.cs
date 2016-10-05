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
}