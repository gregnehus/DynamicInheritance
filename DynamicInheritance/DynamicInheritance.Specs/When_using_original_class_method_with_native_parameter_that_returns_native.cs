using System;
using Machine.Specifications;

namespace DynamicInheritance.Specs
{
    [Subject(typeof(ObjectExtensions))]
    public class When_using_original_class_method_with_native_parameter_that_returns_native : WithSomeObject
    {
        Because of = () => _result = (String)_subject.AddHashTag("yolo");

        It should_return_correct_value = () => _result.ShouldEqual("#yolo");
        
    }
}