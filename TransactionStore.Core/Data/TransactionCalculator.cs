using TransactionStore.Core.Enums;

namespace TransactionStore.Core.Data;

public class TransactionCalculator
{
    // Метод для получения значения по индексу
    public double GetCoefficient(int index)
    {
        if (index < 0 || index >= ArrayOfCoefficients.Coefficients.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
        }

        return ArrayOfCoefficients.Coefficients[index];
    }

    // Метод для подсчета суммы транзакции с использованием коэффициента
    public double CalculateTransaction(int index, double amount, TransactionType type)
    {
        if (type == TransactionType.Deposit || type ==TransactionType.Transfer)
        {
            var coefficient = ArrayOfCoefficients.Coefficients[index];
            var result = amount * coefficient;
            return result;
        }
        else
        {
            var coefficient = ArrayOfCoefficients.Coefficients[index];
            var result = -1 * amount * coefficient;
            return result;
        }
    }
}