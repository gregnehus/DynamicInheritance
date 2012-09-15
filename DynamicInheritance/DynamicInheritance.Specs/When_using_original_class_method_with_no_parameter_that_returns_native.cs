using System;
using Machine.Specifications;

namespace DynamicInheritance.Specs
{
    [Subject(typeof(ObjectExtensions))]
    public class When_using_original_class_method_with_no_parameter_that_returns_native : WithSomeObject
    {
        Because of = () => _result = (String)_subject.GetSuperName();

        It should_return_correct_value = () => _result.ShouldEqual("SomeObject");
        It should_return_native_type_from_method_with_native_type_parameter = () => ((string)_subject.AddHashTag("yolo")).ShouldEqual("#yolo");
    }
}