using System.Collections.Concurrent;
using BusinessService.DTOs;
using BusinessService.Interfaces;

namespace BusinessService.Services
{
    public class InMemorySettlementStore : ISettlementStore
    {
        private readonly ConcurrentQueue<SettlementHistoryDto> _entries = new();

        public void Add(SettlementHistoryDto entry) => _entries.Enqueue(entry);

        public IReadOnlyCollection<SettlementHistoryDto> GetAll() => _entries.ToArray();
    }
}
