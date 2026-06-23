using BusinessService.DTOs;

namespace BusinessService.Interfaces
{
    /// <summary>
    /// In-memory audit log of balance settlements (per the spec, persisted in memory for the demo).
    /// Registered as a singleton so entries survive across requests for the app's lifetime.
    /// </summary>
    public interface ISettlementStore
    {
        void Add(SettlementHistoryDto entry);
        IReadOnlyCollection<SettlementHistoryDto> GetAll();
    }
}
