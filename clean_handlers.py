import re
import os

file_path = r"c:\Users\rflorentino\Desktop\New folder\src\SitraAPI\SITRA.Core\Mediator\Handlers\GeneratedConcreteHandlers.cs"

# List of queries/commands that have custom hand-written handlers
custom_requests = {
    "GetUserByIdQuery",
    "GetSelectsServicesQuery",
    "GetStockProductsQuery",
    "GetSelectsProductsQuery",
    "GetUnregisteredPricingsForItemQuery",
    "GetItemsByPricingIdAndWarehouseIdQuery",
    "GetStockCombosQuery",
    "GetSelectsCombosQuery",
    "GetSelectsCategoriesQuery",
    "GetSelectsBrandsQuery",
    "GetAllSpotPaymentsQuery",
    "GetAllSpotPayablesQuery",
    "GetSaleInvoicesWithBalanceQuery",
    "GetSaleInvoicesBalanceQuery",
    "GetAllSaleInvoicesQuery",
    "GetPurchaseInvoicesWithBalanceQuery",
    "GetAllPurchaseInvoicesQuery",
    "GetSelectsExpensesQuery",
    "GetSelectsTaxesQuery",
    "GetAllDashboardsQuery",
    "GetSelectsCurrenciesQuery",
    "GetPettyCashByUserIdQuery",
    "GetAllPaymentMethodSummaryByIdQuery",
    "GetSelectsAppointmentsQuery",
    "GetSelectsFiscalSupportsQuery",
    "GetPermissionConfigQuery",
    "GetSelectsDeadlinesQuery",
    "GetPaysByIdQuery",
    "SetDefaultWarehouseCommand",
    "UploadCsvServiceCommand",
    "UploadCsvProductCommand",
    "SetDefaultPricingCommand",
    "SetDefaultCurrencyCommand",
    "SetDefaultContactCommand",
    "UpdateClosePettyCashCommand",
    "CreateWithdrawalPettyCashMovementCommand",
    "SetDefaultFiscalSupportCommand",
    "CreateWizardCommand",
    "IsWizardRequiredQuery",
    "CreateSaleInvoiceCommand",
    "CreatePurchaseInvoiceCommand",
    "CreateNoteCommand",
    "AnnulNcfCommand",
    "AssignPermissionsCommand"
}

with open(file_path, 'r', encoding='utf-8') as f:
    content = f.read()

# Pattern to capture class declaration blocks:
# public class [ClassName] : [BaseClass]<[BaseArgs]>(, IRequestHandler<...>)?
# {
#     public [ClassName](...) : base(...) { }
#     (Handle method...)
# }
class_pattern = re.compile(
    r'(?P<indent>\s*)public class (?P<class_name>\w+) : (?P<base_class>[\w\.:]+)<(?P<base_args>[^>]+)>(?:,\s*IRequestHandler<(?P<req_type>[^,>]+),\s*(?P<resp_type>[^>]+)>)?\s*\n'
    r'(?P=indent)\{\n'
    r'(?P<constructor_indent>\s*)public (?P=class_name)\((?P<cons_params>[^\)]*)\) : base\((?P<cons_args>[^\)]*)\) \{ \}\s*\n+'
    r'(?:\s*public ValueTask<(?P<resp_type2>[^>]+)> Handle\((?P<req_type2>[^ ]+) request, CancellationToken cancellationToken\)\s*\n'
    r'\s*\{\s*\n'
    r'\s*return base\.Handle\(request, cancellationToken\);\s*\n'
    r'\s*\}\s*\n)?'
    r'(?P=indent)\}',
    re.MULTILINE
)

last_pos = 0
new_content = ""
deleted_count = 0
cleaned_count = 0

for match in class_pattern.finditer(content):
    start, end = match.span()
    new_content += content[last_pos:start]
    
    class_name = match.group('class_name')
    base_class = match.group('base_class')
    base_args = match.group('base_args')
    req_type = match.group('req_type')
    
    req_short = req_type.split('.')[-1] if req_type else ""
    
    # Infer request type from class name
    implied_req = class_name
    if implied_req.endswith("Handler"):
        implied_req = implied_req[:-len("Handler")]
        
    is_custom = (req_short in custom_requests) or (implied_req in custom_requests) or (class_name in custom_requests)
    
    if is_custom:
        deleted_count += 1
        print(f"Deleting handler for hand-written request: {class_name}")
    else:
        cleaned_count += 1
        indent = match.group('indent')
        cons_params = match.group('cons_params')
        cons_args = match.group('cons_args')
        
        # Cleaned class definition (no explicit IRequestHandler interface, no Handle method override)
        cleaned_class = (
            f"{indent}public class {class_name} : {base_class}<{base_args}>\n"
            f"{indent}{{\n"
            f"{indent}    public {class_name}({cons_params}) : base({cons_args}) {{ }}\n"
            f"{indent}}}"
        )
        new_content += cleaned_class
        
    last_pos = end

new_content += content[last_pos:]

with open(file_path, 'w', encoding='utf-8') as f:
    f.write(new_content)

print(f"Completed! Deleted: {deleted_count}, Cleaned: {cleaned_count}")
