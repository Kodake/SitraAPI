using SITRA.Core.Enums;
using System.Reflection;
using System.Resources;

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
        public static string EmailAlreadyExistsMessage(string email)
        {
            var message = _resourceManager.GetString("EmailAlreadyExistsMessage");
            return string.Format(message!, email);
        }
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
        public static string PettyCashNotOpenMessage => _resourceManager.GetString("PettyCashNotOpenMessage")!;
        public static string PettyCashOpenedByOtherUserMessage => _resourceManager.GetString("PettyCashOpenedByOtherUserMessage")!;
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
        public static string NoSigningKeysFoundMessage => _resourceManager.GetString("NoSigningKeysFoundMessage")!;

        // Role and Permission validation messages
        public static string RoleNameRequiredMessage => _resourceManager.GetString("RoleNameRequiredMessage")!;
        public static string RoleDescriptionRequiredMessage => _resourceManager.GetString("RoleDescriptionRequiredMessage")!;
        public static string RoleAlreadyExistsMessage => _resourceManager.GetString("RoleAlreadyExistsMessage")!;
        public static string RoleNotFoundMessage => _resourceManager.GetString("RoleNotFoundMessage")!;
        public static string CannotDeleteSystemRoleMessage => _resourceManager.GetString("CannotDeleteSystemRoleMessage")!;
        public static string PermissionIdsRequiredMessage => _resourceManager.GetString("PermissionIdsRequiredMessage")!;
        public static string InvalidPermissionIdMessage => _resourceManager.GetString("InvalidPermissionIdMessage")!;
        public static string RoleNameMaxLengthMessage => _resourceManager.GetString("RoleNameMaxLengthMessage")!;
        public static string RoleDescriptionMaxLengthMessage => _resourceManager.GetString("RoleDescriptionMaxLengthMessage")!;

        // Report Titles
        public static string StockReportTitle => _resourceManager.GetString("StockReportTitle")!;
        public static string SalesReportTitle => _resourceManager.GetString("SalesReportTitle")!;
        public static string PurchasesReportTitle => _resourceManager.GetString("PurchasesReportTitle")!;
        public static string QuotationsTitle => _resourceManager.GetString("QuotationsTitle")!;
        public static string ClosePettyCashTitle => _resourceManager.GetString("ClosePettyCashTitle")!;
        public static string InvoiceReceiptTitle => _resourceManager.GetString("InvoiceReceiptTitle")!;
        public static string InvoiceTitle => _resourceManager.GetString("InvoiceTitle")!;

        // Document Labels
        public static string IdentificationLabel => _resourceManager.GetString("IdentificationLabel")!;
        public static string DateLabel => _resourceManager.GetString("DateLabel")!;
        public static string NameLabel => _resourceManager.GetString("NameLabel")!;
        public static string NumberLabel => _resourceManager.GetString("NumberLabel")!;
        public static string FiscalSupportLabel => _resourceManager.GetString("FiscalSupportLabel")!;
        public static string DueDateLabel => _resourceManager.GetString("DueDateLabel")!;
        public static string DescriptionLabel => _resourceManager.GetString("DescriptionLabel")!;
        public static string QuantityLabel => _resourceManager.GetString("QuantityLabel")!;
        public static string TotalLabel => _resourceManager.GetString("TotalLabel")!;
        public static string SubtotalLabel => _resourceManager.GetString("SubtotalLabel")!;
        public static string TaxesLabel => _resourceManager.GetString("TaxesLabel")!;
        public static string UserLabel => _resourceManager.GetString("UserLabel")!;
        public static string ConditionLabel => _resourceManager.GetString("ConditionLabel")!;
        public static string SpotConditionLabel => _resourceManager.GetString("SpotConditionLabel")!;
        public static string CreditConditionLabel => _resourceManager.GetString("CreditConditionLabel")!;
        public static string InvoiceLabel => _resourceManager.GetString("InvoiceLabel")!;
        public static string FiscalSupportNumberLabel => _resourceManager.GetString("FiscalSupportNumberLabel")!;
        public static string RncCedulaLabel => _resourceManager.GetString("RncCedulaLabel")!;
        public static string CustomerLabel => _resourceManager.GetString("CustomerLabel")!;
        public static string ItemsLabel => _resourceManager.GetString("ItemsLabel")!;
        public static string TaxLabelShort => _resourceManager.GetString("TaxLabelShort")!;
        public static string TotalLabelShort => _resourceManager.GetString("TotalLabelShort")!;
        public static string ItbisLabel => _resourceManager.GetString("ItbisLabel")!;
        public static string AmountToPayLabel => _resourceManager.GetString("AmountToPayLabel")!;
        public static string PaymentMethodsLabel => _resourceManager.GetString("PaymentMethodsLabel")!;
        public static string ReceiptFooterMessage => _resourceManager.GetString("ReceiptFooterMessage")!;
        public static string AmountReturnedLabel => _resourceManager.GetString("AmountReturnedLabel")!;
        public static string SellerLabel => _resourceManager.GetString("SellerLabel")!;
        public static string QuotationNumberLabel => _resourceManager.GetString("QuotationNumberLabel")!;
        public static string ProductLabel => _resourceManager.GetString("ProductLabel")!;
        public static string FromLabel => _resourceManager.GetString("FromLabel")!;
        public static string ToLabel => _resourceManager.GetString("ToLabel")!;
        public static string CashierLabel => _resourceManager.GetString("CashierLabel")!;
        public static string OpeningDateLabel => _resourceManager.GetString("OpeningDateLabel")!;
        public static string ClosingDateLabel => _resourceManager.GetString("ClosingDateLabel")!;
        public static string OpeningAmountLabel => _resourceManager.GetString("OpeningAmountLabel")!;
        public static string BilledAmountLabel => _resourceManager.GetString("BilledAmountLabel")!;
        public static string WithdrawalsLabel => _resourceManager.GetString("WithdrawalsLabel")!;
        public static string SummaryByMethodLabel => _resourceManager.GetString("SummaryByMethodLabel")!;
        public static string ExpectedCashLabel => _resourceManager.GetString("ExpectedCashLabel")!;
        public static string ClosingAmountLabel => _resourceManager.GetString("ClosingAmountLabel")!;
        public static string DifferenceLabel => _resourceManager.GetString("DifferenceLabel")!;
        public static string CashierSignatureLabel => _resourceManager.GetString("CashierSignatureLabel")!;
        public static string LogoNotAvailableLabel => _resourceManager.GetString("LogoNotAvailableLabel")!;
        public static string ErrorLoadingLogoLabel => _resourceManager.GetString("ErrorLoadingLogoLabel")!;
        public static string PhoneLabel => _resourceManager.GetString("PhoneLabel")!;
        public static string RncLabel => _resourceManager.GetString("RncLabel")!;

        public static string UserEmptyTenantIdMessage(string email)
        {
            var message = _resourceManager.GetString("UserEmptyTenantIdMessage");
            return string.Format(message!, email);
        }

        public static string FailedToUpdateUserTenantIdMessage(string errors)
        {
            var message = _resourceManager.GetString("FailedToUpdateUserTenantIdMessage");
            return string.Format(message!, errors);
        }

        public static string FailedToUpdateUserClaimsMessage(string errors)
        {
            var message = _resourceManager.GetString("FailedToUpdateUserClaimsMessage") ?? "Failed to update user claims: {0}";
            return string.Format(message, errors);
        }

        public static string LoginUsingTenantIdMessage(Guid tenantId)
        {
            var message = _resourceManager.GetString("LoginUsingTenantIdMessage");
            return string.Format(message!, tenantId);
        }

        public static string TenantServiceUserNull => _resourceManager.GetString("TenantServiceUserNull")!;
        public static string TenantServiceClaimsAvailable => _resourceManager.GetString("TenantServiceClaimsAvailable")!;
        public static string TenantServiceClaimInfo(string type, string value)
        {
            var message = _resourceManager.GetString("TenantServiceClaimInfo");
            return string.Format(message!, type, value);
        }
        public static string TenantServiceFoundClaim(string? claimValue)
        {
            var message = _resourceManager.GetString("TenantServiceFoundClaim");
            return string.Format(message!, claimValue);
        }
        public static string TenantServiceFailedToParse => _resourceManager.GetString("TenantServiceFailedToParse")!;
        public static string StockReportErrorLoadingLogo(string message)
        {
            var res = _resourceManager.GetString("StockReportErrorLoadingLogo");
            return string.Format(res!, message);
        }
        public static string ReportErrorLoadingLogo(string message)
        {
            var res = _resourceManager.GetString("ReportErrorLoadingLogo");
            return string.Format(res!, message);
        }
        public static string QuotationDocumentErrorLoadingLogo(string message)
        {
            var res = _resourceManager.GetString("QuotationDocumentErrorLoadingLogo");
            return string.Format(res!, message);
        }
        public static string InvoiceDocumentErrorLoadingLogo(string message)
        {
            var res = _resourceManager.GetString("InvoiceDocumentErrorLoadingLogo");
            return string.Format(res!, message);
        }

        public static string GreaterThanOrEqualToMensaje(DateTime minDate)
        {
            var message = _resourceManager.GetString("GreaterThanOrEqualToMensaje");
            return string.Format(message!, minDate.ToString("yyyy-MM-dd"));
        }

        public static string MonthlySalesLimitReachedMessage(int limit, string plan)
        {
            var message = _resourceManager.GetString("MonthlySalesLimitReachedMessage");
            return string.Format(message!, limit, plan);
        }

        public static string FiscalSupportSequenceLimitReachedMessage(long maxSequence)
        {
            var message = _resourceManager.GetString("FiscalSupportSequenceLimitReachedMessage");
            return string.Format(message!, maxSequence);
        }

        public static string MaximumUsersReachedMessage(int limit, string plan)
        {
            var message = _resourceManager.GetString("MaximumUsersReachedMessage");
            return string.Format(message!, limit, plan);
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

        public static string FiscalSupportAlreadyExistsMessage(EcfType supportType)
        {
            var message = _resourceManager.GetString("FiscalSupportAlreadyExistsMessage");
            return string.Format(message!, supportType);
        }

        public static string GetPaymentMethodLabel(PaymentMethod method)
        {
            return method switch
            {
                PaymentMethod.Cash => _resourceManager.GetString("PaymentMethodCash")!,
                PaymentMethod.Transfer => _resourceManager.GetString("PaymentMethodTransfer")!,
                PaymentMethod.Card => _resourceManager.GetString("PaymentMethodCard")!,
                _ => method.ToString()
            };
        }
    }
}
