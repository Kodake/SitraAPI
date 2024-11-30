namespace BackEnd.Helpers
{
    public class Utilities
    {
        public static string RequiredFieldMessage = "El campo {PropertyName} es requerido";
        public static string MinimunLengthMessage = "El campo {PropertyName} debe tener más de {MinLength} caracteres";
        public static string MaximunLengthMessage = "El campo {PropertyName} debe tener menos de {MaxLength} caracteres";
        public static string FirstCharUpperMessage = "El campo {PropertyName} debe comenzar con mayúsculas";
        public static string EmailMessage = "El campo {PropertyName} debe ser un email válido";
        public static string GreatherThanZero = "El campo {PropertyName} debe ser mayor a ";
        public static string NameAlreadyExistsWith = "Ya existe con el nombre {0}";
        public static string NotExistsWith = "No existe {PropertyName} con el valor {0}";
        public static string InvalidEnumValueMessage = "El campo {PropertyName} tiene un valor inválido";
        public static string RequiredFieldForConditionMessage = "El campo {PropertyName} es obligatorio cuando {0} es {1}";
        public static string MaximumSupportsReachedMessage = "Maximum fiscal supports reached for this warehouse";

        public static string GreaterThanOrEqualToMensaje(DateTime minDate)
        {
            return "El campo {PropertyName} debe ser posterior a " + minDate.ToString("yyyy-MM-dd");
        }

        public static bool FirstCharUppercase(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                return true;
            }

            var firstChar = valor[0].ToString();

            return firstChar == firstChar.ToUpper();
        }

        public static string MaxLengthMessage(int maxLength)
        {
            return $"El campo {{PropertyName}} debe tener menos de {maxLength} caracteres";
        }

        public class InvoiceItemCalculator
        {
            public static double CalculateTotalAmount(double basePrice, double quantity)
            {
                return basePrice * quantity;
            }

            public static double CalculateTotalTaxes(double basePrice, double taxAmount, double quantity)
            {
                return (basePrice * taxAmount / 100) * quantity;
            }

            public static double CalculateTotalPrice(double totalAmount, double totalTaxes)
            {
                return totalAmount + totalTaxes;
            }
        }
    }
}
