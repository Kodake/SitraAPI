import re
import os

file_path = r"c:\Users\rflorentino\Desktop\New folder\src\SitraAPI\SITRA.Core\Mediator\Handlers\GeneratedConcreteHandlers.cs"

with open(file_path, 'r', encoding='utf-8') as f:
    content = f.read()

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

def parse_class_block(class_text):
    # E.g. public class ClassName : BaseClass<Args> or public class ClassName : BaseClass<Args>, IRequestHandler<Req, Resp>
    first_line_match = re.match(
        r'^\s*public class (?P<class_name>\w+) : (?P<base_class>\w+)<(?P<base_args>.*)>',
        class_text
    )
    if not first_line_match:
        return None
        
    class_name = first_line_match.group('class_name')
    base_class = first_line_match.group('base_class')
    base_args_raw = first_line_match.group('base_args')
    
    # Extract base_args correctly by counting angle brackets < >.
    angle_count = 1  # we already consumed the first < after base_class
    base_args = ""
    for i, c in enumerate(base_args_raw):
        if c == '<':
            angle_count += 1
        elif c == '>':
            angle_count -= 1
        
        if angle_count == 0:
            base_args = base_args_raw[:i]
            break
            
    # Extract the constructor
    constructor_match = re.search(
        r'public ' + class_name + r'\((?P<params>[^\)]*)\) : base\((?P<args>[^\)]*)\)\s*\{\s*\}',
        class_text
    )
    if not constructor_match:
        return None
        
    cons_params = constructor_match.group('params').strip()
    cons_args = constructor_match.group('args').strip()
    
    return {
        'class_name': class_name,
        'base_class': base_class,
        'base_args': base_args.strip(),
        'cons_params': cons_params,
        'cons_args': cons_args
    }

# Find where SITRA.Core.Mediator.Handlers namespace block starts
namespace_match = re.search(r'namespace SITRA.Core.Mediator.Handlers\s*\n\s*\{', content)
if not namespace_match:
    raise Exception("Namespace block not found")
    
header = content[:namespace_match.end()]
footer = "}"

# Find all class declarations
class_pos = []
idx = namespace_match.end()
while True:
    pos = content.find("public class ", idx)
    if pos == -1:
        break
    class_pos.append(pos)
    idx = pos + 1

new_classes_code = []
deleted_count = 0
rebuilt_count = 0
skipped_count = 0

for i, start_pos in enumerate(class_pos):
    # Find matching closing brace for this class
    brace_start = content.find("{", start_pos)
    brace_count = 1
    brace_end = brace_start + 1
    while brace_count > 0 and brace_end < len(content):
        c = content[brace_end]
        if c == '{':
            brace_count += 1
        elif c == '}':
            brace_count -= 1
        brace_end += 1
        
    class_text = content[start_pos:brace_end]
    
    parsed = parse_class_block(class_text)
    if not parsed:
        print(f"Failed to parse class: {class_text[:100]}...")
        new_classes_code.append(class_text)
        skipped_count += 1
        continue
        
    class_name = parsed['class_name']
    base_class = parsed['base_class']
    base_args = parsed['base_args']
    cons_params = parsed['cons_params']
    cons_args = parsed['cons_args']
    
    implied_req = class_name
    if implied_req.endswith("Handler"):
        implied_req = implied_req[:-len("Handler")]
        
    # Check if this class is custom and should be deleted
    if (implied_req in custom_requests) or (class_name in custom_requests):
        deleted_count += 1
        print(f"Deleting duplicate handler: {class_name}")
        continue
        
    is_generic_handler = base_class in {"CreateCommandHandler", "UpdateCommandHandler", "ProcessCommandHandler", "ProccessCommandHandler"}
    
    if not is_generic_handler:
        # Format (1): Clean (No explicit IRequestHandler interface, no Handle method override)
        class_code = (
            f"    public class {class_name} : {base_class}<{base_args}>\n"
            f"    {{\n"
            f"        public {class_name}({cons_params}) : base({cons_args}) {{ }}\n"
            f"    }}"
        )
    else:
        # Format (2): Explicit IRequestHandler, Handle method
        req_type = implied_req
        if base_class == "UpdateCommandHandler":
            resp_type = "bool"
        else:
            args = [a.strip() for a in base_args.split(',')]
            resp_type = args[1] if len(args) > 1 else "object"
            
        class_code = (
            f"    public class {class_name} : {base_class}<{base_args}>, IRequestHandler<{req_type}, {resp_type}>\n"
            f"    {{\n"
            f"        public {class_name}({cons_params}) : base({cons_args}) {{ }}\n\n"
            f"        public ValueTask<{resp_type}> Handle({req_type} request, CancellationToken cancellationToken)\n"
            f"        {{\n"
            f"            return base.Handle(request, cancellationToken);\n"
            f"        }}\n"
            f"    }}"
        )
        rebuilt_count += 1
        
    new_classes_code.append(class_code)

# Assemble final code
final_code = header + "\n" + "\n\n".join(new_classes_code) + "\n" + footer + "\n"

with open(file_path, 'w', encoding='utf-8') as f:
    f.write(final_code)
    
print(f"Completed! Deleted: {deleted_count}, Rebuilt format(2): {rebuilt_count}, Cleaned format(1): {len(new_classes_code) - rebuilt_count}, Skipped: {skipped_count}")
