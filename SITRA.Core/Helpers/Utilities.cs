using SITRA.Core.Enums;
using System.Globalization;
using System.Resources;
using System.Reflection;

namespace SITRA.Core.Helpers
{
    public class Utilities
    {
        private static readonly ResourceManager _resourceManager = new ResourceManager("SITRA.Core.Resources.ValidationMessages", Assembly.GetExecutingAssembly());

        public static string RequiredFieldMessage => _resourceManager.GetString("RequiredFieldMessage")!;
        public static string MinimunLengthMessage => _resourceManager.GetString("MinimunLengthMessage")!;
        public static string MaximunLengthMessage => _resourceManager.GetString("MaximunLengthMessage")!;
        public static string FirstCharUpperMessage => _resourceManager.GetString("FirstCharUpperMessage")!;
        public static string EmailMessage => _resourceManager.GetString("EmailMessage")!;
        public static string GreatherThanZero => _resourceManager.GetString("GreatherThanZero")!;
        public static string GreaterThanOrEqualToZero => _resourceManager.GetString("GreaterThanOrEqualToZero")!;
        public static string NameAlreadyExistsWith => _resourceManager.GetString("NameAlreadyExistsWith")!;
        public static string CodeAlreadyExistsWith => _resourceManager.GetString("CodeAlreadyExistsWith")!;
        public static string NotExistsWith => _resourceManager.GetString("NotExistsWith")!;
        public static string InvalidEnumValueMessage => _resourceManager.GetString("InvalidEnumValueMessage")!;
        public static string RequiredFieldForConditionMessage => _resourceManager.GetString("RequiredFieldForConditionMessage")!;
        public static string MaximumSupportsReachedMessage => _resourceManager.GetString("MaximumSupportsReachedMessage")!;
        public static string IdNotExistMessage => _resourceManager.GetString("IdNotExistMessage")!;
        public static string IdempotencyKeyRequiredMessage => _resourceManager.GetString("IdempotencyKeyRequiredMessage")!;
        public static string ReceiptAlreadyPaidMessage => _resourceManager.GetString("ReceiptAlreadyPaidMessage")!;
        public static string WithdrawnAmountWontBeGreatherThanAvailableCash => _resourceManager.GetString("WithdrawnAmountWontBeGreatherThanAvailableCash")!;
        public static string MinItemsRequired => _resourceManager.GetString("MinItemsRequired")!;
        public static string EachItemMustBeGreatherThanZero => _resourceManager.GetString("EachItemMustBeGreatherThanZero")!;
        public static string RucFieldMessage => _resourceManager.GetString("RucFieldMessage")!;
        public static string PhoneNumberFieldMessage => _resourceManager.GetString("PhoneNumberFieldMessage")!;
        public static string AtLeastOneServiceToChose => _resourceManager.GetString("AtLeastOneServiceToChose")!;
        public static string NotDuplicatesWith => _resourceManager.GetString("NotDuplicatesWith")!;
        public static string AppointmentAlreadyExist => _resourceManager.GetString("AppointmentAlreadyExist")!;
        public static string InvalidCredentialsMessage => _resourceManager.GetString("InvalidCredentialsMessage")!;
        public static string PasswordMinLengthMessage => _resourceManager.GetString("PasswordMinLengthMessage")!;
        public static string UserNotAuthMessage => _resourceManager.GetString("UserNotAuthMessage")!;
        public static string PettyCashRequiredMessage => _resourceManager.GetString("PettyCashRequiredMessage")!;
        public static string PettyCashOwnedByMessage => _resourceManager.GetString("PettyCashOwnedByMessage")!;
        public static string CurrencyNotFoundMessage => _resourceManager.GetString("CurrencyNotFoundMessage")!;
        public static string FiscalSupportNotFoundMessage => _resourceManager.GetString("FiscalSupportNotFoundMessage")!;
        public static string ItemNotFoundMessage => _resourceManager.GetString("ItemNotFoundMessage")!;
        public static string StockNotConfiguredMessage => _resourceManager.GetString("StockNotConfiguredMessage")!;
        public static string QuotationNotFoundMessage => _resourceManager.GetString("QuotationNotFoundMessage")!;
        public static string NoOpenPettyCashMessage => _resourceManager.GetString("NoOpenPettyCashMessage")!;
        public static string AlreadyOpenPettyCashMessage => _resourceManager.GetString("AlreadyOpenPettyCashMessage")!;
        public static string DuplicateServicesMessage => _resourceManager.GetString("DuplicateServicesMessage")!;
        public static string WarehouseNotFoundMessage => _resourceManager.GetString("WarehouseNotFoundMessage")!;
        public static string OriginStockNotFoundMessage => _resourceManager.GetString("OriginStockNotFoundMessage")!;
        public static string InsufficientStockQuantityMessage => _resourceManager.GetString("InsufficientStockQuantityMessage")!;
        public static string TransferStatusChangeErrorMessage => _resourceManager.GetString("TransferStatusChangeErrorMessage")!;
        public static string TransferStockNotFoundMessage => _resourceManager.GetString("TransferStockNotFoundMessage")!;
        public static string TransferIdInvalidMessage => _resourceManager.GetString("TransferIdInvalidMessage")!;
        public static string CommentTooLongMessage => _resourceManager.GetString("CommentTooLongMessage")!;
        public static string OneOrMoreServicesInvalidMessage => _resourceManager.GetString("OneOrMoreServicesInvalidMessage")!;
        public static string ContactNotFoundOrInvalidMessage => _resourceManager.GetString("ContactNotFoundOrInvalidMessage")!;

        public static string GreaterThanOrEqualToMensaje(DateTime minDate)
        {
            var message = _resourceManager.GetString("GreaterThanOrEqualToMensaje");
            return string.Format(message!, minDate.ToString("yyyy-MM-dd"));
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
            var message = _resourceManager.GetString("MaximunLengthMessage");
            return message!.Replace("{MaxLength}", maxLength.ToString());
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

        public static string FiscalSupportAlreadyExistsMessage(FiscalSupportType supportType, long warehouseId)
        {
            var message = _resourceManager.GetString("FiscalSupportAlreadyExistsMessage");
            return string.Format(message!, supportType, warehouseId);
        }
    }
}
