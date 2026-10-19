using ByMyPC.Models.RAMModels.DTO;
using FluentValidation;

namespace ByMyPC.Models.RAMModels
{
    public class ValidatorRAMCreate : AbstractValidator<DTORAMCreate>
    {
        public ValidatorRAMCreate()
        {
            RuleFor(x => x.name)
                .NotNull().WithErrorCode("NAME_NULL").WithMessage("Name can't be null")
                .NotEmpty().WithErrorCode("NAME_EMPTY").WithMessage("Name can't be null");

            RuleFor(x => x.size)
                .GreaterThanOrEqualTo(0).WithErrorCode("WRONG_SIZE").WithMessage("Ram Size can't be lower zero");

            RuleFor(x => x.frequency)
                .GreaterThanOrEqualTo(0).WithErrorCode("WRONG_FREQUENCY").WithMessage("Frequency Size can't be lower zero");

        }
    }

}
