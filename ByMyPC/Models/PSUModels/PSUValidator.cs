using ByMyPC.Models.PSUModels.DTO;
using FluentValidation;

namespace ByMyPC.Models.PSUModels
{
    public class PSUValidator : AbstractValidator<DTOPSUModelCreate>
    {
        public PSUValidator()
        {
            RuleFor(x => x.Name).NotNull()
                                .NotEmpty()
                                .WithErrorCode("NAME_EMPTY")
                                .WithMessage("Name can't be null");

            RuleFor(x => x.PowerWatt).GreaterThan(-1)
                                     .WithErrorCode("NAME_EMPTY")
                                     .WithMessage("Name can't be null");


        }
    }
}
