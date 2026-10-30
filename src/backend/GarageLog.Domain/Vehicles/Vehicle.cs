using GarageLog.Domain.Infrastructure;
using GarageLog.Domain.Vehicles.Events;
using GarageLog.SharedKernel;
using GarageLog.SharedKernel.Interfaces;
using static GarageLog.Domain.Errors.VehicleErrors;

namespace GarageLog.Domain.Vehicles;

/// <summary>
/// Автомобиль
/// </summary>
public class Vehicle : EntityBase<Guid>, IAgregateRoot
{
    /// <summary>
    /// История пробега
    /// </summary>
    private readonly List<OdometerReading> _odometerHistory = new();

    /// <summary>
    /// Id собственника
    /// </summary>
    public Guid OwnerId { get; private set; }

    /// <summary>
    /// VIN
    /// </summary>
    public Vin Vin { get; private set; } = default!;

    /// <summary>
    /// Гос. номер
    /// </summary>
    public LicensePlate? LicensePlate { get; private set; }
    /// <summary>
    /// Марка
    /// </summary>
    public string Make { get; private set; } = default!;
    /// <summary>
    /// Модель
    /// </summary>
    public string Model { get; private set; } = default!;
    /// <summary>
    /// Год выпуска
    /// </summary>
    public int Year { get; private set; }
    /// <summary>
    /// Наименование (номер) двигателя
    /// </summary>
    public string? Engine { get; private set; }
    /// <summary>
    /// Наименование (номер) трансмиссии (рамы)
    /// </summary>
    public string? Transmission { get; private set; }
    /// <summary>
    /// Текущий статус
    /// </summary>
    public VehicleStatus Status { get; private set; }
    /// <summary>
    /// Дата создания
    /// </summary>
    public DateTimeOffset CreatedAt { get; private set; }
    /// <summary>
    /// Дата обновления
    /// </summary>
    public DateTimeOffset UpdatedAt { get; private set; }
    /// <summary>
    /// Дата продажи
    /// </summary>
    public DateOnly? SoldAt { get; private set; }
    /// <summary>
    /// История пробега
    /// </summary>
    public IReadOnlyCollection<OdometerReading> OdometerHistory => _odometerHistory.AsReadOnly();
    /// <summary>
    /// Текущие показания
    /// </summary>
    public OdometerReading? CurrentOdometer => _odometerHistory.Count == 0 ? null : _odometerHistory[^1];

    private Vehicle() { }

    /// <summary>
    /// Регистрирует новый автомобиль
    /// </summary>
    /// <param name="ownerId">Код владельца</param>
    /// <param name="vin">VIN номер</param>
    /// <param name="make">Марка</param>
    /// <param name="model">Модель</param>
    /// <param name="year">Год выпуска</param>
    /// <param name="licensePlate">Гос. номер</param>
    /// <param name="engine">Наименование двигателя</param>
    /// <param name="transmission">Наименование трансмиссии</param>
    /// <param name="initialOdometer">Начальный пробег</param>
    public static Vehicle Register(
        Guid ownerId,
        Vin vin,
        string make,
        string model,
        int year,
        LicensePlate? licensePlate = null,
        string? engine = null,
        string? transmission = null,
        OdometerReading? initialOdometer = null
        )
    {
        if (ownerId == default(Guid))
            throw new DomainException<Vehicle>(OwnerIdIsDefault);
        CheckMake(make);
        CheckModel(model);

        CheckYear(year);

        DateTimeOffset now = DateTimeOffset.UtcNow;
        Vehicle vehicle = new Vehicle
        {
            Id = Guid.NewGuid(),
            OwnerId = ownerId,
            Vin = vin,
            LicensePlate = licensePlate,
            Make = make.Trim(),
            Model = model.Trim(),
            Year = year,
            Engine = engine?.Trim(),
            Transmission = transmission?.Trim(),
            Status = VehicleStatus.Active,
            CreatedAt = now,
            UpdatedAt = now
        };

        if (initialOdometer is not null)
            vehicle._odometerHistory.Add(initialOdometer);
        vehicle.AddDomainEvent(new VehicleRegistered(vehicle.Id, ownerId, vin.Value, licensePlate?.Value));

        return vehicle;
    }

    /// <summary>
    /// Обновляет паспортные данные
    /// </summary>
    /// <param name="make">Марка</param>
    /// <param name="model">Модель</param>
    /// <param name="year">Год выпуска</param>
    /// <param name="licensePlate">Гос. номер</param>
    /// <param name="engine">Наименование двигателя</param>
    /// <param name="transmission">Наименование трансмиссии</param>
    public void UpdateDetails(
        string make,
        string model,
        int year,
        LicensePlate? licensePlate,
        string? engine,
        string? transmission
        )
    {
        EnsureActive();
        CheckMake(make);
        CheckModel(model);
        CheckYear(year);

        Make = make.Trim();
        Model = model.Trim();
        Year = year;
        LicensePlate = licensePlate;
        Engine = engine?.Trim();
        Transmission = transmission?.Trim();
        Touch();
        AddDomainEvent(new VehicleDetailsUpdated(Id));
    }

    /// <summary>
    /// Обновляет пробег
    /// </summary>
    /// <param name="reading">Новые показания пробега</param>
    /// <param name="correctionReason">Причина корректировки</param>
    public void UpdateOdometer(OdometerReading reading, string? correctionReason = null)
    {
        EnsureActive();
        OdometerReading? current = CurrentOdometer;
        bool isCorrection = current is not null && reading.Kilometers < current.Kilometers;
        if (isCorrection)
        {
            if (reading.Source != OdometerSource.Correction)
                throw new DomainException<Vehicle>(CorrectionRequiresSource);
            if (string.IsNullOrWhiteSpace(correctionReason))
                throw new DomainException<Vehicle>(CorrectionRequiresReason);
        }
        else if (reading.Source == OdometerSource.Correction)
        {
            throw new DomainException<Vehicle>(CorrectionOnlyForDecrease);
        }
        if (current is not null && reading.TakenAt < current.TakenAt)
            throw new DomainException<Vehicle>(ReadingTakenAtCanNotBeEarlierThanCurrentTakenAt);

        // Защита от дубликата: то же значение на ту же дату
        if (current is not null &&
            current.Kilometers == reading.Kilometers &&
            current.TakenAt == reading.TakenAt)
        {
            return; // идемпотентно — ничего не делаем
        }

        Touch();

        AddDomainEvent(new OdometerUpdated(
            Id,
            current?.Kilometers ?? 0, 
            reading.Kilometers, 
            reading.Source,
            isCorrection, 
            isCorrection ? correctionReason!.Trim() : null,
            reading.TakenAt));
    }
    /// <summary>
    /// Обновляет пробег
    /// </summary>
    public void UpdateOdometer(int kilometers, DateTimeOffset takenAt, OdometerSource source = OdometerSource.Manual, string? correctionReason = null)
        => UpdateOdometer(OdometerReading.Of(kilometers, takenAt, source), correctionReason);

    /// <summary>
    /// Продает автомобиль
    /// </summary>
    /// <param name="soldDate">Дата продажи</param>
    public void Sell(DateOnly soldDate)
    {
        EnsureActive();
        if (soldDate > DateOnly.FromDateTime(DateTime.UtcNow))
            throw new DomainException<Vehicle>(SellDateInTheFuture);
        Status = VehicleStatus.Sold;
        SoldAt = soldDate;
        Touch();
        AddDomainEvent(new VehicleSold(Id, soldDate));
    }

    /// <summary>
    /// Архивирует автомобиль
    /// </summary>
    public void Archive()
    {
        if (Status == VehicleStatus.Archived)
            throw new DomainException<Vehicle>(VehiscleIsAlreadyArchived);

        Status = VehicleStatus.Archived;
        Touch();

        AddDomainEvent(new VehicleArchived(Id));
    }

    private static void CheckMake(string make)
    {
        if (string.IsNullOrWhiteSpace(make))
            throw new DomainException<Vehicle>(MakeIsEmpty);
    }

    private static void CheckModel(string model)
    {
        if (string.IsNullOrWhiteSpace(model))
            throw new DomainException<Vehicle>(ModelIsEmpty);
    }

    private static void CheckYear(int year)
    {
        int currentYear = DateTimeOffset.UtcNow.Year;
        if (year < 1886 || year > (currentYear + 1))
            throw new DomainException<Vehicle>(InvalidYear);
    }

    private void EnsureActive()
    {
        if (Status != VehicleStatus.Active)
            throw new DomainException<Vehicle>(VehicleIsNotActive);
    }

    private void Touch() => UpdatedAt = DateTimeOffset.UtcNow;
}
