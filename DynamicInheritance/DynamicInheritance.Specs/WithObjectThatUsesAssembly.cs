using Machine.Specifications;

namespace DynamicInheritance.Specs
{
    public class WithObjectThatUsesAssembly
    {
        protected static string Result;
        protected static dynamic Subject;

        Establish context = () =>
            {
                Subject = typeof(AnotherClass).GetInstanceWithBaseType<BaseClassWithFieldFromOtherAssembly>();
            };
    }
}