using System;
using Machine.Specifications;
using NSubstitute;

namespace DynamicInheritance.Specs
{
    public class WithSomeObject
    {
        static readonly SomeObject Super = new SomeObject();
        protected static readonly IAmAnInterface Inter = Substitute.For<IAmAnInterface>();
        protected static string _result;
        protected static dynamic _subject;

        Establish context = () =>
                                {
                                    Inter.DoSomething().Returns("interface");
                                    _subject = Activator.CreateInstance(Super.AddBaseType<BaseType>());
                                };
    }
}