namespace SITRA.Core.Enums
{
    public enum DgiiStatus
    {
        Rechazado = 1,
        Aceptado = 2,
        AceptadoCondicional = 3,
        EnProceso = 4,
        Error = 5
    }

    public static class DgiiStatusExtensions
    {
        public static string ToDatabaseString(this DgiiStatus status)
        {
            return status switch
            {
                DgiiStatus.Rechazado => "Rechazado",
                DgiiStatus.Aceptado => "Aceptado",
                DgiiStatus.AceptadoCondicional => "Aceptado Condicional",
                DgiiStatus.EnProceso => "En Proceso",
                DgiiStatus.Error => "Error",
                _ => status.ToString()
            };
        }
    }
}
