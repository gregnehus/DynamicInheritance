using Machine.Specifications;
using NSubstitute;

namespace DynamicInheritance.Specs
{
    public class WithSomeObject
    {
        protected static readonly IAmAnInterface Inter = Substitute.For<IAmAnInterface>();
        protected static string Result;
        protected static dynamic Subject;

        Establish context = () =>
                                {
                                    Inter.DoSomething().Returns("interface");
                                    Subject = typeof(SomeObject).GetInstanceWithBaseType<BaseType>();
                                };
    }
}