using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetadataService;
using System.Diagnostics;

namespace RepositoryTest
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {

            var i = new MetadataService.Repository();
            var methods = i.findMethods("prc_loadqueue");
            methods.Wait();
            Debug.WriteLine(i.getPLSQLWrapper(methods.Result.Matches.FirstOrDefault()).ToString());
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
        }
    }
}
