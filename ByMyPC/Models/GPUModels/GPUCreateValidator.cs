using ByMyPC.Models.GPUModels.DTO;
using FluentValidation;

namespace ByMyPC.Models.GPUModels
{
    public class GPUCreateValidator : AbstractValidator<DTOGPUCreateModel>
    {
        public GPUCreateValidator()
        {
            RuleFor(x => x.name)
                .NotNull().NotEmpty()
                .WithErrorCode("NAME_EMPTY").WithMessage("Name can't be null");

            RuleFor(x => x.videoMemorySize)
                .GreaterThan(-1)
                .WithErrorCode("VIDEO_MEMORY_SIZE_MISTAKE").WithMessage("Video Memory can't be Greater Than 0");

            RuleFor(x => x.memoryBus)
                .GreaterThan(-1)
                .WithErrorCode("MEMORY_BUS_MISTAKE").WithMessage("Memory Bus can't be Greater Than 0");

            RuleFor(x => x.typeConnector)
                .NotNull().NotEmpty()
                .WithErrorCode("TYPE_CONNECTOR_EMPTY").WithMessage("Type Connector can't be null");

            RuleFor(x => x.typeMemory)
                .NotNull().NotEmpty()
                .WithErrorCode("TYPE_MEMORY_EMPTY").WithMessage("Type Memory can't be null");
        }
    }
}
