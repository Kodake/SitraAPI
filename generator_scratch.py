import os
import re

commands_dir = r"c:\Users\rflorentino\Desktop\New folder\src\SitraAPI\SITRA.Core\Mediator\Commands"
queries_dir = r"c:\Users\rflorentino\Desktop\New folder\src\SitraAPI\SITRA.Core\Mediator\Queries"
output_file = r"c:\Users\rflorentino\Desktop\New folder\src\SitraAPI\SITRA.Core\Mediator\Handlers\GeneratedConcreteHandlers.cs"

# Custom handlers that are already hand-written and should not be auto-generated
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

# Regex to find class declarations
class_decl_pattern = re.compile(
    r'public\s+(?:abstract\s+)?class\s+(?P<class_name>\w+)(?:<[^>]+>)?(?:\([^:]*\))?\s*:\s*(?P<base_list>[^\{\n;]+)',
    re.MULTILINE
)

# Regex to parse the first base type
base_type_pattern = re.compile(
    r'^(?P<base_class>[\w\.]+)(?:<(?P<base_args>[^>]+)>)?(?:\([^)]*\))?'
)

requests = {}

def scan_directory(directory):
    for root, dirs, files in os.walk(directory):
        for file in files:
            if file.endswith('.cs'):
                fpath = os.path.join(root, file)
                with open(fpath, 'r', encoding='utf-8') as f:
                    content = f.read()
                
                # Remove comments
                content_clean = re.sub(r'//.*', '', content)
                content_clean = re.sub(r'/\*.*?\*/', '', content_clean, flags=re.DOTALL)
                
                for match in class_decl_pattern.finditer(content_clean):
                    class_name = match.group('class_name')
                    base_list = match.group('base_list').strip()
                    
                    # Split base list by comma to get the first base type
                    # But be careful with commas inside angle brackets <...> or parentheses (...)
                    # Since we only need the first base type, we can split up to the first outer comma.
                    first_base_type_raw = ""
                    bracket_level = 0
                    paren_level = 0
                    for char in base_list:
                        if char == '<':
                            bracket_level += 1
                        elif char == '>':
                            bracket_level -= 1
                        elif char == '(':
                            paren_level += 1
                        elif char == ')':
                            paren_level -= 1
                        elif char == ',' and bracket_level == 0 and paren_level == 0:
                            break
                        first_base_type_raw += char
                    
                    first_base_type_raw = first_base_type_raw.strip()
                    
                    type_match = base_type_pattern.match(first_base_type_raw)
                    if type_match:
                        base_class = type_match.group('base_class').split('.')[-1]
                        base_args = type_match.group('base_args')
                        base_args = base_args.strip() if base_args else ''
                        
                        requests[class_name] = {
                            'base_class': base_class,
                            'base_args': base_args
                        }

scan_directory(commands_dir)
scan_directory(queries_dir)

print(f"Scanned {len(requests)} total requests.")

# Now, generate handlers code
handlers_code = []

# Order request keys for consistent file output
for req_name in sorted(requests.keys()):
    if req_name in custom_requests:
        continue
        
    info = requests[req_name]
    base_class = info['base_class']
    base_args = info['base_args']
    
    handler_name = f"{req_name}Handler"
    
    # Map command/query base to handler base
    if base_class == "CreateCommand":
        # Format 2 (Explicit IRequestHandler)
        # base_args contains "TEntity, TDto"
        args = [a.strip() for a in base_args.split(',')]
        dto_type = args[1] if len(args) > 1 else "object"
        
        code = (
            f"    public class {handler_name} : CreateCommandHandler<{base_args}>, IRequestHandler<{req_name}, {dto_type}>\n"
            f"    {{\n"
            f"        public {handler_name}(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, IMapper mapper) : base(unitOfWork, outputCacheStore, mapper) {{ }}\n\n"
            f"        public ValueTask<{dto_type}> Handle({req_name} request, CancellationToken cancellationToken)\n"
            f"        {{\n"
            f"            return base.Handle(request, cancellationToken);\n"
            f"        }}\n"
            f"    }}"
        )
        handlers_code.append(code)
        
    elif base_class == "UpdateCommand":
        # Format 2 (Explicit IRequestHandler)
        # base_args contains "TEntity, TDto"
        code = (
            f"    public class {handler_name} : UpdateCommandHandler<{base_args}>, IRequestHandler<{req_name}, bool>\n"
            f"    {{\n"
            f"        public {handler_name}(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, IMapper mapper) : base(unitOfWork, outputCacheStore, mapper) {{ }}\n\n"
            f"        public ValueTask<bool> Handle({req_name} request, CancellationToken cancellationToken)\n"
            f"        {{\n"
            f"            return base.Handle(request, cancellationToken);\n"
            f"        }}\n"
            f"    }}"
        )
        handlers_code.append(code)
        
    elif base_class == "ProcessCommand":
        # Format 2 (Explicit IRequestHandler)
        # base_args contains "TEntity, TDto"
        args = [a.strip() for a in base_args.split(',')]
        dto_type = args[1] if len(args) > 1 else "object"
        
        code = (
            f"    public class {handler_name} : ProcessCommandHandler<{base_args}>, IRequestHandler<{req_name}, {dto_type}>\n"
            f"    {{\n"
            f"        public {handler_name}(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, IMapper mapper) : base(unitOfWork, outputCacheStore, mapper) {{ }}\n\n"
            f"        public ValueTask<{dto_type}> Handle({req_name} request, CancellationToken cancellationToken)\n"
            f"        {{\n"
            f"            return base.Handle(request, cancellationToken);\n"
            f"        }}\n"
            f"    }}"
        )
        handlers_code.append(code)
        
    elif base_class == "ToggleCommand":
        # Format 1 (Clean)
        code = (
            f"    public class {handler_name} : ToggleCommandHandler<{req_name}>\n"
            f"    {{\n"
            f"        public {handler_name}(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore) : base(unitOfWork, outputCacheStore) {{ }}\n"
            f"    }}"
        )
        handlers_code.append(code)
        
    elif base_class == "DeleteCommand":
        # Format 1 (Clean)
        code = (
            f"    public class {handler_name} : DeleteCommandHandler<{req_name}>\n"
            f"    {{\n"
            f"        public {handler_name}(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore) : base(unitOfWork, outputCacheStore) {{ }}\n"
            f"    }}"
        )
        handlers_code.append(code)
        
    elif base_class == "AuthCommand":
        # Format 1 (Clean)
        # base_args is TResult
        code = (
            f"    public class {handler_name} : AuthCommandHandler<{req_name}, {base_args}>\n"
            f"    {{\n"
            f"        public {handler_name}(IUnitOfWork unitOfWork, IOutputCacheStore outputCacheStore, IMapper mapper) : base(unitOfWork, outputCacheStore, mapper) {{ }}\n"
            f"    }}"
        )
        handlers_code.append(code)
        
    elif base_class == "AuthQuery":
        # Format 1 (Clean)
        # base_args is TResult
        code = (
            f"    public class {handler_name} : AuthQueryHandler<{req_name}, {base_args}>\n"
            f"    {{\n"
            f"        public {handler_name}(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper) {{ }}\n"
            f"    }}"
        )
        handlers_code.append(code)
        
    elif base_class in {"GetSelectsQuery", "GetByQuery", "GetByIdQuery", "GetAllQuery", "GetAllByIdQuery", "IQuery", "Query"}:
        # Format 1 (Clean)
        # Determine the result type
        if base_class in {"GetSelectsQuery", "GetAllQuery"}:
            result_type = f"List<{base_args}>"
        elif base_class == "GetAllByIdQuery":
            result_type = f"IEnumerable<{base_args}>"
        else:
            result_type = base_args
            
        code = (
            f"    public class {handler_name} : QueryHandler<{req_name}, {result_type}>\n"
            f"    {{\n"
            f"        public {handler_name}(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper) {{ }}\n"
            f"    }}"
        )
        handlers_code.append(code)
        
    else:
        print(f"Skipped unknown base class {base_class} for request {req_name}")

header = """// <auto-generated />
using AutoMapper;
using Mediator;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SITRA.Core.DTOs.Accounts.PayableReceipts;
using SITRA.Core.DTOs.Accounts.Payables;
using SITRA.Core.DTOs.Accounts.PaymentReceipts;
using SITRA.Core.DTOs.Accounts.Receivables;
using SITRA.Core.DTOs.AdministrativeTools.Deadlines;
using SITRA.Core.DTOs.AdministrativeTools.FiscalSupports;
using SITRA.Core.DTOs.AdministrativeTools.Roles;
using SITRA.Core.DTOs.Appointments.Appointments;
using SITRA.Core.DTOs.Auths;
using SITRA.Core.DTOs.CashBoxes;
using SITRA.Core.DTOs.Contacts.Contacts;
using SITRA.Core.DTOs.DGII;
using SITRA.Core.DTOs.Dashboards;
using SITRA.Core.DTOs.Finances.Currencies;
using SITRA.Core.DTOs.Finances.Expenses;
using SITRA.Core.DTOs.Finances.Taxes;
using SITRA.Core.DTOs.Invoices.LogInvoices;
using SITRA.Core.DTOs.Invoices.PurchaseInvoices;
using SITRA.Core.DTOs.Invoices.Quotations;
using SITRA.Core.DTOs.Invoices.SaleInvoices;
using SITRA.Core.DTOs.Invoices.SpotPayables;
using SITRA.Core.DTOs.Invoices.SpotPayments;
using SITRA.Core.DTOs.Products.Brands;
using SITRA.Core.DTOs.Products.Categories;
using SITRA.Core.DTOs.Products.Combos;
using SITRA.Core.DTOs.Products.Items;
using SITRA.Core.DTOs.Products.Models;
using SITRA.Core.DTOs.Products.PricingItems;
using SITRA.Core.DTOs.Products.Pricings;
using SITRA.Core.DTOs.Products.Products;
using SITRA.Core.DTOs.Products.Services;
using SITRA.Core.DTOs.Products.SubCategories;
using SITRA.Core.DTOs.Warehouses.Stocks;
using SITRA.Core.DTOs.Warehouses.Transfers;
using SITRA.Core.DTOs.Warehouses.Warehouses;
using SITRA.Core.DTOs.Wizard;
using SITRA.Core.DTOs;
using SITRA.Core.Entities;
using SITRA.Core.Entities.AdministrativeTools;
using SITRA.Core.Entities.Products;
using SITRA.Core.Entities.Warehouses;
using SITRA.Core.Entities.Invoices;
using SITRA.Core.Entities.Finances;
using SITRA.Core.Entities.CashBoxes;
using SITRA.Core.Entities.Contacts;
using SITRA.Core.Entities.Appointments;
using SITRA.Core.Entities.Tenants;
using SITRA.Core.Interfaces.Services.AdministrativeTools;
using SITRA.Core.Interfaces.Services.Auths;
using SITRA.Core.Interfaces.Services.CashBoxes;
using SITRA.Core.Interfaces.Services.Dashboards;
using SITRA.Core.Interfaces.Services.Finances;
using SITRA.Core.Interfaces.Services.Invoices;
using SITRA.Core.Interfaces.Services.Messaging;
using SITRA.Core.Interfaces.Services.Products;
using SITRA.Core.Interfaces.Services.Storage;
using SITRA.Core.Interfaces.Services;
using SITRA.Core.Interfaces;
using SITRA.Core.Mediator.Commands.Accounts.Payables;
using SITRA.Core.Mediator.Commands.Accounts.Receivables;
using SITRA.Core.Mediator.Commands.AdministrativeTools.Deadlines;
using SITRA.Core.Mediator.Commands.AdministrativeTools.FiscalSupports;
using SITRA.Core.Mediator.Commands.AdministrativeTools.Roles;
using SITRA.Core.Mediator.Commands.Appointments.Appointments;
using SITRA.Core.Mediator.Commands.Auths;
using SITRA.Core.Mediator.Commands.CashBoxes;
using SITRA.Core.Mediator.Commands.Contacts;
using SITRA.Core.Mediator.Commands.Finances.Currencies;
using SITRA.Core.Mediator.Commands.Finances.Expenses;
using SITRA.Core.Mediator.Commands.Finances.Taxes;
using SITRA.Core.Mediator.Commands.Invoices.PurchaseInvoices;
using SITRA.Core.Mediator.Commands.Invoices.Quotations;
using SITRA.Core.Mediator.Commands.Invoices.SaleInvoices;
using SITRA.Core.Mediator.Commands.Products.Brands;
using SITRA.Core.Mediator.Commands.Products.Categories;
using SITRA.Core.Mediator.Commands.Products.Combos;
using SITRA.Core.Mediator.Commands.Products.Models;
using SITRA.Core.Mediator.Commands.Products.PricingItems;
using SITRA.Core.Mediator.Commands.Products.Pricings;
using SITRA.Core.Mediator.Commands.Products.Products;
using SITRA.Core.Mediator.Commands.Products.Services;
using SITRA.Core.Mediator.Commands.Products.SubCategories;
using SITRA.Core.Mediator.Commands.Warehouses.Stocks;
using SITRA.Core.Mediator.Commands.Warehouses.Transfers;
using SITRA.Core.Mediator.Commands.Warehouses.Warehouses;
using SITRA.Core.Mediator.Commands.Wizard;
using SITRA.Core.Mediator.Commands;
using SITRA.Core.Mediator.Handlers.AdministrativeTools.Roles;
using SITRA.Core.Mediator.Queries.Accounts.Payables;
using SITRA.Core.Mediator.Queries.Accounts.Receivables;
using SITRA.Core.Mediator.Queries.AdministrativeTools.Deadlines;
using SITRA.Core.Mediator.Queries.AdministrativeTools.FiscalSupports;
using SITRA.Core.Mediator.Queries.AdministrativeTools.Roles;
using SITRA.Core.Mediator.Queries.Appointments.Appointments;
using SITRA.Core.Mediator.Queries.Auths;
using SITRA.Core.Mediator.Queries.CashBoxes;
using SITRA.Core.Mediator.Queries.Contacts;
using SITRA.Core.Mediator.Queries.Dashboards;
using SITRA.Core.Mediator.Queries.Finances.Currencies;
using SITRA.Core.Mediator.Queries.Finances.Expenses;
using SITRA.Core.Mediator.Queries.Finances.Taxes;
using SITRA.Core.Mediator.Queries.Invoices.LogInvoices;
using SITRA.Core.Mediator.Queries.Invoices.PurchaseInvoices;
using SITRA.Core.Mediator.Queries.Invoices.Quotations;
using SITRA.Core.Mediator.Queries.Invoices.SaleInvoices;
using SITRA.Core.Mediator.Queries.Invoices.SpotPayables;
using SITRA.Core.Mediator.Queries.Invoices.SpotPayments;
using SITRA.Core.Mediator.Queries.Products.Brands;
using SITRA.Core.Mediator.Queries.Products.Categories;
using SITRA.Core.Mediator.Queries.Products.Combos;
using SITRA.Core.Mediator.Queries.Products.Items;
using SITRA.Core.Mediator.Queries.Products.Models;
using SITRA.Core.Mediator.Queries.Products.PricingItems;
using SITRA.Core.Mediator.Queries.Products.Pricings;
using SITRA.Core.Mediator.Queries.Products.Products;
using SITRA.Core.Mediator.Queries.Products.Services;
using SITRA.Core.Mediator.Queries.Products.SubCategories;
using SITRA.Core.Mediator.Queries.Receivables;
using SITRA.Core.Mediator.Queries.Tenants;
using SITRA.Core.Mediator.Queries.Warehouses.Stocks;
using SITRA.Core.Mediator.Queries.Warehouses.Transfers;
using SITRA.Core.Mediator.Queries.Warehouses.Warehouses;
using SITRA.Core.Mediator.Queries.Wizard;
using SITRA.Core.Mediator.Queries;
using SITRA.Core.Models.Dgii;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Xml.Linq;
using System;

namespace SITRA.Core.Mediator.Handlers
{"""

footer = "}"

final_code = header + "\n" + "\n\n".join(handlers_code) + "\n" + footer + "\n"

with open(output_file, 'w', encoding='utf-8') as f:
    f.write(final_code)

print(f"Generated {len(handlers_code)} concrete handlers successfully!")
