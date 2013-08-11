using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetadataService.Model;
using UpdateControls.Fields;

namespace PLSQLTemplates.SelectionModels
{
    public class MethodSelectionModel
    {
        private Independent<MetadataService.Model.Method> _selectedMethod = new Independent<Method>();


        public Method SelectedMethod
        {
            get { return _selectedMethod; }
            set { _selectedMethod.Value = value; }
        }

    }
}
