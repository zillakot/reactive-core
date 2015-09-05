using System;
using System.Reflection;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using UnityEngine;
using UniRx;

namespace UnityTest
{
    [TestFixture]
    [Category("Constant Tests")]
    public class ConstantTests
    {
        [Test]
        public void LayersTest(){
            var fields = typeof(Layers).GetFields();
            foreach (var f in fields){
                var value = LayerMask.NameToLayer((string)f.GetValue(null));
                Assert.GreaterOrEqual(value,0);
            }
            Assert.Pass();
        }
    }
}