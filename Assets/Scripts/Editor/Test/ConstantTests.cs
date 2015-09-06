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
        public void TestLayer(){
            var layers = UnityEditorInternal.InternalEditorUtility.layers;
            var fields = typeof(Layers).GetFields();
            foreach (var f in fields){
                Assert.Contains(f.GetValue(null), layers);
            }
        }
        
        [Test]
        public void TestTags(){
            var tags = UnityEditorInternal.InternalEditorUtility.tags;
            var fields = typeof(Tags).GetFields();
            foreach (var f in fields){
                Assert.Contains(f.GetValue(null), tags);
            }
        }
    }
}