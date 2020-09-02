using FluentValidation;

namespace AssociateTestsToTestCases.Validation
{
    public class InputOptionsValidator : AbstractValidator<InputOptions>
    {
        public InputOptionsValidator()
        {
            RuleFor(inputOption => inputOption.Directory).NotEmpty().WithMessage("Required argument 'Directory' has not been set.");
            RuleFor(inputOption => inputOption.ProjectName).NotEmpty().WithMessage("Required argument 'ProjectName' has not been set.");
            RuleFor(inputOption => inputOption.TestPlanId).NotEmpty().WithMessage("Required argument 'TestPlanId' has not been set.");
            RuleFor(inputOption => inputOption.TestSuiteId).NotEmpty().WithMessage("Required argument 'TestSuiteId' has not been set.");
            RuleFor(inputOption => inputOption.CollectionUri).NotEmpty().WithMessage("Required argument 'CollectionUri' has not been set.");
            RuleFor(inputOption => inputOption.PersonalAccessToken).NotEmpty().WithMessage("Required argument 'PersonalAccessToken' has not been set.");
            RuleFor(inputOption => inputOption.TestFrameworkType).NotEmpty().WithMessage("Required argument 'TestFrameworktype' has not been set.");

            RuleFor(inputOption => inputOption.MinimatchPatterns).NotNull().WithMessage("Required argument 'MinimatchPatterns' has not been set");
            RuleFor(inputOption => inputOption.MinimatchPatterns).Must(mm => mm[0] != string.Empty).When(io => io.MinimatchPatterns != null).WithMessage("Required argument 'MinimatchPatterns' has not been set");
        }
    }
}
