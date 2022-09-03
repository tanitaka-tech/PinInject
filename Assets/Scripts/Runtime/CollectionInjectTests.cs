using System.Collections;
using System.Collections.Generic;
using Cathei.PinInject;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

[TestFixture]
public class CollectionInjectTests
{
    public class GlboalInjectContext : IInjectContext
    {
        public void Configure(IInjectBinder binder)
        {
            binder.Bind<IBindWithInterface>(new BindWithInterface(8));
            binder.Bind(new BindWithNew());
        }
    }

    public class InjectableChild
    {
        [Inject]
        public IBindWithInterface bindedInterf;

        [Inject]
        public string bindedStr;

        public int id;

        public InjectableChild(int id)
        {
            this.id = id;
        }
    }

    public class InjectableParent : IInjectContext
    {
        [Resolve]
        public InjectCollection<InjectableChild> list;

        private string _value;

        public InjectableParent(string value)
        {
            list = new InjectCollection<InjectableChild>();
            _value = value;
        }

        public void Configure(IInjectBinder binder)
        {
            binder.Bind(_value);
        }
    }

    public class InjectableKeyedParent : InjectKeyedCollection<int, InjectableChild>, IInjectContext
    {
        private string _value;

        public InjectableKeyedParent(string value)
        {
            _value = value;
        }

        public void Configure(IInjectBinder binder)
        {
            binder.Bind(_value);
        }

        protected override int GetKeyForItem(InjectableChild item)
        {
            return item.id;
        }
    }

    [SetUp]
    public void Setup()
    {
        Pin.Reset();
        Pin.AddGlobalContext<GlboalInjectContext>();
    }

    [Test]
    public void CollectionInject_TestCollection()
    {
        var child1 = new InjectableChild(1);
        var child2 = new InjectableChild(2);
        var child3 = new InjectableChild(3);
        var child4 = new InjectableChild(4);

        string str = "collection test";
        var parent = new InjectableParent(str);

        parent.list.Add(child1);
        parent.list.Add(child2);

        Pin.Inject(parent);

        parent.list.Add(child3);
        parent.list.Add(child4);

        Assert.AreEqual(4, parent.list.Count);

        Assert.AreSame(child1, parent.list[0]);
        Assert.AreSame(child3, parent.list[2]);

        Assert.AreSame(str, child1.bindedStr);
        Assert.AreSame(str, child3.bindedStr);

        Assert.AreEqual(8, child1.bindedInterf.Value);
        Assert.AreEqual(child2.bindedInterf, child4.bindedInterf);
    }

    [Test]
    public void CollectionInject_TestKeyedCollection()
    {
        var child1 = new InjectableChild(9999);
        var child2 = new InjectableChild(7777);
        var child3 = new InjectableChild(3333);
        var child4 = new InjectableChild(4444);

        string str = "keyed collection test";
        var parent = new InjectableKeyedParent(str);

        parent.Add(child1);
        parent.Add(child2);

        Pin.Inject(parent);

        parent.Add(child3);
        parent.Add(child4);

        Assert.AreEqual(4, parent.Count);

        Assert.AreSame(child1, parent[9999]);
        Assert.AreSame(child3, parent[3333]);

        Assert.AreSame(str, child1.bindedStr);
        Assert.AreSame(str, child3.bindedStr);

        Assert.AreEqual(8, child1.bindedInterf.Value);
        Assert.AreEqual(child2.bindedInterf, child4.bindedInterf);
    }
}
