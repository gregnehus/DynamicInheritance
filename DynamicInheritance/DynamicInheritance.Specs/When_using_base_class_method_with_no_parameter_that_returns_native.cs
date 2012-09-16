using Machine.Specifications;

namespace DynamicInheritance.Specs
{
    [Subject(typeof(TypeExtensions))]
    public class When_using_base_class_method_with_no_parameter_that_returns_native : WithSomeObject
    {


        Because of = () => Result = Subject.GetBaseName();

        It should_return_string_from_method_with_no_parameter = () => Result.ShouldEqual("BaseType");



    }
}