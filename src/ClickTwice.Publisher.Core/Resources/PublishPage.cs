﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 14.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace ClickTwice.Publisher.Core.Resources
{
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\Users\ucrm4\Source\Repos\ClickTwice\src\ClickTwice.Publisher.Core\Resources\PublishPage.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "14.0.0.0")]
    public partial class PublishPage : PublishPageBase
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public virtual string TransformText()
        {
            this.Write("<HTML>\r\n<HEAD>\r\n    <TITLE>");
            
            #line 9 "C:\Users\ucrm4\Source\Repos\ClickTwice\src\ClickTwice.Publisher.Core\Resources\PublishPage.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Manifest.ApplicationName));
            
            #line default
            #line hidden
            this.Write("</TITLE>\r\n    <META HTTP-EQUIV=\"Content-Type\" CONTENT=\"text/html; charset=utf-8\" " +
                    "/>\r\n    <STYLE TYPE=\"text/css\">\r\n        <!--\r\n        BODY {\r\n            margi" +
                    "n-top: 20px;\r\n            margin-left: 20px;\r\n            margin-right: 20px;\r\n " +
                    "           color: #000000;\r\n            font-family: Tahoma;\r\n            backgr" +
                    "ound-color: white;\r\n        }\r\n\r\n        A:link {\r\n            font-weight: norm" +
                    "al;\r\n            color: #000066;\r\n            text-decoration: none;\r\n        }\r" +
                    "\n\r\n        A:visited {\r\n            font-weight: normal;\r\n            color: #00" +
                    "0066;\r\n            text-decoration: none;\r\n        }\r\n\r\n        A:active {\r\n    " +
                    "        font-weight: normal;\r\n            text-decoration: none;\r\n        }\r\n\r\n " +
                    "       A:hover {\r\n            font-weight: normal;\r\n            color: #FF6600;\r" +
                    "\n            text-decoration: none;\r\n        }\r\n\r\n        P {\r\n            margi" +
                    "n-top: 0px;\r\n            margin-bottom: 12px;\r\n            color: #000000;\r\n    " +
                    "        font-family: Tahoma;\r\n        }\r\n\r\n        PRE {\r\n            border-rig" +
                    "ht: #f0f0e0 1px solid;\r\n            padding-right: 5px;\r\n            border-top:" +
                    " #f0f0e0 1px solid;\r\n            margin-top: -5px;\r\n            padding-left: 5p" +
                    "x;\r\n            font-size: x-small;\r\n            padding-bottom: 5px;\r\n         " +
                    "   border-left: #f0f0e0 1px solid;\r\n            padding-top: 5px;\r\n            b" +
                    "order-bottom: #f0f0e0 1px solid;\r\n            font-family: Courier New;\r\n       " +
                    "     background-color: #e5e5cc;\r\n        }\r\n\r\n        TD {\r\n            font-siz" +
                    "e: 12px;\r\n            color: #000000;\r\n            font-family: Tahoma;\r\n       " +
                    " }\r\n\r\n        H2 {\r\n            border-top: #003366 1px solid;\r\n            marg" +
                    "in-top: 25px;\r\n            font-weight: bold;\r\n            font-size: 1.5em;\r\n  " +
                    "          margin-bottom: 10px;\r\n            margin-left: -15px;\r\n            col" +
                    "or: #003366;\r\n        }\r\n\r\n        H3 {\r\n            margin-top: 10px;\r\n        " +
                    "    font-size: 1.1em;\r\n            margin-bottom: 10px;\r\n            margin-left" +
                    ": -15px;\r\n            color: #000000;\r\n        }\r\n\r\n        UL {\r\n            ma" +
                    "rgin-top: 10px;\r\n            margin-left: 20px;\r\n        }\r\n\r\n        OL {\r\n    " +
                    "        margin-top: 10px;\r\n            margin-left: 20px;\r\n        }\r\n\r\n        " +
                    "LI {\r\n            margin-top: 10px;\r\n            color: #000000;\r\n        }\r\n\r\n " +
                    "       FONT.value {\r\n            font-weight: bold;\r\n            color: darkblue" +
                    ";\r\n        }\r\n\r\n        FONT.key {\r\n            font-weight: bold;\r\n            " +
                    "color: darkgreen;\r\n        }\r\n\r\n        .divTag {\r\n            border: 1px;\r\n   " +
                    "         border-style: solid;\r\n            background-color: #FFFFFF;\r\n         " +
                    "   text-decoration: none;\r\n            height: auto;\r\n            width: auto;\r\n" +
                    "            background-color: #cecece;\r\n        }\r\n\r\n        .BannerColumn {\r\n  " +
                    "          background-color: #000000;\r\n        }\r\n\r\n        .Banner {\r\n          " +
                    "  border: 0;\r\n            padding: 0;\r\n            height: 8px;\r\n            mar" +
                    "gin-top: 0px;\r\n            color: #ffffff;\r\n            filter: progid:DXImageTr" +
                    "ansform.Microsoft.Gradient(GradientType=1,StartColorStr=\'#1c5280\',EndColorStr=\'#" +
                    "FFFFFF\');\r\n        }\r\n\r\n        .BannerTextCompany {\r\n            font: bold;\r\n " +
                    "           font-size: 18pt;\r\n            color: #cecece;\r\n            font-famil" +
                    "y: Tahoma;\r\n            height: 0px;\r\n            margin-top: 0;\r\n            ma" +
                    "rgin-left: 8px;\r\n            margin-bottom: 0;\r\n            padding: 0px;\r\n     " +
                    "       white-space: nowrap;\r\n            filter: progid:DXImageTransform.Microso" +
                    "ft.dropshadow(OffX=2,OffY=2,Color=\'black\',Positive=\'true\');\r\n        }\r\n\r\n      " +
                    "  .BannerTextApplication {\r\n            font: bold;\r\n            font-size: 18pt" +
                    ";\r\n            font-family: Tahoma;\r\n            height: 0px;\r\n            margi" +
                    "n-top: 0;\r\n            margin-left: 8px;\r\n            margin-bottom: 0;\r\n       " +
                    "     padding: 0px;\r\n            white-space: nowrap;\r\n            filter: progid" +
                    ":DXImageTransform.Microsoft.dropshadow(OffX=2,OffY=2,Color=\'black\',Positive=\'tru" +
                    "e\');\r\n        }\r\n\r\n        .BannerText {\r\n            font: bold;\r\n            f" +
                    "ont-size: 18pt;\r\n            font-family: Tahoma;\r\n            height: 0px;\r\n   " +
                    "         margin-top: 0;\r\n            margin-left: 8px;\r\n            margin-botto" +
                    "m: 0;\r\n            padding: 0px;\r\n            filter: progid:DXImageTransform.Mi" +
                    "crosoft.dropshadow(OffX=2,OffY=2,Color=\'black\',Positive=\'true\');\r\n        }\r\n\r\n " +
                    "       .BannerSubhead {\r\n            border: 0;\r\n            padding: 0;\r\n      " +
                    "      height: 16px;\r\n            margin-top: 0px;\r\n            margin-left: 10px" +
                    ";\r\n            color: #ffffff;\r\n            filter: progid:DXImageTransform.Micr" +
                    "osoft.Gradient(GradientType=1,StartColorStr=\'#4B3E1A\',EndColorStr=\'#FFFFFF\');\r\n " +
                    "       }\r\n\r\n        .BannerSubheadText {\r\n            font: bold;\r\n            h" +
                    "eight: 11px;\r\n            font-size: 11px;\r\n            font-family: Tahoma;\r\n  " +
                    "          margin-top: 1;\r\n            margin-left: 10;\r\n            filter: prog" +
                    "id:DXImageTransform.Microsoft.dropshadow(OffX=2,OffY=2,Color=\'black\',Positive=\'t" +
                    "rue\');\r\n        }\r\n\r\n        .FooterRule {\r\n            border: 0;\r\n            " +
                    "padding: 0;\r\n            height: 1px;\r\n            margin: 0px;\r\n            col" +
                    "or: #ffffff;\r\n            filter: progid:DXImageTransform.Microsoft.Gradient(Gra" +
                    "dientType=1,StartColorStr=\'#4B3E1A\',EndColorStr=\'#FFFFFF\');\r\n        }\r\n\r\n      " +
                    "  .FooterText {\r\n            font-size: 11px;\r\n            font-weight: normal;\r" +
                    "\n            text-decoration: none;\r\n            font-family: Tahoma;\r\n         " +
                    "   margin-top: 10;\r\n            margin-left: 0px;\r\n            margin-bottom: 2;" +
                    "\r\n            padding: 0px;\r\n            color: #999999;\r\n            white-spac" +
                    "e: nowrap;\r\n        }\r\n\r\n            .FooterText A:link {\r\n                font-" +
                    "weight: normal;\r\n                color: #999999;\r\n                text-decoratio" +
                    "n: underline;\r\n            }\r\n\r\n            .FooterText A:visited {\r\n           " +
                    "     font-weight: normal;\r\n                color: #999999;\r\n                text" +
                    "-decoration: underline;\r\n            }\r\n\r\n            .FooterText A:active {\r\n  " +
                    "              font-weight: normal;\r\n                color: #999999;\r\n           " +
                    "     text-decoration: underline;\r\n            }\r\n\r\n            .FooterText A:hov" +
                    "er {\r\n                font-weight: normal;\r\n                color: #FF6600;\r\n   " +
                    "             text-decoration: underline;\r\n            }\r\n\r\n        .ClickOnceInf" +
                    "oText {\r\n            font-size: 11px;\r\n            font-weight: normal;\r\n       " +
                    "     text-decoration: none;\r\n            font-family: Tahoma;\r\n            margi" +
                    "n-top: 0;\r\n            margin-right: 2px;\r\n            margin-bottom: 0;\r\n      " +
                    "      padding: 0px;\r\n            color: #000000;\r\n        }\r\n\r\n        .InstallT" +
                    "extStyle {\r\n            font: bold;\r\n            font-size: 14pt;\r\n            f" +
                    "ont-family: Tahoma;\r\n            a: #FF0000;\r\n            text-decoration: None;" +
                    "\r\n        }\r\n\r\n        .DetailsStyle {\r\n            margin-left: 30px;\r\n        " +
                    "}\r\n\r\n        .ItemStyle {\r\n            margin-left: -15px;\r\n            font-wei" +
                    "ght: bold;\r\n        }\r\n\r\n        .StartColorStr {\r\n            background-color:" +
                    " #4B3E1A;\r\n        }\r\n\r\n        .JustThisApp A:link {\r\n            font-weight: " +
                    "normal;\r\n            color: #000066;\r\n            text-decoration: underline;\r\n " +
                    "       }\r\n\r\n        .JustThisApp A:visited {\r\n            font-weight: normal;\r\n" +
                    "            color: #000066;\r\n            text-decoration: underline;\r\n        }\r" +
                    "\n\r\n        .JustThisApp A:active {\r\n            font-weight: normal;\r\n          " +
                    "  text-decoration: underline;\r\n        }\r\n\r\n        .JustThisApp A:hover {\r\n    " +
                    "        font-weight: normal;\r\n            color: #FF6600;\r\n            text-deco" +
                    "ration: underline;\r\n        }\r\n        -->\r\n    </STYLE>\r\n\r\n    <SCRIPT LANGUAGE" +
                    "=\"JavaScript\">\r\n<!--\r\nruntimeVersion = \"4.5.0\";\r\ncheckClient = false;\r\ndirectLin" +
                    "k = \"");
            
            #line 306 "C:\Users\ucrm4\Source\Repos\ClickTwice\src\ClickTwice.Publisher.Core\Resources\PublishPage.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(AppLauncher.Name));
            
            #line default
            #line hidden
            this.Write("\";\r\n\r\n\r\nfunction Initialize()\r\n{\r\n  if (HasRuntimeVersion(runtimeVersion, false) " +
                    "|| (checkClient && HasRuntimeVersion(runtimeVersion, checkClient)))\r\n  {\r\n    In" +
                    "stallButton.href = directLink;\r\n    BootstrapperSection.style.display = \"none\";\r" +
                    "\n  }\r\n}\r\nfunction HasRuntimeVersion(v, c)\r\n{\r\n  var va = GetVersion(v);\r\n  var i" +
                    ";\r\n  var a = navigator.userAgent.match(/\\.NET CLR [0-9.]+/g);\r\n  if(va[0]==4)\r\n " +
                    "   a = navigator.userAgent.match(/\\.NET[0-9.]+E/g);\r\n  if (c)\r\n  {\r\n    a = navi" +
                    "gator.userAgent.match(/\\.NET Client [0-9.]+/g);\r\n    if (va[0]==4)\r\n       a = n" +
                    "avigator.userAgent.match(/\\.NET[0-9.]+C/g);\r\n  }\r\n  if (a != null)\r\n    for (i =" +
                    " 0; i < a.length; ++i)\r\n      if (CompareVersions(va, GetVersion(a[i])) <= 0)\r\n " +
                    "                               return true;\r\n  return false;\r\n}\r\nfunction GetVer" +
                    "sion(v)\r\n{\r\n  var a = v.match(/([0-9]+)\\.([0-9]+)\\.([0-9]+)/i);\r\n  if(a==null)\r\n" +
                    "     a = v.match(/([0-9]+)\\.([0-9]+)/i);\r\n  return a.slice(1);\r\n}\r\nfunction Comp" +
                    "areVersions(v1, v2)\r\n{\r\n   if(v1.length>v2.length)\r\n   {\r\n      v2[v2.length]=0;" +
                    "\r\n   }\r\n   else if(v1.length<v2.length)\r\n   {\r\n      v1[v1.length]=0;\r\n   }\r\n\r\n " +
                    " for (i = 0; i < v1.length; ++i)\r\n  {\r\n    var n1 = new Number(v1[i]);\r\n    var " +
                    "n2 = new Number(v2[i]);\r\n    if (n1 < n2)\r\n      return -1;\r\n    if (n1 > n2)\r\n " +
                    "     return 1;\r\n  }\r\n  return 0;\r\n}\r\n\r\n-->\r\n    </SCRIPT>\r\n\r\n</HEAD>\r\n<BODY ONLO" +
                    "AD=\"Initialize()\">\r\n    <TABLE WIDTH=\"100%\" CELLPADDING=\"0\" CELLSPACING=\"2\" BORD" +
                    "ER=\"0\">\r\n\r\n        <!-- Begin Banner -->\r\n        <TR><TD><TABLE CELLPADDING=\"2\"" +
                    " CELLSPACING=\"0\" BORDER=\"0\" BGCOLOR=\"#cecece\" WIDTH=\"100%\"><TR><TD><TABLE BGCOLO" +
                    "R=\"#1c5280\" WIDTH=\"100%\" CELLPADDING=\"0\" CELLSPACING=\"0\" BORDER=\"0\"><TR><TD CLAS" +
                    "S=\"Banner\" /></TR><TR><TD CLASS=\"Banner\"><SPAN CLASS=\"BannerTextCompany\">Accentu" +
                    "re Australia</SPAN></TD></TR><TR><TD CLASS=\"Banner\"><SPAN CLASS=\"BannerTextAppli" +
                    "cation\">DocumentConversion</SPAN></TD></TR><TR><TD CLASS=\"Banner\" ALIGN=\"RIGHT\" " +
                    "/></TR></TABLE></TD></TR></TABLE></TD></TR>\r\n        <!-- End Banner -->\r\n      " +
                    "  <!-- Begin Dialog -->\r\n        <TR>\r\n            <TD ALIGN=\"LEFT\">\r\n          " +
                    "      <TABLE CELLPADDING=\"2\" CELLSPACING=\"0\" BORDER=\"0\" WIDTH=\"540\">\r\n          " +
                    "          <TR>\r\n                        <TD WIDTH=\"496\">\r\n\r\n                    " +
                    "        <!-- Begin AppInfo -->\r\n                            <TABLE><TR><TD COLSP" +
                    "AN=\"3\">&nbsp;</TD></TR><TR><TD><B>Name:</B></TD><TD WIDTH=\"5\"><SPACER TYPE=\"bloc" +
                    "k\" WIDTH=\"10\" /></TD><TD>");
            
            #line 384 "C:\Users\ucrm4\Source\Repos\ClickTwice\src\ClickTwice.Publisher.Core\Resources\PublishPage.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Manifest.ApplicationName));
            
            #line default
            #line hidden
            this.Write("</TD></TR><TR><TD COLSPAN=\"3\">&nbsp;</TD></TR><TR><TD><B>Version:</B></TD><TD WID" +
                    "TH=\"5\"><SPACER TYPE=\"block\" WIDTH=\"10\" /></TD><TD>");
            
            #line 384 "C:\Users\ucrm4\Source\Repos\ClickTwice\src\ClickTwice.Publisher.Core\Resources\PublishPage.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Manifest.AppVersion));
            
            #line default
            #line hidden
            this.Write("</TD></TR><TR><TD COLSPAN=\"3\">&nbsp;</TD></TR><TR><TD><B>Publisher:</B></TD><TD W" +
                    "IDTH=\"5\"><SPACER TYPE=\"block\" WIDTH=\"10\" /></TD><TD>");
            
            #line 384 "C:\Users\ucrm4\Source\Repos\ClickTwice\src\ClickTwice.Publisher.Core\Resources\PublishPage.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Manifest.PublisherName));
            
            #line default
            #line hidden
            this.Write(@"</TD></TR><tr><td colspan=""3"">&nbsp;</td></tr></TABLE>
                            <!-- End AppInfo -->
                            <!-- Begin Prerequisites -->
                            <TABLE ID=""BootstrapperSection"" BORDER=""0"">
                                <TR><TD COLSPAN=""2"">The following prerequisites are required:</TD></TR>
                                <TR>
                                    <TD WIDTH=""10"">&nbsp;</TD>
                                    <TD>
                                        <UL>
                                            <LI>Microsoft .NET Framework ");
            
            #line 393 "C:\Users\ucrm4\Source\Repos\ClickTwice\src\ClickTwice.Publisher.Core\Resources\PublishPage.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Manifest.FrameworkVersion));
            
            #line default
            #line hidden
            this.Write(@" (x86 and x64)</LI>
                                        </UL>
                                    </TD>
                                </TR>
                                <TR>
                                    <TD COLSPAN=""2"">
                                        If these components are already installed, you can <SPAN CLASS=""JustThisApp""><A HREF=""");
            
            #line 399 "C:\Users\ucrm4\Source\Repos\ClickTwice\src\ClickTwice.Publisher.Core\Resources\PublishPage.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(AppLauncher.Name));
            
            #line default
            #line hidden
            this.Write("\">launch</A></SPAN> the application now. Otherwise, click the button below to ins" +
                    "tall the prerequisites and run the application.\r\n                               " +
                    "     </TD>\r\n                                </TR>\r\n                             " +
                    "   <TR><TD COLSPAN=\"2\">&nbsp;</TD></TR>\r\n                            </TABLE>\r\n " +
                    "                           <!-- End Prerequisites -->\r\n\r\n\r\n                     " +
                    "   </TD>\r\n                    </TR>\r\n                </TABLE>\r\n                <" +
                    "!-- Begin Buttons -->\r\n        <TR><TD ALIGN=\"LEFT\"><TABLE CELLPADDING=\"2\" CELLS" +
                    "PACING=\"0\" BORDER=\"0\" WIDTH=\"540\" STYLE=\"cursor:hand\" ONCLICK=\"window.navigate(I" +
                    "nstallButton.href)\"><TR><TD ALIGN=\"LEFT\"><TABLE CELLPADDING=\"1\" BGCOLOR=\"#333333" +
                    "\" CELLSPACING=\"0\" BORDER=\"0\"><TR><TD><TABLE CELLPADDING=\"1\" BGCOLOR=\"#cecece\" CE" +
                    "LLSPACING=\"0\" BORDER=\"0\"><TR><TD><TABLE CELLPADDING=\"1\" BGCOLOR=\"#efefef\" CELLSP" +
                    "ACING=\"0\" BORDER=\"0\"><TR><TD WIDTH=\"20\"><SPACER TYPE=\"block\" WIDTH=\"20\" HEIGHT=\"" +
                    "1\" /></TD><TD><A ID=\"InstallButton\" HREF=\"setup.exe\">Install</A></TD><TD width=\"" +
                    "20\"><SPACER TYPE=\"block\" WIDTH=\"20\" HEIGHT=\"1\" /></TD></TR></TABLE></TD></TR></T" +
                    "ABLE></TD></TR></TABLE></TD><TD WIDTH=\"15%\" ALIGN=\"right\" /></TR></TABLE></TD></" +
                    "TR>\r\n        <!-- End Buttons -->\r\n        </TD></TR>\r\n        <!-- End Dialog -" +
                    "->\r\n        <!-- Spacer Row -->\r\n        <TR><TD>&nbsp;</TD></TR>\r\n\r\n        <TR" +
                    ">\r\n            <TD>\r\n                <!-- Begin Footer -->\r\n                <TAB" +
                    "LE WIDTH=\"100%\" CELLPADDING=\"0\" CELLSPACING=\"0\" BORDER=\"0\" BGCOLOR=\"#ffffff\">\r\n " +
                    "                   <TR><TD HEIGHT=\"5\"><SPACER TYPE=\"block\" HEIGHT=\"5\" /></TD></T" +
                    "R>\r\n                    <TR>\r\n                        <TD CLASS=\"FooterText\" ALI" +
                    "GN=\"center\">\r\n                            <A HREF=\"http://go.microsoft.com/fwlin" +
                    "k/?LinkId=154571\">ClickOnce and .NET Framework Resources</A>\r\n\t\t\t\t\t\t\t<A HREF=\"ht" +
                    "tps://bitbucket.org/agc93/clicktwice\">ClickTwice Home Page</A>\r\n                " +
                    "        </TD>\r\n                    </TR>\r\n                    <TR><TD HEIGHT=\"5\"" +
                    "><SPACER TYPE=\"block\" HEIGHT=\"5\" /></TD></TR>\r\n                    <TR><TD HEIGH" +
                    "T=\"1\" bgcolor=\"#cecece\"><SPACER TYPE=\"block\" HEIGHT=\"1\" /></TD></TR>\r\n          " +
                    "      </TABLE>\r\n                <!-- End Footer -->\r\n            </TD>\r\n        " +
                    "</TR>\r\n\r\n    </TABLE>\r\n</BODY>\r\n</HTML>");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
    #region Base class
    /// <summary>
    /// Base class for this transformation
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "14.0.0.0")]
    public class PublishPageBase
    {
        #region Fields
        private global::System.Text.StringBuilder generationEnvironmentField;
        private global::System.CodeDom.Compiler.CompilerErrorCollection errorsField;
        private global::System.Collections.Generic.List<int> indentLengthsField;
        private string currentIndentField = "";
        private bool endsWithNewline;
        private global::System.Collections.Generic.IDictionary<string, object> sessionField;
        #endregion
        #region Properties
        /// <summary>
        /// The string builder that generation-time code is using to assemble generated output
        /// </summary>
        protected System.Text.StringBuilder GenerationEnvironment
        {
            get
            {
                if ((this.generationEnvironmentField == null))
                {
                    this.generationEnvironmentField = new global::System.Text.StringBuilder();
                }
                return this.generationEnvironmentField;
            }
            set
            {
                this.generationEnvironmentField = value;
            }
        }
        /// <summary>
        /// The error collection for the generation process
        /// </summary>
        public System.CodeDom.Compiler.CompilerErrorCollection Errors
        {
            get
            {
                if ((this.errorsField == null))
                {
                    this.errorsField = new global::System.CodeDom.Compiler.CompilerErrorCollection();
                }
                return this.errorsField;
            }
        }
        /// <summary>
        /// A list of the lengths of each indent that was added with PushIndent
        /// </summary>
        private System.Collections.Generic.List<int> indentLengths
        {
            get
            {
                if ((this.indentLengthsField == null))
                {
                    this.indentLengthsField = new global::System.Collections.Generic.List<int>();
                }
                return this.indentLengthsField;
            }
        }
        /// <summary>
        /// Gets the current indent we use when adding lines to the output
        /// </summary>
        public string CurrentIndent
        {
            get
            {
                return this.currentIndentField;
            }
        }
        /// <summary>
        /// Current transformation session
        /// </summary>
        public virtual global::System.Collections.Generic.IDictionary<string, object> Session
        {
            get
            {
                return this.sessionField;
            }
            set
            {
                this.sessionField = value;
            }
        }
        #endregion
        #region Transform-time helpers
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void Write(string textToAppend)
        {
            if (string.IsNullOrEmpty(textToAppend))
            {
                return;
            }
            // If we're starting off, or if the previous text ended with a newline,
            // we have to append the current indent first.
            if (((this.GenerationEnvironment.Length == 0) 
                        || this.endsWithNewline))
            {
                this.GenerationEnvironment.Append(this.currentIndentField);
                this.endsWithNewline = false;
            }
            // Check if the current text ends with a newline
            if (textToAppend.EndsWith(global::System.Environment.NewLine, global::System.StringComparison.CurrentCulture))
            {
                this.endsWithNewline = true;
            }
            // This is an optimization. If the current indent is "", then we don't have to do any
            // of the more complex stuff further down.
            if ((this.currentIndentField.Length == 0))
            {
                this.GenerationEnvironment.Append(textToAppend);
                return;
            }
            // Everywhere there is a newline in the text, add an indent after it
            textToAppend = textToAppend.Replace(global::System.Environment.NewLine, (global::System.Environment.NewLine + this.currentIndentField));
            // If the text ends with a newline, then we should strip off the indent added at the very end
            // because the appropriate indent will be added when the next time Write() is called
            if (this.endsWithNewline)
            {
                this.GenerationEnvironment.Append(textToAppend, 0, (textToAppend.Length - this.currentIndentField.Length));
            }
            else
            {
                this.GenerationEnvironment.Append(textToAppend);
            }
        }
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void WriteLine(string textToAppend)
        {
            this.Write(textToAppend);
            this.GenerationEnvironment.AppendLine();
            this.endsWithNewline = true;
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void Write(string format, params object[] args)
        {
            this.Write(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void WriteLine(string format, params object[] args)
        {
            this.WriteLine(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }
        /// <summary>
        /// Raise an error
        /// </summary>
        public void Error(string message)
        {
            System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
            error.ErrorText = message;
            this.Errors.Add(error);
        }
        /// <summary>
        /// Raise a warning
        /// </summary>
        public void Warning(string message)
        {
            System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
            error.ErrorText = message;
            error.IsWarning = true;
            this.Errors.Add(error);
        }
        /// <summary>
        /// Increase the indent
        /// </summary>
        public void PushIndent(string indent)
        {
            if ((indent == null))
            {
                throw new global::System.ArgumentNullException("indent");
            }
            this.currentIndentField = (this.currentIndentField + indent);
            this.indentLengths.Add(indent.Length);
        }
        /// <summary>
        /// Remove the last indent that was added with PushIndent
        /// </summary>
        public string PopIndent()
        {
            string returnValue = "";
            if ((this.indentLengths.Count > 0))
            {
                int indentLength = this.indentLengths[(this.indentLengths.Count - 1)];
                this.indentLengths.RemoveAt((this.indentLengths.Count - 1));
                if ((indentLength > 0))
                {
                    returnValue = this.currentIndentField.Substring((this.currentIndentField.Length - indentLength));
                    this.currentIndentField = this.currentIndentField.Remove((this.currentIndentField.Length - indentLength));
                }
            }
            return returnValue;
        }
        /// <summary>
        /// Remove any indentation
        /// </summary>
        public void ClearIndent()
        {
            this.indentLengths.Clear();
            this.currentIndentField = "";
        }
        #endregion
        #region ToString Helpers
        /// <summary>
        /// Utility class to produce culture-oriented representation of an object as a string.
        /// </summary>
        public class ToStringInstanceHelper
        {
            private System.IFormatProvider formatProviderField  = global::System.Globalization.CultureInfo.InvariantCulture;
            /// <summary>
            /// Gets or sets format provider to be used by ToStringWithCulture method.
            /// </summary>
            public System.IFormatProvider FormatProvider
            {
                get
                {
                    return this.formatProviderField ;
                }
                set
                {
                    if ((value != null))
                    {
                        this.formatProviderField  = value;
                    }
                }
            }
            /// <summary>
            /// This is called from the compile/run appdomain to convert objects within an expression block to a string
            /// </summary>
            public string ToStringWithCulture(object objectToConvert)
            {
                if ((objectToConvert == null))
                {
                    throw new global::System.ArgumentNullException("objectToConvert");
                }
                System.Type t = objectToConvert.GetType();
                System.Reflection.MethodInfo method = t.GetMethod("ToString", new System.Type[] {
                            typeof(System.IFormatProvider)});
                if ((method == null))
                {
                    return objectToConvert.ToString();
                }
                else
                {
                    return ((string)(method.Invoke(objectToConvert, new object[] {
                                this.formatProviderField })));
                }
            }
        }
        private ToStringInstanceHelper toStringHelperField = new ToStringInstanceHelper();
        /// <summary>
        /// Helper to produce culture-oriented representation of an object as a string
        /// </summary>
        public ToStringInstanceHelper ToStringHelper
        {
            get
            {
                return this.toStringHelperField;
            }
        }
        #endregion
    }
    #endregion
}
