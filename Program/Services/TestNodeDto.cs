using System;
using System.Collections.Generic;

namespace FridayAfternoon.WpfPlay.Program.Services
{
    public sealed class TestNodeDto : IEquatable<TestNodeDto>
    {
        public TestNodeDto(int id, string name, int parentId)
        {
            Id = id;
            Name = name;
            ParentId = parentId;
        }

        public int Id { get; }

        public string Name { get; }

        public int ParentId { get; }

        public override bool Equals(object obj) => obj is TestNodeDto dto && Equals(dto);

        public bool Equals(TestNodeDto other) => Id == other.Id && Name == other.Name && ParentId == other.ParentId;

        public override int GetHashCode() => Id;
    }
}