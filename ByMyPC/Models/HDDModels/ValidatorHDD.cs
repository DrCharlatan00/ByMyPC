using ByMyPC.Models.HDDModels.DTO;
using FluentValidation;

namespace ByMyPC.Models.HDDModels
{
    public class ValidatorHDDCreate : AbstractValidator<DTOHDDCreateModel>
    {
        public ValidatorHDDCreate()
        {
            RuleFor(x => x.name)
                .NotNull().WithErrorCode("NAME_IS_NULL").WithMessage("Name can't be null")
                .NotEmpty().WithErrorCode("NAME_IS_EMPTY").WithMessage("Name can't be empty");
            RuleFor(x => x.GbSize)
                .GreaterThan(-1).WithErrorCode("WRONG_SIZE").WithMessage("Disk can't be lower zero");
        }
    }
}
