using System;
using System.Collections.Generic;
using System.Linq;

using MongoDB.Driver;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;

using NUnit.Framework;
using FluentAssertions;
using Moq;

using Booth.Common;
using Booth.EventStore;
using Booth.EventStore.MongoDB;

namespace Booth.EventStore.MongoDB.Test
{
    class EventStoreTests
    {
        [TestCase]
        public void IncludesNeededConventions()
        {
            var database = Mock.Of<IMongoDatabase>();
            var eventStore = new MongodbEventStore(database);

            var conventionPack = ConventionRegistry.Lookup(typeof(Event));

            conventionPack.Conventions.Should().Contain(x => x.Name == "IgnoreExtraElements");
        }

        [TestCase]
        public void DateSerializerRegistered()
        {
            var database = Mock.Of<IMongoDatabase>();
            var eventStore = new MongodbEventStore(database);

            var serializer = BsonSerializer.LookupSerializer<Date>();

            serializer.Should().BeOfType<DateSerializer>();
        }

        [TestCase]
        public void EventTypesMapped()
        {
            var database = Mock.Of<IMongoDatabase>();
            var eventStore = new MongodbEventStore(database);

            var isRegisterd = BsonClassMap.IsClassMapRegistered(typeof(TestEvent));

            isRegisterd.Should().BeTrue();
        }

    }

    class TestEvent: Event
    {
        public TestEvent(Guid id, int version)
            :base(id, version)
        {

        }
    }
}
