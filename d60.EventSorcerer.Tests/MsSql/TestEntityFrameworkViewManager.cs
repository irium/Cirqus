﻿using System;
using System.Linq;
using d60.EventSorcerer.Aggregates;
using d60.EventSorcerer.EntityFramework;
using d60.EventSorcerer.Events;
using d60.EventSorcerer.Views.Basic;
using d60.EventSorcerer.Views.Basic.Locators;
using NUnit.Framework;
using TestContext = d60.EventSorcerer.TestHelpers.TestContext;

namespace d60.EventSorcerer.Tests.MsSql
{
    [TestFixture, Category(TestCategories.MsSql)]
    public class TestEntityFrameworkViewManager : FixtureBase
    {
        TestContext _context;
        EntityFrameworkViewManager<View> _viewManager;

        protected override void DoSetUp()
        {
            TestSqlHelper.DropTable("__MigrationHistory");
            TestSqlHelper.DropTable("View");

            _viewManager = new EntityFrameworkViewManager<View>(TestSqlHelper.ConnectionString);
            _context = new TestContext().AddViewManager(_viewManager);
        }

        [Test]
        public void CanDoTheFunkyLinqyStuff()
        {
            // arrange
            var root1Id = Guid.NewGuid();
            var root2Id = Guid.NewGuid();
            var root3Id = Guid.NewGuid();
            var root4Id = Guid.NewGuid();
            var root5Id = Guid.NewGuid();
            _context.Get<Root>(root1Id).ThisIsTheNumber(1);
            _context.Get<Root>(root2Id).ThisIsTheNumber(2);
            _context.Get<Root>(root3Id).ThisIsTheNumber(43);
            _context.Get<Root>(root4Id).ThisIsTheNumber(43);
            _context.Get<Root>(root5Id).ThisIsTheNumber(43);
            _context.Commit();

            // act
            // assert
            using (var views = _viewManager.Linq())
            {
                Assert.That(views.Query().First(v => v.Id == root1Id.ToString()).SomeNumber, Is.EqualTo(1));                
                Assert.That(views.Query().First(v => v.Id == root2Id.ToString()).SomeNumber, Is.EqualTo(2));                
                
                Assert.That(views.Query().Count(v => v.SomeNumber == 43), Is.EqualTo(3));                
            }

        }

        public class Root : AggregateRoot, IEmit<Event>
        {
            public void ThisIsTheNumber(int number)
            {
                Emit(new Event { SomeNumber = number });
            }
            public void Apply(Event e)
            {
            }
        }

        public class Event : DomainEvent<Root>
        {
            public int SomeNumber { get; set; }
        }

        class View : IView<InstancePerAggregateRootLocator>, ISubscribeTo<Event>
        {
            public string Id { get; set; }
            public long LastGlobalSequenceNumber { get; set; }
            public int SomeNumber { get; set; }
            public void Handle(IViewContext context, Event domainEvent)
            {
                SomeNumber = domainEvent.SomeNumber;
            }
        }
    }
}