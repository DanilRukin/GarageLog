namespace GarageLog.Domain.Errors;

public static class OdometerReadingErrors
{
    public const string OdometerReadingIsNegative = "odometer_reading_is_negative";
    public const string OdometerReadingsInTheFuture = "odometer_readings_in_the_future";
}
