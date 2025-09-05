using Auth.Exception.CustomExceptions;
using Auth.Exception.ErrorMessages;

namespace Auth.Domain.ValueObjects;
public class Cpf
{
    public string Value { get; private set; }
    public Cpf(string value)
    {
        if (!IsValid(value))
            throw new OnValidationException(ResourceErrorMessages.CPF_INVALID);
        Value = FormatCpf(value);
    }
    private bool IsValid(string cpf)
    {
        cpf = new string(cpf.Where(char.IsDigit).ToArray());

        // Validation
        if (cpf.Length != 11 || cpf.Distinct().Count() == 1)
            return false;
        var numbers = cpf.Select(c => int.Parse(c.ToString())).ToArray();


        // First digit
        var sum = 0;
        for (int i = 0; i < 9; i++)
            sum += numbers[i] * (10 - i);
        var firstCheckDigit = sum % 11 < 2 ? 0 : 11 - (sum % 11);
        if (numbers[9] != firstCheckDigit)
            return false;

        // Second digit
        sum = 0;
        for (int i = 0; i < 10; i++)
            sum += numbers[i] * (11 - i);
        var secondCheckDigit = sum % 11 < 2 ? 0 : 11 - (sum % 11);
        if (numbers[10] != secondCheckDigit)
            return false;
        return true;
    }
    private static string FormatCpf(string cpf)
    {
        // Remove dots and hyphen
        cpf = new string(cpf.Where(char.IsDigit).ToArray());

        // Format as XXX.XXX.XXX-XX
        return $"{cpf.Substring(0, 3)}.{cpf.Substring(3, 3)}.{cpf.Substring(6, 3)}-{cpf.Substring(9, 2)}";
    }

    public override string ToString() => Value;
}
