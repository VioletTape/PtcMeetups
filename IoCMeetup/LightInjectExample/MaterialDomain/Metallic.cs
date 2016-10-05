using LightInjectExample.Domain;

namespace LightInjectExample.MaterialDomain {
    public abstract class Metallic : IMaterial {}


    public class Gold : Metallic {}

    public class Silver : Metallic {}

    public class Steel : Metallic {}

    public class Iron : Metallic {}

    public class Bronze : Metallic {}
}