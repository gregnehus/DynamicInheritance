using System;
using Machine.Specifications;

namespace DynamicInheritance.Specs
{
    [Subject(typeof(TypeExtensions))]
    public class When_using_original_class_method_with_no_parameter_that_returns_native : WithSomeObject
    {
        Because of = () => Result = (String)Subject.GetSuperName();

        It should_return_correct_value = () => Result.ShouldEqual("SomeObject");
        It should_return_native_type_from_method_with_native_type_parameter = () => ((string)Subject.AddHashTag("yolo")).ShouldEqual("#yolo");
    }
}