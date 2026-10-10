namespace SITRA.Core.Enums
{
    public enum EcfType
    {
        // ── Tradicionales (NCF tipo B) — SpaceNumber = 8, sin envío a DGII ──
        B01 = 1,   // Factura de Crédito Fiscal
        B02 = 2,   // Factura de Consumo
        B03 = 3,   // Nota de Débito
        B04 = 4,   // Nota de Crédito
        B11 = 11,  // Comprobante de Compras
        B12 = 12,  // Registro Único de Ingresos (RUI)
        B13 = 13,  // Comprobante para Gastos Menores
        B14 = 14,  // Comprobante para Regímenes Especiales
        B15 = 15,  // Comprobante Gubernamental
        B16 = 16,  // Comprobante para Exportaciones
        B17 = 17,  // Comprobante para Pagos al Exterior

        // ── Electrónicos (e-CF) — SpaceNumber = 10, envío a DGII ──
        E31 = 31,  // Factura de Crédito Fiscal Electrónica
        E32 = 32,  // Factura de Consumo Electrónica
        E33 = 33,  // Nota de Débito Electrónica
        E34 = 34,  // Nota de Crédito Electrónica
        E41 = 41,  // Comprobante Electrónico de Compras
        E43 = 43,  // Comprobante Electrónico para Gastos Menores
        E44 = 44,  // Comprobante Electrónico para Regímenes Especiales
        E45 = 45,  // Comprobante Electrónico Gubernamental
        E46 = 46,  // Comprobante Electrónico para Exportaciones
        E47 = 47   // Comprobante Electrónico para Pagos al Exterior
    }
}
