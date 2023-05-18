using Molinos.DataAgro.Entities.Dto;

namespace Molinos.DataAgro.Entities.Validations
{
    public interface IEntityKeyValid
    {
        bool ValidateKey(Resultado oErrorMessages);
        bool Validate(Resultado oErrorMessages);
    }
}