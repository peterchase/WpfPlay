
using System;
using System.Collections.Generic;
using System.Linq;
using DynamicData;

namespace FridayAfternoon.WpfPlay.Program.Services
{
    public sealed class TestNodeService
    {
        private readonly SourceCache<TestNodeDto, int> mTestNodes = new SourceCache<TestNodeDto, int>(x => x.Id);

        public static TestNodeService CreateWithRandomContent()
        {
            var service = new TestNodeService();
            service.mTestNodes.AddOrUpdate(CreateTestNodes(25000));
            return service;
        }

        private TestNodeService() { }

        public IObservableCache<TestNodeDto, int> TestNodes => mTestNodes.AsObservableCache();

        public void Move(TestNodeDto dto, int newParentId)
        {
            // TODO go to service then update the cache

            //update the cache 
            mTestNodes.AddOrUpdate(new TestNodeDto(dto.Id, dto.Name, newParentId));
        }

        public void Remove(TestNodeDto dto)
        {
            // TODO go to service then updated the cache

            mTestNodes.Edit(updater =>
            {
                //assign new parent to the children of the removed test node
                var childrenToMove = updater.Items
                                    .Where(tn => tn.ParentId == dto.Id)
                                    .Select(dto => new TestNodeDto(dto.Id, dto.Name, dto.ParentId))
                                    .ToArray();

                updater.AddOrUpdate(childrenToMove);

                //get rid of the existing test node
                updater.Remove(dto.Id);
            });
        }

        private static IEnumerable<TestNodeDto> CreateTestNodes(int numberToLoad)
        {
            var random = new Random();

            return Enumerable.Range(1, numberToLoad)
                .Select(i =>
                {
                    var parent = i % 1000 == 0 ? 0 : random.Next(0, i);
                    return new TestNodeDto(i, $"Test {i}", parent);
                });
        }
    }
}