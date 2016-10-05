using LightInjectExample.Domain;

namespace LightInjectExample.MaterialDomain {
    public abstract class Naturals : IMaterial {}


    public class Stone : Naturals {}

    public class Wood : Naturals {}
}