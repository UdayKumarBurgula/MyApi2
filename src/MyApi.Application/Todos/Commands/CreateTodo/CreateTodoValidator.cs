using FluentValidation;

namespace MyApi.Application.Todos.Commands.CreateTodo;

public class CreateTodoValidator : AbstractValidator<CreateTodoCommand>
{
    public CreateTodoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        // optional: normalize whitespace checks, etc.
    }
}
