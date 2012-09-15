using System;
using Machine.Specifications;
using NSubstitute;

namespace DynamicInheritance.Specs
{
    [Subject(typeof(ObjectExtensions))]
    public class When_using_original_class_method_with_object_parameter_that_returns_native : WithSomeObject
    {
        Because of = () => _result = (String)_subject.UseInterface(Inter);

        It should_use_object_as_expected = () => Inter.Received().DoSomething();
        It should_return_correct_value = () => _result.ShouldEqual("interface");

    }
}