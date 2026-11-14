namespace GarageLog.Application.Vehicles.RegisterVehicle;

/// <summary>Результат регистрации автомобиля.</summary>
/// <param name="VehicleId">Идентификатор созданного автомобиля.</param>
public record RegisterVehicleResponse(Guid VehicleId);
