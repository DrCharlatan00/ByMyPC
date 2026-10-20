using ByMyPC.Models.PCModels.DTO;
using FluentValidation;

namespace ByMyPC.Models.PCModels
{
    public class PCValidatorCreate : AbstractValidator<DTOPCCreateModel>
    {
        public PCValidatorCreate()
        {
            RuleFor(x => x.name)
               .NotNull().NotEmpty()
               .WithErrorCode("NAME_NULL_OR_NOT_CORRECT").WithMessage("Name can't be null or empty");
        }
    }
}
