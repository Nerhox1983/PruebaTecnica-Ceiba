using FluentValidation;
using CourierMax.Domain.Enums;

namespace CourierMax.Application.Shipments.Commands.CreateShipment
{
    public class CreateShipmentCommandValidator : AbstractValidator<CreateShipmentCommand>
    {
        public CreateShipmentCommandValidator()
        {            
            RuleFor(x => x.SenderName).NotEmpty().WithMessage("El nombre del remitente es requerido.");
            
            RuleFor(x => x.PackageType)
                .NotEmpty().WithMessage("El tipo de paquete es requerido.")
                .IsEnumName(typeof(PackageType), caseSensitive: false)
                .WithMessage("El tipo de paquete enviado no es válido. Valores permitidos: Documento, Paquete, Perecedero.");

            RuleFor(x => x.ServiceType)
                .NotEmpty().WithMessage("El tipo de servicio es requerido.")
                .IsEnumName(typeof(ServiceType), caseSensitive: false)
                .WithMessage("El tipo de servicio no es válido. Valores permitidos: Estandar, Express, MismoDia.");
        }
    }
}
