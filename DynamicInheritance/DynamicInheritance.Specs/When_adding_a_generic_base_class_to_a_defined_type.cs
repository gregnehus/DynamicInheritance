using System;
using Machine.Specifications;

namespace DynamicInheritance.Specs
{
    [Subject(typeof(ObjectExtensions))]
    public class When_adding_a_base_class_to_a_generic_type
    {
        static GenericSomeObject<string> _super = new GenericSomeObject<string>();
        static Type _result;

        Because of = () => _result = _super.AddBaseType<BaseType>();

        It should_return_a_non_null_type = () => _result.ShouldNotBeNull();
        It should_return_a_type_that_inherits_from_specified_base_class = () => _result.BaseType.ShouldEqual(typeof(BaseType));
        It should_return_a_type_that_can_be_instantiated = () => Activator.CreateInstance(_result).ShouldNotBeNull();

    }
}