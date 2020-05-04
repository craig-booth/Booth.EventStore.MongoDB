using System;

using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.IO;


using NUnit.Framework;
using FluentAssertions;
using Moq;

using Booth.Common;


namespace Booth.EventStore.MongoDB.Test
{
    class DateSerializerTests
    {
        [TestCase]
        public void Serialize()
        {
            var writer = Mock.Of<IBsonWriter>();

            var context = BsonSerializationContext.CreateRoot(writer, null);
            var serializer = new DateSerializer();
           
            var date = new Date(2019, 12, 01); 
            serializer.Serialize(context, date);

            Mock.Get(writer).Verify(x => x.WriteString("2019-12-01"));
        }

        [TestCase]
        public void DeSerializeValidDate()
        {
            var reader = Mock.Of<IBsonReader>();
            Mock.Get(reader).Setup(x => x.CurrentBsonType).Returns(BsonType.String);
            Mock.Get(reader).Setup(x => x.ReadString()).Returns("2019-12-01");
            var context = BsonDeserializationContext.CreateRoot(reader, null);

            var serializer = new DateSerializer();
            var date = serializer.Deserialize(context);

            date.Should().Be(new Date(2019, 12, 01));
        }

        [TestCase]
        public void DeSerializeNullValue()
        {
            var reader = Mock.Of<IBsonReader>();
            Mock.Get(reader).Setup(x => x.CurrentBsonType).Returns(BsonType.String);
            Mock.Get(reader).Setup(x => x.ReadString()).Returns<string>(null);
            var context = BsonDeserializationContext.CreateRoot(reader, null);

            var serializer = new DateSerializer();
            var date = serializer.Deserialize(context);

            date.Should().Be(Date.MinValue);
        }

        [TestCase]
        public void DeSerializeEmptyValue()
        {
            var reader = Mock.Of<IBsonReader>();
            Mock.Get(reader).Setup(x => x.CurrentBsonType).Returns(BsonType.String);
            Mock.Get(reader).Setup(x => x.ReadString()).Returns("");
            var context = BsonDeserializationContext.CreateRoot(reader, null);

            var serializer = new DateSerializer();
            var date = serializer.Deserialize(context);

            date.Should().Be(Date.MinValue);
        }

        [TestCase]
        public void DeSerializeInvalidDate()
        {
            var reader = Mock.Of<IBsonReader>();
            Mock.Get(reader).Setup(x => x.CurrentBsonType).Returns(BsonType.String);
            Mock.Get(reader).Setup(x => x.ReadString()).Returns("2019-12-32");
            var context = BsonDeserializationContext.CreateRoot(reader, null);

            var serializer = new DateSerializer();
            var date = serializer.Deserialize(context);

            date.Should().Be(Date.MinValue);
        }

        [TestCase]
        public void DeSerializeNumericType()
        {
            var reader = Mock.Of<IBsonReader>();
            Mock.Get(reader).Setup(x => x.CurrentBsonType).Returns(BsonType.Int32);
            var context = BsonDeserializationContext.CreateRoot(reader, null);

            var serializer = new DateSerializer();
            var date = serializer.Deserialize(context);

            date.Should().Be(Date.MinValue);
            Mock.Get(reader).Verify(x => x.SkipValue());
        }

        [TestCase]
        public void DeSerializeDateTimeType()
        {
            var testDate = new DateTimeOffset(2019, 12, 01, 8, 54, 26, TimeSpan.Zero);

            var reader = Mock.Of<IBsonReader>();
            Mock.Get(reader).Setup(x => x.CurrentBsonType).Returns(BsonType.DateTime);
            Mock.Get(reader).Setup(x => x.ReadDateTime()).Returns(testDate.ToUnixTimeMilliseconds);
            var context = BsonDeserializationContext.CreateRoot(reader, null);

            var serializer = new DateSerializer();
            var date = serializer.Deserialize(context);

            date.Should().Be(new Date(2019, 12, 01));
        }

        [TestCase]
        public void DeSerializeTimeStampType()
        {
            var testDate = new DateTimeOffset(2019, 12, 01, 8, 54, 26, TimeSpan.Zero);

            var reader = Mock.Of<IBsonReader>();
            Mock.Get(reader).Setup(x => x.CurrentBsonType).Returns(BsonType.Timestamp);
            Mock.Get(reader).Setup(x => x.ReadTimestamp()).Returns(testDate.ToUnixTimeMilliseconds);
            var context = BsonDeserializationContext.CreateRoot(reader, null);

            var serializer = new DateSerializer();
            var date = serializer.Deserialize(context);

            date.Should().Be(new Date(2019, 12, 01));
        }

    }
}
