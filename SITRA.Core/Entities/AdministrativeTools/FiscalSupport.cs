using SITRA.Core.Enums;
using SITRA.Core.Interfaces;

namespace SITRA.Core.Entities.AdministrativeTools
{
    public sealed class FiscalSupport : ITenant, IAuditableEntity
    {
        public Guid Id { get; set; }
        public DocumentType DocumentType { get; set; }
        public EcfType FiscalSupportType { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Symbol { get; private set; } = string.Empty;
        public long InitialSequence { get; set; }
        public long FinalSequence { get; set; }
        public long SpaceNumber { get; private set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsElectronic => (int)FiscalSupportType >= 31;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public Guid TenantId { get; set; }
        public void InitializeDefaults()
        {
            SpaceNumber = IsElectronic ? 10 : 8;
            Symbol = "0";
        }
        public void IncrementSequence()
        {
            if (InitialSequence >= FinalSequence)
                throw new InvalidOperationException("Sequence limit reached");

            InitialSequence++;
        }
        public string GetNextFiscalSupportNumber()
        {
            if (InitialSequence >= FinalSequence)
                return null;

            var fiscalSupportName =
                Enum.GetName(typeof(EcfType), FiscalSupportType) ?? "Unknown";

            char symbolChar = Symbol[0];
            int space = (int)SpaceNumber;

            long nextSequence = InitialSequence + 1;

            var sequence = nextSequence.ToString().PadLeft(space, symbolChar);

            return $"{fiscalSupportName}{sequence}";
        }
        public string GenerateFiscalSupportNumber()
        {
            var fiscalSupportName =
                Enum.GetName(typeof(EcfType), FiscalSupportType) ?? "Unknown";

            char symbolChar = Symbol[0];
            int space = (int)SpaceNumber;

            var sequence = InitialSequence.ToString().PadLeft(space, symbolChar);

            return $"{fiscalSupportName}{sequence}";
        }
    }
}
