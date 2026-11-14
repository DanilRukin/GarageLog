using MediatR;

namespace GarageLog.Application.Vehicles.RegisterVehicle;

/// <summary>
/// Команда регистрации нового автомобиля.
/// </summary>
/// <param name="Vin">VIN (17 символов).</param>
/// <param name="Make">Марка.</param>
/// <param name="Model">Модель.</param>
/// <param name="Year">Год выпуска.</param>
/// <param name="LicensePlate">Номерной знак (опционально).</param>
/// <param name="Engine">Двигатель (опционально).</param>
/// <param name="Transmission">Трансмиссия (опционально).</param>
/// <param name="InitialOdometerKm">Начальный пробег (опционально).</param>
public record RegisterVehicleCommand(
    string Vin,
    string Make,
    string Model,
    int Year,
    string? LicensePlate = null,
    string? Engine = null,
    string? Transmission = null,
    int? InitialOdometerKm = null) : IRequest<RegisterVehicleResponse>;
