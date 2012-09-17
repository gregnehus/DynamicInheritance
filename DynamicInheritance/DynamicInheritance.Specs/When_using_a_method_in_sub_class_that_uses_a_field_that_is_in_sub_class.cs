using Machine.Specifications;

namespace DynamicInheritance.Specs
{
    public class When_using_a_method_in_sub_class_that_uses_a_field_that_is_in_sub_class : WithSomeObject
    {
        Because of = () => Result = (string)Subject.GetSubField();
        It should_use_the_field_in_the_base_class = () => Result.ShouldEqual("cherry");
    }
}