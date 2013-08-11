using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetadataService;
using MetadataService.Model;
using MetadataService.Database;
using System.Data.Entity;
using System.Diagnostics;
using Common;
using System.Windows;

namespace MetadataService
{
    public class Repository
    {
        public static DatabaseContext _DBContext;

        public void Load()
        {
            var metadataService = new DataLoader("Data Source=ece;User Id=EC_LDBA;Password=LDBA;", @"Data Source=|DataDirectory|\LocalDatabase.sdf");
            metadataService.load();
        }


        public Repository()
        {
            if (_DBContext == null) _DBContext = new DatabaseContext();
            //_DBContext.Database.Log = Console.Write;
            var count = _DBContext.Packages.CountAsync();


        }

        public async Task<SearchResult> findMethods(String searchTerm)
        {
             var str = searchTerm.ToLower();
             var strLen = str.Length;

             List<Method> list;

            if (str.EndsWith("(")) {
                list = await _DBContext.Methods.Where(a => (a.Name+"(").Contains(str)).Include(p => p.Package).Select(a => a).ToListAsync();
             }
             else
             {
                 list = await _DBContext.Methods.Where(a => a.Name.Contains(str) || a.Package.Name.Contains(str)).Include(p => p.Package).Select(a => a).ToListAsync();
             }

             return new SearchResult {SearchTerm = searchTerm, Matches = list };
        }

        public Method completeMethod(Method method)
        {
            Method result = method;

            if (method != null)
            {
                result = _DBContext.Methods.Where(a => a.ObjectId == method.ObjectId &&
                                                     a.SubprogramId == method.SubprogramId &&
                                                     a.Overload == method.Overload)
                   .Include(u => u.Package)
                   .Include(a => a.Arguments)
                   .FirstOrDefault();

            }
            return result;
        }

        public PlSqlWrapper getPLSQLWrapper(Method method)
        {
            return new PlSqlWrapper(completeMethod(method));

        }

    }
}
