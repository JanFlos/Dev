using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetadataService.Model;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeFirstTest
{



    [TestClass]
    public class Testclass
    {

        [TestMethod]
        public void Test01()
        {
            var i = new MetadataService.Repository();
            var methods = i.findMethods("prc_loadqueue");
            methods.Wait();
            Debug.WriteLine(i.getPLSQLWrapper(methods.Result.Matches.FirstOrDefault()).ToString());
        }
    }
}
