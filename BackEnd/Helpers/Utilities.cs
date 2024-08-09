namespace BackEnd.Helpers
{
    public class Utilities
    {
        public static string RequiredFieldMessage = "El campo {PropertyName} es requerido";
        public static string MinimunLengthMessage = "El campo {PropertyName} debe tener más de {MinLength} caracteres";
        public static string MaximunLengthMessage = "El campo {PropertyName} debe tener menos de {MaxLength} caracteres";
        public static string FirstCharUpperMessage = "El campo {PropertyName} debe comenzar con mayúsculas";
        public static string EmailMessage = "El campo {PropertyName} debe ser un email válido";

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
    }
}
