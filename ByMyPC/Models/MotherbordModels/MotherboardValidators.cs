using ByMyPC.Models.MotherbordModels.DTO;
using FluentValidation;

namespace ByMyPC.Models.MotherbordModels
{
    public class MotherboardUpdateValidator : AbstractValidator<DTOMotherboardUpdateModel>
    {
        public MotherboardUpdateValidator()
        {
            RuleFor(x => x.Name)
                .NotNull().WithErrorCode("NAME_IS_NULL").WithMessage("Name can't be null")
                .NotEmpty().WithErrorCode("NAME_IS_EMPTY").WithMessage("Name can't be empty");
            
            RuleFor(x => x.Socket)
               .NotNull().WithErrorCode("SOCKET_IS_NULL").WithMessage("Socket can't be null")
               .NotEmpty().WithErrorCode("SOCKET_IS_EMPTY").WithMessage("Socket can't be empty");

        }
    }

    public class MotherboardCreateValidator : AbstractValidator<DTOMotherboardCreateModel>
    {
        public MotherboardCreateValidator()
        {
            RuleFor(x => x.Name)
                .NotNull().WithErrorCode("NAME_IS_NULL").WithMessage("Name can't be null")
                .NotEmpty().WithErrorCode("NAME_IS_EMPTY").WithMessage("Name can't be empty");

            RuleFor(x => x.Socket)
               .NotNull().WithErrorCode("SOCKET_IS_NULL").WithMessage("Socket can't be null")
               .NotEmpty().WithErrorCode("SOCKET_IS_EMPTY").WithMessage("Socket can't be empty");


            RuleFor(x => x.MaxRamSlot)
                .GreaterThan(-1).WithErrorCode("MAXRAMSLOT_WRONG").WithMessage("Max Ram Slot can't be lower zero");

            RuleFor(x => x.MaxRamFrequency)
              .GreaterThan(-1).WithErrorCode("MAXRAMFREQUENCY_WRONG").WithMessage("Max Ram Frequency can't be lower zero");

            RuleFor(x => x.MaxCpuFrequency)
              .GreaterThan(-1).WithErrorCode("MAXCPUFREQUENCY_WRONG").WithMessage("Max CPU Frequency can't be lower zero");


        }
    }
}
