using System.Data;
using System.Linq;
using d60.Cirqus.MsSql.Views;
using NUnit.Framework;

namespace d60.Cirqus.Tests.MsSql
{
    [TestFixture]
    public class TestSchemaHelper
    {
        public class SomeView
        {
            public string JustString { get; set; }

            [Column]
            public string EmptyColumnAttribute { get; set; }

            [Column(size:"32")]
            public string FixedLengthString { get; set; }

            [Column(name:"TheColumn", size:"111")]
            public string Column2 { get; set; }
        }

        [Test]
        public void StringPropSize_IsMaxByDefault()
        {
            var schema = SchemaHelper.GetSchema<SomeView>();

            var prop = schema.Single(x => x.ColumnName == "JustString");
            Assert.AreEqual("max", prop.Size);
        }

        [Test]
        public void EmptyColumnAttribute_DoesnotCrash()
        {
            var schema = SchemaHelper.GetSchema<SomeView>();

            var prop = schema.SingleOrDefault(x => x.ColumnName == "EmptyColumnAttribute");
            Assert.IsNotNull(prop);
        }

        [Test]
        public void StringPropSize_CanBeSetViaColumnAttribute()
        {
            var schema = SchemaHelper.GetSchema<SomeView>();

            var prop = schema.Single(x => x.ColumnName == "FixedLengthString");
            Assert.AreEqual("32", prop.Size);
        }

        [Test]
        public void ColumnAttribute_BothParams()
        {
            var schema = SchemaHelper.GetSchema<SomeView>();

            var prop = schema.Single(x => x.ColumnName == "TheColumn");
            Assert.IsNotNull(prop);
            Assert.AreEqual("111", prop.Size);
        }

        [Test]
        public void PropMatches_TakesSizeIntoAccount()
        {
            var prop1 = new Prop { ColumnName = "Col", SqlDbType = SqlDbType.VarChar, Size = "100" };
            var prop2 = new Prop { ColumnName = "Col", SqlDbType = SqlDbType.VarChar, Size = "101" };

            var matches = prop1.Matches(prop2);

            Assert.IsFalse(matches);
        }
    }
}