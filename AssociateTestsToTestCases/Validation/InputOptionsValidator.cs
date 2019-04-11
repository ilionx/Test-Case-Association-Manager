using FluentValidation;

namespace AssociateTestsToTestCases.Validation
{
    public class InputOptionsValidator : AbstractValidator<InputOptions>
    {
        public InputOptionsValidator()
        {
            RuleFor(inputOption => inputOption.Directory).NotEmpty().WithMessage("Required argument 'Directory' has not been set.");
            RuleFor(inputOption => inputOption.ProjectName).NotEmpty().WithMessage("Required argument 'ProjectName' has not been set.");
            RuleFor(inputOption => inputOption.TestPlanName).NotEmpty().WithMessage("Required argument 'TestPlanName' has not been set.");
            RuleFor(inputOption => inputOption.CollectionUri).NotEmpty().WithMessage("Required argument 'CollectionUri' has not been set.");
            RuleFor(inputOption => inputOption.PersonalAccessToken).NotEmpty().WithMessage("Required argument 'PersonalAccessToken' has not been set.");

            RuleFor(inputOption => inputOption.MinimatchPatterns).NotNull().WithMessage("Required argument 'MinimatchPatterns' has not been set");
            RuleFor(inputOption => inputOption.MinimatchPatterns).Must(abc => abc[0] != string.Empty).When(abc => abc.MinimatchPatterns != null).WithMessage("Required argument 'MinimatchPatterns' has not been set");
        }
    }
}
