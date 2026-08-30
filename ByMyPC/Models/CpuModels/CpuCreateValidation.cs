using ByMyPC.Models.CpuModels.DTO;
using FluentValidation;

namespace ByMyPC.Models.CpuModels
{
    public class CpuCreateValidation : AbstractValidator<DTOCpuCreateModel>
    {
        public CpuCreateValidation()
        {
            RuleFor(x => x.Frequency)
                .NotNull().WithErrorCode("FREQUENCY_ISNULL").WithMessage("Frequency can't be null")
                .GreaterThan(-1).WithErrorCode("FREQUENCY_LOWER_ZERO").WithMessage("Frequency can't be lower zero");

            RuleFor(x => x.Count_Cores)
                .NotNull().WithErrorCode("CORES_ISNULL").WithMessage("Count cores can't be null")
                .GreaterThan(-1).WithErrorCode("CORES_LOWER_ZERO").WithMessage("Count cores can't be lower zero");

            RuleFor(x => x.IsLive)
                .NotNull().WithErrorCode("ISLIVE_ISNULL").WithMessage("Is live state can't be null");

            RuleFor(x => x.Name)
                .NotNull().WithErrorCode("NAME_ISNULL").WithMessage("Name can't be null")
                .NotEmpty().WithErrorCode("NAME_EMPTY").WithMessage("Name can't be empty");

            RuleFor(x => x.Socket)
               .NotNull().WithErrorCode("SOCKET_ISNULL").WithMessage("Socket can't be null")
               .NotEmpty().WithErrorCode("SOCKET_EMPTY").WithMessage("Socket can't be empty");
        }
    }

    public class CpuUpdateValidation : AbstractValidator<DTOCpuUpdateModel>
    {
        public CpuUpdateValidation()
        {
            RuleFor(x => x.Frequency)
                .NotNull().WithErrorCode("FREQUENCY_ISNULL").WithMessage("Frequency can't be null")
                .GreaterThan(-1).WithErrorCode("FREQUENCY_LOWER_ZERO").WithMessage("Frequency can't be lower zero");

            RuleFor(x => x.Count_Cores)
                .NotNull().WithErrorCode("CORES_ISNULL").WithMessage("Count cores can't be null")
                .GreaterThan(-1).WithErrorCode("CORES_LOWER_ZERO").WithMessage("Count cores can't be lower zero");

            RuleFor(x => x.IsLive)
                .NotNull().WithErrorCode("ISLIVE_ISNULL").WithMessage("Is live state can't be null");

        }
    }
}
