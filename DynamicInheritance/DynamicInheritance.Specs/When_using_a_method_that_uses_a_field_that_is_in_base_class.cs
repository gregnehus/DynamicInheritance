using Machine.Specifications;

namespace DynamicInheritance.Specs
{
    public class When_using_a_method_that_uses_a_field_that_is_in_base_class : WithSomeObject
    {
        //Establish context = () => Subject.name = "peach";
        Because of = () => Result = (string)Subject.GetField();
        It should_use_the_field_in_the_base_class = () => Result.ShouldEqual("peach");
    }
}
