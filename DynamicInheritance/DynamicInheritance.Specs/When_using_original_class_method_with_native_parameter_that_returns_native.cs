using System;
using Machine.Specifications;

namespace DynamicInheritance.Specs
{
    [Subject(typeof(TypeExtensions))]
    public class When_using_original_class_method_with_native_parameter_that_returns_native : WithSomeObject
    {
        Because of = () => Result = (String)Subject.AddHashTag("yolo");

        It should_return_correct_value = () => Result.ShouldEqual("#yolo");
        
    }
}