using CourierMax.Application.Shipments.Commands.CreateShipment;
using CourierMax.Application.Shipments.Commands;
using FluentValidation.TestHelper;

namespace CourierMax.Tests.Application.Validators
{
    public class CreateShipmentCommandValidatorTests
    {
        private readonly CreateShipmentCommandValidator _validator;

        public CreateShipmentCommandValidatorTests()
        {
            _validator = new CreateShipmentCommandValidator();
        }

        [Fact]
        public void Validator_ShouldNotHaveValidationError_WhenCommandIsValid()
        {            
            var command = new CreateShipmentCommand(
                "Sergio Cáceres", "3154433221", "Calle 100 # 15-22",
                "Tatiana Bernal", "3107654321", "Carrera 7 # 32-10",
                1, 2, 5.5m, 30, 20, 15,
                "Paquete", "Estandar", 15000
            );
         
            var result = _validator.TestValidate(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validator_ShouldHaveValidationError_WhenPackageTypeIsInvalid()
        {
            var command = new CreateShipmentCommand(
                "Sergio Cáceres", "3154433221", "Calle 100 # 15-22",
                "Tatiana Bernal", "3107654321", "Carrera 7 # 32-10",
                1, 2, 5.5m, 30, 20, 15,
                "Peligroso", "Estandar", 15000
            );

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.PackageType)
                  .WithErrorMessage("El tipo de paquete enviado no es válido. Valores permitidos: Documento, Paquete, Perecedero.");
        }

        [Fact]
        public void Validator_ShouldHaveValidationError_WhenSenderNameIsEmpty()
        { 
            var command = new CreateShipmentCommand(
                "", "3154433221", "Calle 100 # 15-22",
                "Tatiana Bernal", "3107654321", "Carrera 7 # 32-10",
                1, 2, 5.5m, 30, 20, 15,
                "Paquete", "Estandar", 15000
            );
            
            var result = _validator.TestValidate(command);
            
            result.ShouldHaveValidationErrorFor(x => x.SenderName)
                  .WithErrorMessage("El nombre del remitente es requerido.");
        }
    }
}
