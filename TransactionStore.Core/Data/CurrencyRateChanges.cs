namespace TransactionStore.Core.Data;

public static class CurrencyRateChanges
{
    // Метод для обновления значения по индексу
    public static void UpdateCoefficient(int index, double newValue)
    {
        if (index < 0 || index >= ArrayOfCoefficients.Coefficients.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
        }

        ArrayOfCoefficients.Coefficients[index] = newValue;
    }
}