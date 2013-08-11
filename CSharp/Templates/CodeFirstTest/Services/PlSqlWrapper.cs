using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MetadataService.Model;
using Common;

namespace MetadataService
{
    public class PlSqlWrapper
    {
        private readonly Method _method;
        private String _parameterDeclr;
        private int _maxParamLen = -1;

        public PlSqlWrapper(Method method)
        {
            _method = method;
        }

        private static String _template;
        private string _parameterCall;

        public String Template
        {
            get
            {
                if (_template == null)
                {
                    var s = new StringBuilder();
                    s.AppendLine("/*");
                    s.AppendLine("* Wrapper für {PackageName}.{MethodName}{Overload}");
                    s.AppendLine("*/");
                    s.AppendLine("{MethodClause} {WrapperMethod}({ParameterDeclr}) {ReturnClause}{ReturnType}IS");
                    s.AppendLine("BEGIN");
                    s.AppendLine("  {ReturnClause}{PackageName}.{MethodName}( {ParameterCall});");
                    s.AppendLine("END;");

                    return s.ToString();

                }
                return _template;

            }
        }

        public Boolean IsFunction { get { return _method.Datatype != null; } }

        public override string ToString()
        {
            return this.ToString(Template);
        }

        public String PackageName { get { return _method.Package.Name; } }
        public String MethodName { get { return _method.Name.ToLower(); } }
        public String Overload { get { return _method.Overload != "0" ? "." + _method.Overload : ""; } }
        public String MethodClause { get { return IsFunction ? "FUNCTION" : "PROCEDURE"; } }
        public String WrapperMethod
        {
            get
            {

                if (IsFunction && !MethodName.StartsWith("fnc_"))
                {
                    return "fnc_" + MethodName;
                }

                if (!IsFunction && !MethodName.StartsWith("prc_"))
                {
                    return "prc_" + MethodName;
                }
                return MethodName;
            }
        }

        public int MaxParamLen
        {
            get { return _maxParamLen != -1 ? _maxParamLen : _method.Arguments.Where(a => a.Name != null).Max(p => p.Name.Length) + 1; }
        }

        public String ParameterDeclr
        {
            get
            {

                if (_parameterDeclr == null)
                {

                    var lpad = "";
                    var s = new StringBuilder();
                    String arg;

                    var hasDefault = _method.Arguments.Any(a => a.Defaulted != "N");
                    var maxDatatypeLen = 0;
                    if (hasDefault) maxDatatypeLen = _method.Arguments.Where(a => a.Name != null).Max(p => p.Datatype.Length);

                    foreach (var argument in _method.Arguments)
                    {
                        if (argument.Name != null)
                        {
                            arg = argument.Name.StartsWith("p_") ? argument.Name : "p_" + argument.Name;
                            s.AppendFormat("{2}{0} {3} {1}{4},\r\n",
                                arg.PadRight(MaxParamLen, ' '),
                                argument.Datatype.PadRight(maxDatatypeLen, ' '),
                                lpad,
                                argument.InOut.Replace('/', ' ').PadRight(6, ' '),
                                argument.Defaulted != "N" ? " := NULL" : ""
                             );

                            if (lpad == "") lpad = new String(' ', (MethodClause + " " + WrapperMethod + "(").Length);
                        }
                    }
                    _parameterDeclr = s.ToString();
                    _parameterDeclr = _parameterDeclr.TrimEnd('\n', '\r', ',');
                }
                return _parameterDeclr;
            }
        }
        public String ReturnClause { get { return IsFunction ? "RETURN " : ""; } }
        public String ReturnType { get { return IsFunction ? _method.Datatype + " " : ""; } }

        public String ParameterCall
        {
            get
            {

                if (_parameterCall == null)
                {
                    var s = new StringBuilder();
                    var lpad = "";
                    foreach (var argument in _method.Arguments)
                    {
                        if (argument.Name != null)
                        {
                            s.AppendFormat("{0}=> {1},\r\n", lpad + argument.Name.PadRight(MaxParamLen, ' '), argument.Name.StartsWith("p_") ? argument.Name : "p_" + argument.Name);

                            if (lpad == "") lpad = new String(' ', ("  " + ReturnClause + PackageName + "." + MethodName + "( ").Length);
                        }
                    }
                    _parameterCall = s.ToString().TrimEnd('\n', '\r', ',');
                }
                return _parameterCall;
            }

        }

    }
}

