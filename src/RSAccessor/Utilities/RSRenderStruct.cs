// Copyright (c) 2016 Microsoft Corporation. All Rights Reserved.
// Licensed under the MIT License (MIT)

using System;
using System.Reflection;
using System.Collections.Specialized;
using System.Threading;

namespace RSAccessor.Utilities
{

    /// <summary>
    /// Abstract base class for Render Methods
    /// </summary>
    public abstract class RSRenderStruct
    {
        public abstract NameValueCollection BuildParamCollection();
        public abstract String RenderFormat
        {
            get;
        }
        public virtual String DeviceInfoXMLString
        {
            get
            {
                string XMLParamCollection = "<DeviceInfo>";
                NameValueCollection paramCollection = BuildParamCollection();
                foreach (string nv in paramCollection.AllKeys)
                {
                    XMLParamCollection += string.Format("<{0}><![CDATA[{1}]]></{0}>", nv, paramCollection[nv]);
                }

                XMLParamCollection += "</DeviceInfo>";

                return XMLParamCollection;
            }
        }

        public static RSRenderStruct PopulateRenderStruct(string RenderFormat, NameValueCollection DeviceInfos)
        {
            foreach (Type t in typeof(RSRenderStruct).Assembly.GetTypes())
            {
                if (typeof(RSRenderStruct).IsAssignableFrom(t) && !t.IsAbstract)
                {
                    RSRenderStruct rs;
                    try { rs = (RSRenderStruct)Activator.CreateInstance(t); }
                    catch { continue; }

                    if (string.Compare(rs.RenderFormat, RenderFormat, true) == 0)
                    {
                        rs.Set(DeviceInfos);

                        return rs;
                    }
                }
            }

            throw new ApplicationException(string.Format("Cannot find an RSRenderStruct for Render Format \"{0}\"", RenderFormat));
        }

        public void Set(NameValueCollection deviceInfo)
        {
            foreach (string key in deviceInfo.Keys)
            {
                PropertyInfo diProp = null;

                foreach (PropertyInfo pi in this.GetType().GetProperties())
                {
                    if (string.Compare(pi.Name, key, true) == 0)
                    {
                        diProp = pi;
                        break;
                    }
                }

                if (diProp == null)
                    throw new ApplicationException(string.Format("Property {0} not found in type {1}",
                        key, this.GetType().Name));

                if (!typeof(string).IsAssignableFrom(diProp.PropertyType))
                    throw new ApplicationException(string.Format("Property {0} is of type \"{1}\". Expecting of type \"{2}\"",
                        key, diProp.PropertyType.FullName, typeof(string).FullName));

                if (!diProp.CanWrite)
                    throw new ApplicationException(string.Format("Propery {0}.{1} is read-only", this.GetType().Name, key));

                diProp.SetValue(this, deviceInfo[key], null);
            }
        }
    }

    public interface IRSMultiPageRenderStruct
    {
        /// <summary>
        /// Sets the current page for the RSRenderStruct
        /// </summary>
        /// <param name="PageNumber">The page number to set in the render struct</param>
        void SetCurrentPage(int PageNumber);
    }

    #region CSVRender
    public class CSVRender : RSRenderStruct
    {
        #region Members

        private String m_encoding = "UTF-8";
        private String m_extension = "CSV";
        private String m_fieldDelimiter = ",";
        private String m_noHeader = "false";
        private String m_qualifier = "\"";
        private String m_recordDelimiter = "\r\n";
        private String m_suppressLineBreaks = "false";
        private String m_useFormattedValues = "true";
        private String m_excelMode = "true";

        #endregion

        #region Constructor

        /// <summary>
        /// CSV Url Render. 9 DeviceInfo Props: ExcelMode, UseformattedValues, Encoding, Extension, FieldDelimiter, NoHeader, Qualifier, RecordDelimiter, SuppressLineBreaks
        /// </summary>
        public CSVRender()
            : base()
        {
            // on machines which has "," as decimal seperator, set csv field delimiter as ";";
            // otherwise, using "," will cause excel cannot open the csv file, and oledb cannot parse it
            if (Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator.Equals(","))
                m_fieldDelimiter = ";";
        }

        #endregion

        #region Public Properties
        /// <summary>
        /// Return the format this RSRenderStruct outputs.
        /// </summary>
        public override string RenderFormat
        {
            get { return this.getRenderFormat(); }
        }

        /// <summary>
        /// One of the character encoding schemas: ASCII, UTF-7, UTF-8, or Unicode. The default value is Unicode.
        /// </summary> 
        public string Encoding
        {
            get { return m_encoding; }
            set { m_encoding = value; }
        }
        /// <summary>
        /// The file extension to put on the result. The default value is .CSV.
        /// </summary> 
        public string Extension
        {
            get { return m_extension; }
            set { m_extension = value; }
        }
        /// <summary>
        /// The Delimiter string to put in the result. The default value is a comma (,). You should URL encode the value of this device information when passing it on a URL. For example, a tab character as a Delimiter should be "%09".
        /// </summary> 
        public string FieldDelimiter
        {
            get { return m_fieldDelimiter; }
            set { m_fieldDelimiter = value; }
        }
        /// <summary>
        /// Indicates whether the header row is excluded from the output. The default value is false.
        /// </summary> 
        public string NoHeader
        {
            get { return m_noHeader; }
            set { m_noHeader = value; }
        }
        /// <summary>
        /// The qualifier string to put around results that contain the field Delimiter or record Delimiter. If the results contain the qualifier, the qualifier is repeated. The Qualifier setting must be different from the FieldDelimiter and RecordDelimiter settings. The default value is a quotation mark (").
        /// </summary> 
        public string Qualifier
        {
            get { return m_qualifier; }
            set { m_qualifier = value; }
        }
        /// <summary>
        /// The record Delimiter to put at the end of each record. The default value is <cr><lf>.
        /// </summary> 
        public string RecordDelimiter
        {
            get { return m_recordDelimiter; }
            set { m_recordDelimiter = value; }
        }
        /// <summary>
        /// Indicates whether line breaks are removed from the data included in the output. The default value is false. If the value is true, the FieldDelimiter, RecordDelimiter, and Qualifier settings cannot be a space character.
        /// </summary> 
        public string SuppressLineBreaks
        {
            get { return m_suppressLineBreaks; }
            set { m_suppressLineBreaks = value; }
        }
        /// <summary>
        /// Whether or not to emit formatted strings into the CSV output.  When true, renderer should use TextBox.Value property.  When false, use TextBox.OriginalValue.
        /// </summary> 
        public String UseFormattedValues
        {
            get { return m_useFormattedValues; }
            set { m_useFormattedValues = value; }
        }
        /// <summary>
        /// When true, targets data export for Excel.  If false, data is arranged Diagonally.
        /// </summary> 
        public String ExcelMode
        {
            get { return m_excelMode; }
            set { m_excelMode = value; }
        }
        #endregion

        #region Methods

        /// <summary>
        /// Builds Parameter Collection 
        /// </summary>
        public override NameValueCollection BuildParamCollection()
        {
            NameValueCollection paramCollection = new NameValueCollection();
            paramCollection.Add("Encoding", this.Encoding);
            paramCollection.Add("Extension", this.Extension);
            paramCollection.Add("FieldDelimiter", this.FieldDelimiter);
            paramCollection.Add("NoHeader", this.NoHeader);
            paramCollection.Add("Qualifier", this.Qualifier);
            paramCollection.Add("RecordDelimiter", this.RecordDelimiter);
            paramCollection.Add("SuppressLineBreaks", this.SuppressLineBreaks);
            paramCollection.Add("UseFormattedValues", this.UseFormattedValues);
            paramCollection.Add("ExcelMode", this.ExcelMode);
            return paramCollection;
        }

        /// <summary>
        /// Returns format
        /// </summary>
        /// <returns></returns>
        public string getRenderFormat()
        {
            return "CSV";
        }

        #endregion
    }
    #endregion

    #region ATOMRender
    public class ATOMRender : RSRenderStruct
    {
        #region Members

        private String m_encoding = null;
        private String m_dataFeed = null;
        private String m_itemPath = null;

        #endregion

        #region Constructor

        /// <summary>
        /// ATOM Url Render. 2 DeviceInfo Props: Encoding, DateFeed
        /// </summary>
        public ATOMRender()
            : base()
        {

        }

        #endregion

        #region Public Properties
        /// <summary>
        /// Return the format this RSRenderStruct outputs.
        /// </summary>
        public override string RenderFormat
        {
            get { return this.getRenderFormat(); }
        }

        /// <summary>
        /// One of the character encoding schemas: ASCII, UTF-7, UTF-8, or Unicode. 
        /// The default value is null  (=> device info is not set).
        /// </summary> 
        public string Encoding
        {
            get { return m_encoding; }
            set { m_encoding = value; }
        }

        /// <summary>
        /// The Data Feed identifier. The default value is null (=> device info is not set), 
        /// which will produce an Atom Service Document.
        /// </summary> 
        public string DataFeed
        {
            get { return m_dataFeed; }
            set { m_dataFeed = value; }
        }

        /// <summary>
        /// The Item Path, used as identifier, e.g. "Tablix.Group1". 
        /// </summary> 
        public string ItemPath
        {
            get { return m_itemPath; }
            set { m_itemPath = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Builds Parameter Collection 
        /// </summary>
        public override NameValueCollection BuildParamCollection()
        {
            NameValueCollection paramCollection = new NameValueCollection();
            if (this.Encoding != null) paramCollection.Add("Encoding", this.Encoding);
            if (this.DataFeed != null) paramCollection.Add("DataFeed", this.DataFeed);
            if (this.ItemPath != null) paramCollection.Add("ItemPath", this.ItemPath);
            return paramCollection;
        }

        /// <summary>
        /// Returns format
        /// </summary>
        /// <returns></returns>
        public string getRenderFormat()
        {
            return "ATOM";
        }

        #endregion
    }
    #endregion

    #region EXCELRender
    public class EXCELRender : RSRenderStruct
    {
        #region Members

        private String m_omitDocumentMap = "false";
        private String m_omitFormulas = "false";
        private String m_removeSpaces = "0.125in";
        private String m_SimplePageHeaders = "false";

        #endregion

        #region Constructor

        /// <summary>
        /// EXCEL Url Render. 4 DeviceInfo Props: OmitDocumentMap, OmitForumulas, RemoveSpace, SimplePageHeaders
        /// </summary>
        public EXCELRender()
            : base()
        {

        }

        #endregion

        #region Public Properties
        /// <summary>
        /// Return the format this RSRenderStruct outputs.
        /// </summary>
        public override string RenderFormat
        {
            get { return this.getRenderFormat(); }
        }

        /// <summary>
        /// Omit document map for reports that support it. The default value is false.
        /// </summary> 
        public string OmitDocumentMap
        {
            get { return m_omitDocumentMap; }
            set { m_omitDocumentMap = value; }
        }
        /// <summary>
        /// Omit formulas from the rendered report. The default value is false.
        /// </summary>
        public string OmitFormulas
        {
            get { return m_omitFormulas; }
            set { m_omitFormulas = value; }
        }
        /// <summary>
        /// Omit rows or columns with no data smaller than the given size. Must supply string number as string followed by in, such as 0.5in
        /// </summary> 
        public string RemoveSpace
        {
            get { return m_removeSpaces; }
            set { m_removeSpaces = value; }
        }
        /// <summary>
        /// Indicate whether the page header is rendered to the Excel page header. Default value of false means the header is rendered to the first row of the worksheet.
        /// </summary>
        public string SimplePageHeaders
        {
            get { return m_SimplePageHeaders; }
            set { m_SimplePageHeaders = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Builds Parameter Collection 
        /// </summary>
        public override NameValueCollection BuildParamCollection()
        {
            NameValueCollection paramCollection = new NameValueCollection();
            if (this.OmitDocumentMap != null) paramCollection.Add("OmitDocumentMap", this.OmitDocumentMap);
            if (this.OmitFormulas != null) paramCollection.Add("OmitFormulas", this.OmitFormulas);
            if (this.RemoveSpace != null) paramCollection.Add("RemoveSpace", this.RemoveSpace);
            if (this.SimplePageHeaders != null) paramCollection.Add("SimplePageHeaders", this.SimplePageHeaders);
            return paramCollection;
        }

        /// <summary>
        /// Returns format
        /// </summary>
        /// <returns></returns>
        public virtual string getRenderFormat()
        {
            return "EXCEL";
        }

        #endregion
    }
    #endregion

    #region EXCELOOXMLRender

    public class EXCELOPENXMLRender : EXCELRender
    {
        #region Constructor

        /// <summary>
        /// EXCELOOXML Url Render. 4 DeviceInfo Props: OmitDocumentMap, OmitForumulas, RemoveSpace, SimplePageHeaders
        /// </summary>
        public EXCELOPENXMLRender()
            : base()
        {

        }

        #endregion

        #region Methods
        /// <summary>
        /// Returns format
        /// </summary>
        /// <returns></returns>
        public override string getRenderFormat()
        {
            return "EXCELOPENXML";
        }

        #endregion

    }
    #endregion

    #region HTMLRenderBase
    public abstract class HTMLRenderBase : RSRenderStruct
    {
        #region Members

        private String m_section = "0";

        #endregion

        #region Constructor

        /// <summary>
        /// HTML Url Render. 20 DeviceInfo Props: BookmarkID, DocMap, DocMapID, EndFind, FallbackPage, FindString, GetImage, HTMLFragment, Icon, JavaScript, LinkTarget, Parameters, ReplacementRoot, Section, StartFind, StreamRoot, StyleStream, Toolbar, Type, Zoom
        /// </summary>
        public HTMLRenderBase()
            : base()
        {

        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The page number of the report to render. A value of 0 indicates that all sections of the report are rendered. The default value is 1.
        /// </summary> 
        public string Section
        {
            get { return m_section; }
            set { m_section = value; }
        }

        #endregion
    }
    #endregion

    #region HTMLRender
    public abstract class HTMLRender : HTMLRenderBase
    {
        #region Members

        private String m_bookmarkID = null;
        private String m_docMap = "false";
        private String m_docMapID = null;
        private String m_endFind = null;
        private String m_fallbackPage = null;
        private String m_findString = null;
        private String m_getImage = null;
        private String m_HTMLFragment = "false";
        private String m_icon = null;
        private String m_javaScript = "false";
        private String m_linkTarget = null;
        private String m_parameters = "false";
        private String m_replacementRoot = null;
        private String m_startFind = null;
        private String m_streamRoot = null;
        private String m_styleStream = "false";
        private String m_toolbar = "false";
        private String m_type = null;
        private String m_zoom = "100";
        private String m_accessibleTablix = "false";
        private String m_imageConsolidation = "true";

        #endregion

        #region Constructor

        /// <summary>
        /// HTML Url Render. 20 DeviceInfo Props: BookmarkID, DocMap, DocMapID, EndFind, FallbackPage, FindString, GetImage, HTMLFragment, Icon, JavaScript, LinkTarget, Parameters, ReplacementRoot, Section, StartFind, StreamRoot, StyleStream, Toolbar, Type, Zoom
        /// </summary>
        public HTMLRender()
            : base()
        {

        }

        #endregion

        #region Public Properties
        /// <summary>
        /// Return the format this RSRenderStruct outputs.
        /// </summary>
        public override string RenderFormat
        {
            get { return this.getRenderFormat(); }
        }

        /// <summary>
        /// The bookmark ID to jump to in the report.
        /// </summary> 
        public string BookmarkID
        {
            get { return m_bookmarkID; }
            set { m_bookmarkID = value; }
        }
        /// <summary>
        /// Indicates whether to show or hide the report document map. The default value of this parameter is true.
        /// </summary> 
        public string DocMap
        {
            get { return m_docMap; }
            set { m_docMap = value; }
        }
        /// <summary>
        /// The document map ID to scroll to in the report.
        /// </summary> 
        public string DocMapID
        {
            get { return m_docMapID; }
            set { m_docMapID = value; }
        }
        /// <summary>
        /// The number of the last page to use in the search. For example, a value of 5 indicates that the last page to be searched is page 5 of the report. The default value is the number of the current page. Use this setting in conjunction with the StartFind setting.
        /// </summary> 
        public string EndFind
        {
            get { return m_endFind; }
            set { m_endFind = value; }
        }
        /// <summary>
        /// The number of the page to display if a search or a document map selection fails. The default value is the number of the current page.
        /// </summary> 
        public string FallbackPage
        {
            get { return m_fallbackPage; }
            set { m_fallbackPage = value; }
        }
        /// <summary>
        /// The text to search for in the report. The default value of this parameter is an empty string.
        /// </summary> 
        public string FindString
        {
            get { return m_findString; }
            set { m_findString = value; }
        }
        /// <summary>
        /// Gets a particular icon for the HTML Viewer user interface.
        /// </summary> 
        public string GetImage
        {
            get { return m_getImage; }
            set { m_getImage = value; }
        }
        /// <summary>
        /// Indicates whether an HTML fragment is created in place of a full HTML document. An HTML fragment includes the report content in a TABLE element and omits the HTML and BODY elements. The default value is false. If you are rendering to HTML using the Render method of the SOAP API, you need to set this device information to true if you are rendering a report with images. Rendering using SOAP with the HTMLFragment property set to true creates URLs containing session information that can be used to properly request images. The images must be uploaded resources in the report server database.
        /// </summary> 
        public string HTMLFragment
        {
            get { return m_HTMLFragment; }
            set { m_HTMLFragment = value; }
        }
        /// <summary>
        /// The icon of a particular rendering extension.
        /// </summary> 
        public string Icon
        {
            get { return m_icon; }
            set { m_icon = value; }
        }
        /// <summary>
        /// Indicates whether JavaScript is supported in the rendered report.
        /// </summary> 
        public string JavaScript
        {
            get { return m_javaScript; }
            set { m_javaScript = value; }
        }
        /// <summary>
        /// The target for hyperlinks in the report. You can target a window or frame by providing the name of the window, like LinkTarget=window_name, or you can target a new window using LinkTarget=_blank. Other valid target names include _self, _parent, and _top.
        /// </summary> 
        public string LinkTarget
        {
            get { return m_linkTarget; }
            set { m_linkTarget = value; }
        }
        /// <summary>
        /// Indicates whether to show or hide the parameters area of the toolbar. If you set this parameter to a value of true, the parameters area of the toolbar is displayed. The default value of this parameter is true.
        /// </summary> 
        public string Parameters
        {
            get { return m_parameters; }
            set { m_parameters = value; }
        }
        /// <summary>
        /// The path used for prefixing the value of the href attribute of the A element in the HTML report returned by the report server. By default, the report server provides the path. You can use this setting to specify a root path for any hyperlinks in your report.
        /// </summary> 
        public string ReplacementRoot
        {
            get { return m_replacementRoot; }
            set { m_replacementRoot = value; }
        }
        /// <summary>
        /// The number of the page on which to begin the search. The default value is the number of the current page. Use this setting in conjunction with the EndFind setting.
        /// </summary> 
        public string StartFind
        {
            get { return m_startFind; }
            set { m_startFind = value; }
        }
        /// <summary>
        /// The path used for prefixing the value of the src attribute of the IMG element in the HTML report returned by the report server. By default, the report server provides the path. You can use this setting to specify a root path for the images in a report (for example, http://myserver/resources/companyimages).
        /// </summary> 
        public string StreamRoot
        {
            get { return m_streamRoot; }
            set { m_streamRoot = value; }
        }
        /// <summary>
        /// Indicates whether styles and scripts are created as a separate stream instead of in the document. The default value is false.
        /// </summary> 
        public string StyleStream
        {
            get { return m_styleStream; }
            set { m_styleStream = value; }
        }
        /// <summary>
        /// Indicates whether to show or hide the toolbar. The default of this parameter is true. If the value of this parameter is false, all remaining options (except the document map) are ignored. If you omit this parameter, the toolbar is automatically displayed for rendering formats that support it.
        /// The Report Viewer toolbar is rendered when you use URL access to render a report. The toolbar is not rendered through the SOAP API. However, the Toolbar device information setting affects the way that the report is displayed when using the SOAP Render method. If the value of this parameter is true when using SOAP to render to HTML, only the first section of the report is rendered. If the value is false, the entire HTML report is rendered as a single HTML page.
        /// </summary> 
        public string Toolbar
        {
            get { return m_toolbar; }
            set { m_toolbar = value; }
        }
        /// <summary>
        /// The short name of the browser type (for example, "IE5") as defined in browscap.ini.
        /// </summary> 
        public string Type
        {
            get { return m_type; }
            set { m_type = value; }
        }
        /// <summary>
        /// The report zoom value as an integer percentage or a string constant. Standard string values include Page Width and Whole Page. This parameter is ignored by versions of Microsoft Internet Explorer earlier than Internet Explorer 5.0 and all non-Microsoft browsers. The default value of this parameter is 100.
        /// </summary> 
        public string Zoom
        {
            get { return m_zoom; }
            set { m_zoom = value; }
        }

        /// <summary>
        /// Indicates whether the accessiblity info (headers="...") will be output for a tablix. The default value is false.
        /// </summary> 
        public string AccessibleTablix
        {
            get { return m_accessibleTablix; }
            set { m_accessibleTablix = value; }
        }

        public string ImageConsolidation
        {
            get { return m_imageConsolidation; }
            set { m_imageConsolidation = value; }
        }
        #endregion

        #region Methods

        /// <summary>
        /// Builds Parameter Collection 
        /// </summary>
        public override NameValueCollection BuildParamCollection()
        {
            NameValueCollection paramCollection = new NameValueCollection();
            if (this.BookmarkID != null) paramCollection.Add("BookmarkID", this.BookmarkID);
            if (this.DocMap != null) paramCollection.Add("DocMap", this.DocMap);
            if (this.DocMapID != null) paramCollection.Add("DocMapID", this.DocMapID);
            if (this.EndFind != null) paramCollection.Add("EndFind", this.EndFind);
            if (this.FallbackPage != null) paramCollection.Add("FallbackPage", this.FallbackPage);
            if (this.FindString != null) paramCollection.Add("FindString", this.FindString);
            if (this.GetImage != null) paramCollection.Add("GetImage", this.GetImage);
            if (this.HTMLFragment != null) paramCollection.Add("HTMLFragment", this.HTMLFragment);
            if (this.Icon != null) paramCollection.Add("Icon", this.Icon);
            if (this.JavaScript != null) paramCollection.Add("JavaScript", this.JavaScript);
            if (this.LinkTarget != null) paramCollection.Add("LinkTarget", this.LinkTarget);
            if (this.Parameters != null) paramCollection.Add("Parameters", this.Parameters);
            if (this.ReplacementRoot != null) paramCollection.Add("ReplacementRoot", this.ReplacementRoot);
            if (this.Section != null) paramCollection.Add("Section", this.Section);
            if (this.StartFind != null) paramCollection.Add("StartFind", this.StartFind);
            if (this.StreamRoot != null) paramCollection.Add("StreamRoot", this.StreamRoot);
            if (this.StyleStream != null) paramCollection.Add("StyleStream", this.StyleStream);
            if (this.Toolbar != null) paramCollection.Add("Toolbar", this.Toolbar);
            if (this.Type != null) paramCollection.Add("Type", this.Type);
            if (this.Zoom != null) paramCollection.Add("Zoom", this.Zoom);
            if (this.AccessibleTablix != null) paramCollection.Add("AccessibleTablix", this.AccessibleTablix);
            if (this.ImageConsolidation != null) paramCollection.Add("ImageConsolidation", this.ImageConsolidation);
            return paramCollection;
        }

        /// <summary>
        /// Returns format
        /// </summary>
        /// <returns></returns>
        public abstract string getRenderFormat();

        #endregion
    }
    #endregion

    #region HTML40Render
    public sealed class HTML40Render : HTMLRender
    {
        /// <summary>
        /// Returns format
        /// </summary>
        /// <returns></returns>
        public override string getRenderFormat()
        {
            return "HTML4.0";
        }
    }
    #endregion

    #region HTML50Render
    public sealed class HTML50Render : HTMLRender
    {
        /// <summary>
        /// Returns format
        /// </summary>
        /// <returns></returns>
        public override string getRenderFormat()
        {
            return "HTML5";
        }
    }
    #endregion

    #region IMAGERender
    public class IMAGERender : RSRenderStruct
    {
        #region Members

        private String m_colorDepth = null;
        private String m_columns = null;
        private String m_columnSpacing = null;
        private String m_dpiX = null;
        private String m_dpiY = null;
        private String m_endPage = null;
        private String m_marginBottom = null;
        private String m_marginLeft = null;
        private String m_marginRight = null;
        private String m_marginTop = null;
        private String m_outputFormat = null;
        private String m_pageHeight = null;
        private String m_pageWidth = null;
        private String m_startPage = null;
        private String m_printDpiX = null;
        private String m_printDpiY = null;


        #endregion

        #region Constructor

        /// <summary>
        /// IMAGE Url Render. 14 DeviceInfo Props: ColorDepth, Columns, ColumnSpacing, DpiX, DpiY, EndPage, MarginBottom, MarginLeft, MarginRight, MarginTop, OutputFormat, PageHeight, PageWidth, StartPage
        /// </summary>
        public IMAGERender()
            : base()
        {

        }

        #endregion

        #region Public Properties
        /// <summary>
        /// Return the format this RSRenderStruct outputs.
        /// </summary>
        public override string RenderFormat
        {
            get { return this.getRenderFormat(); }
        }

        /// <summary>
        /// The pixel depth of the color range supported by the image output. Valid values are 1, 4, 8, 24, and 32. The default value is 24. ColorDepth is only supported for TIFF rendering and is otherwise ignored by the report server for other image output formats.
        /// Note: For the 2005 release of SQL Server, the value of this setting is ignored, and the TIFF image is always rendered as 24-bit.
        /// </summary> 
        public string ColorDepth
        {
            get { return m_colorDepth; }
            set { m_colorDepth = value; }
        }
        /// <summary>
        /// The number of columns to set for the report. This value overrides the report's original settings.
        /// </summary> 
        public string Columns
        {
            get { return m_columns; }
            set { m_columns = value; }
        }
        /// <summary>
        /// The column spacing to set for the report. This value overrides the report's original settings.
        /// </summary> 
        public string ColumnSpacing
        {
            get { return m_columnSpacing; }
            set { m_colorDepth = value; }
        }
        /// <summary>
        /// The resolution of the output device in x-direction. The default value is 96.
        /// </summary> 
        public string DpiX
        {
            get { return m_dpiX; }
            set { m_dpiX = value; }
        }
        /// <summary>
        /// The resolution of the output device in y-direction. The default value is 96.
        /// </summary> 
        public string DpiY
        {
            get { return m_dpiY; }
            set { m_dpiY = value; }
        }
        /// <summary>
        /// The resolution of the output device in x-direction for EMF text. The default value is 120.
        /// </summary> 
        public string PrintDpiX
        {
            get { return m_printDpiX; }
            set { m_printDpiX = value; }
        }
        /// <summary>
        /// The resolution of the output device in y-direction for EMF text. The default value is 120.
        /// </summary> 
        public string PrintDpiY
        {
            get { return m_printDpiY; }
            set { m_printDpiY = value; }
        }

        public int? iPrintDpiX
        {
            get
            {
                if (m_printDpiX == null || m_printDpiX.Trim().Length == 0)
                    return null;
                else
                    return Convert.ToInt32(m_printDpiX);
            }
        }

        public int? iPrintDpiY
        {
            get
            {
                if (m_printDpiY == null || m_printDpiY.Trim().Length == 0)
                    return null;
                else
                    return Convert.ToInt32(m_printDpiY);
            }
        }

        public int? iDpiX
        {
            get
            {
                if (m_dpiX == null || m_dpiX.Trim().Length == 0)
                    return null;
                else
                    return Convert.ToInt32(m_dpiX);
            }
        }

        public int? iDpiY
        {
            get
            {
                if (m_dpiY == null || m_dpiY.Trim().Length == 0)
                    return null;
                else
                    return Convert.ToInt32(m_dpiY);
            }
        }
        /// <summary>
        /// The last page of the report to render. No set default, but it should be the same value as StartPage
        /// </summary> 
        public string EndPage
        {
            get { return m_endPage; }
            set { m_endPage = value; }
        }
        /// <summary>
        /// The bottom margin value, in inches, to set for the report. You must include an integer or decimal value followed by "in" (for example, 1in). This value overrides the report's original settings.
        /// </summary> 
        public string MarginBottom
        {
            get { return m_marginBottom; }
            set { m_marginBottom = value; }
        }
        /// <summary>
        /// The left margin value, in inches, to set for the report. You must include an integer or decimal value followed by "in" (for example, 1in). This value overrides the report's original settings.
        /// </summary> 
        public string MarginLeft
        {
            get { return m_marginLeft; }
            set { m_marginLeft = value; }
        }
        /// <summary>
        /// The right margin value, in inches, to set for the report. You must include an integer or decimal value followed by "in" (for example, 1in). This value overrides the report's original settings.
        /// </summary> 
        public string MarginRight
        {
            get { return m_marginRight; }
            set { m_marginRight = value; }
        }
        /// <summary>
        /// The top margin value, in inches, to set for the report. You must include an integer or decimal value followed by "in" (for example, 1in). This value overrides the report's original settings.
        /// </summary> 
        public string MarginTop
        {
            get { return m_marginTop; }
            set { m_marginTop = value; }
        }
        /// <summary>
        /// One of the Graphics Device Interface (GDI) supported output formats: BMP, EMF, GIF, JPEG, PNG, or TIFF.
        /// </summary> 
        public string OutputFormat
        {
            get { return m_outputFormat; }
            set { m_outputFormat = value; }
        }
        /// <summary>
        /// The page height, in inches, to set for the report. You must include an integer or decimal value followed by "in" (for example, 11in). This value overrides the report's original settings.
        /// </summary> 
        public string PageHeight
        {
            get { return m_pageHeight; }
            set { m_pageHeight = value; }
        }
        /// <summary>
        /// The page width, in inches, to set for the report. You must include an integer or decimal value followed by "in" (for example, 8.5in). This value overrides the report's original settings.
        /// </summary> 
        public string PageWidth
        {
            get { return m_pageWidth; }
            set { m_pageWidth = value; }
        }
        /// <summary>
        /// The first page of the report to render. A value of 0 indicates that all pages are rendered. The default value is 1.
        /// </summary> 
        public string StartPage
        {
            get { return m_startPage; }
            set { m_startPage = value; }
        }
        #endregion

        #region Methods



        /// <summary>
        /// Builds Parameter Collection 
        /// </summary>
        public override NameValueCollection BuildParamCollection()
        {
            NameValueCollection paramCollection = new NameValueCollection();
            if (this.ColorDepth != null) paramCollection.Add("ColorDepth", this.ColorDepth);
            if (this.Columns != null) paramCollection.Add("Columns", this.Columns);
            if (this.ColumnSpacing != null) paramCollection.Add("ColumnSpacing", this.ColumnSpacing);
            if (this.DpiX != null) paramCollection.Add("DpiX", this.DpiX);
            if (this.DpiY != null) paramCollection.Add("DpiY", this.DpiY);
            if (this.PrintDpiX != null) paramCollection.Add("PrintDpiX", this.PrintDpiX);
            if (this.PrintDpiY != null) paramCollection.Add("PrintDpiY", this.PrintDpiY);
            if (this.EndPage != null) paramCollection.Add("EndPage", this.EndPage);
            if (this.MarginBottom != null) paramCollection.Add("MarginBottom", this.MarginBottom);
            if (this.MarginLeft != null) paramCollection.Add("MarginLeft", this.MarginLeft);
            if (this.MarginRight != null) paramCollection.Add("MarginRight", this.MarginRight);
            if (this.MarginTop != null) paramCollection.Add("MarginTop", this.MarginTop);
            if (this.OutputFormat != null) paramCollection.Add("OutputFormat", this.OutputFormat);
            if (this.PageHeight != null) paramCollection.Add("PageHeight", this.PageHeight);
            if (this.PageWidth != null) paramCollection.Add("PageWidth", this.PageWidth);
            if (this.StartPage != null) paramCollection.Add("StartPage", this.StartPage);
            return paramCollection;
        }

        /// <summary>
        /// Returns format
        /// </summary>
        /// <returns></returns>
        public string getRenderFormat()
        {
            return "IMAGE";
        }

        #endregion
    }

    public class EMFRender : IMAGERender, IRSMultiPageRenderStruct
    {
        public void SetCurrentPage(int PageNumber)
        {
            StartPage = EndPage = PageNumber.ToString();

        }

    }
    #endregion

    #region PPTXRender
    public sealed class PPTXRender : RSRenderStruct
    {
        #region Members

        private String m_colorDepth = null;
        private String m_columns = null;
        private String m_columnSpacing = null;
        private String m_dpiX = null;
        private String m_dpiY = null;
        private String m_endPage = null;
        private String m_marginBottom = null;
        private String m_marginLeft = null;
        private String m_marginRight = null;
        private String m_marginTop = null;
        private String m_outputFormat = null;
        private String m_pageHeight = null;
        private String m_pageWidth = null;
        private String m_startPage = null;
        private String m_useReportPageSize = null;
        private String m_printDpiX = null;
        private String m_printDpiY = null;


        #endregion

        #region Constructor

        /// <summary>
        /// PPTX Url Render. 17 DeviceInfo Props: ColorDepth, Columns, ColumnSpacing, DpiX, DpiY, EndPage, MarginBottom, MarginLeft, MarginRight, MarginTop, OutputFormat, PageHeight, PageWidth, UseReportPageSize, StartPage
        /// </summary>
        public PPTXRender()
            : base()
        {

        }

        #endregion

        #region Public Properties
        /// <summary>
        /// Return the format this RSRenderStruct outputs.
        /// </summary>
        public override string RenderFormat
        {
            get { return this.getRenderFormat(); }
        }

        /// <summary>
        /// The pixel depth of the color range supported by the image output. Valid values are 1, 4, 8, 24, and 32. The default value is 24. ColorDepth is only supported for TIFF rendering and is otherwise ignored by the report server for other image output formats.
        /// Note: For the 2005 release of SQL Server, the value of this setting is ignored, and the TIFF image is always rendered as 24-bit.
        /// </summary> 
        public string ColorDepth
        {
            get { return m_colorDepth; }
            set { m_colorDepth = value; }
        }
        /// <summary>
        /// The number of columns to set for the report. This value overrides the report's original settings.
        /// </summary> 
        public string Columns
        {
            get { return m_columns; }
            set { m_columns = value; }
        }
        /// <summary>
        /// The column spacing to set for the report. This value overrides the report's original settings.
        /// </summary> 
        public string ColumnSpacing
        {
            get { return m_columnSpacing; }
            set { m_colorDepth = value; }
        }
        /// <summary>
        /// The resolution of the output device in x-direction. The default value is 96.
        /// </summary> 
        public string DpiX
        {
            get { return m_dpiX; }
            set { m_dpiX = value; }
        }
        /// <summary>
        /// The resolution of the output device in y-direction. The default value is 96.
        /// </summary> 
        public string DpiY
        {
            get { return m_dpiY; }
            set { m_dpiY = value; }
        }
        /// <summary>
        /// The resolution of the output device in x-direction for EMF text. The default value is 120.
        /// </summary> 
        public string PrintDpiX
        {
            get { return m_printDpiX; }
            set { m_printDpiX = value; }
        }
        /// <summary>
        /// The resolution of the output device in y-direction for EMF text. The default value is 120.
        /// </summary> 
        public string PrintDpiY
        {
            get { return m_printDpiY; }
            set { m_printDpiY = value; }
        }

        public int? iPrintDpiX
        {
            get
            {
                if (m_printDpiX == null || m_printDpiX.Trim().Length == 0)
                    return null;
                else
                    return Convert.ToInt32(m_printDpiX);
            }
        }

        public int? iPrintDpiY
        {
            get
            {
                if (m_printDpiY == null || m_printDpiY.Trim().Length == 0)
                    return null;
                else
                    return Convert.ToInt32(m_printDpiY);
            }
        }

        public int? iDpiX
        {
            get
            {
                if (m_dpiX == null || m_dpiX.Trim().Length == 0)
                    return null;
                else
                    return Convert.ToInt32(m_dpiX);
            }
        }

        public int? iDpiY
        {
            get
            {
                if (m_dpiY == null || m_dpiY.Trim().Length == 0)
                    return null;
                else
                    return Convert.ToInt32(m_dpiY);
            }
        }
        /// <summary>
        /// The last page of the report to render. No set default, but it should be the same value as StartPage
        /// </summary> 
        public string EndPage
        {
            get { return m_endPage; }
            set { m_endPage = value; }
        }
        /// <summary>
        /// The bottom margin value, in inches, to set for the report. You must include an integer or decimal value followed by "in" (for example, 1in). This value overrides the report's original settings.
        /// </summary> 
        public string MarginBottom
        {
            get { return m_marginBottom; }
            set { m_marginBottom = value; }
        }
        /// <summary>
        /// The left margin value, in inches, to set for the report. You must include an integer or decimal value followed by "in" (for example, 1in). This value overrides the report's original settings.
        /// </summary> 
        public string MarginLeft
        {
            get { return m_marginLeft; }
            set { m_marginLeft = value; }
        }
        /// <summary>
        /// The right margin value, in inches, to set for the report. You must include an integer or decimal value followed by "in" (for example, 1in). This value overrides the report's original settings.
        /// </summary> 
        public string MarginRight
        {
            get { return m_marginRight; }
            set { m_marginRight = value; }
        }
        /// <summary>
        /// The top margin value, in inches, to set for the report. You must include an integer or decimal value followed by "in" (for example, 1in). This value overrides the report's original settings.
        /// </summary> 
        public string MarginTop
        {
            get { return m_marginTop; }
            set { m_marginTop = value; }
        }
        /// <summary>
        /// One of the Graphics Device Interface (GDI) supported output formats: BMP, EMF, GIF, JPEG, PNG, or TIFF.
        /// </summary> 
        public string OutputFormat
        {
            get { return m_outputFormat; }
            set { m_outputFormat = value; }
        }
        /// <summary>
        /// The page height, in inches, to set for the report. You must include an integer or decimal value followed by "in" (for example, 11in). This value overrides the report's original settings.
        /// </summary> 
        public string PageHeight
        {
            get { return m_pageHeight; }
            set { m_pageHeight = value; }
        }
        /// <summary>
        /// The page width, in inches, to set for the report. You must include an integer or decimal value followed by "in" (for example, 8.5in). This value overrides the report's original settings.
        /// </summary> 
        public string PageWidth
        {
            get { return m_pageWidth; }
            set { m_pageWidth = value; }
        }
        /// <summary>
        /// Determines if the reports page size should be used. If this value is set to false, the size will be determine by PowerPoint. If the value is true, the report's PageWidth/PageHeigth will be respected.
        /// </summary> 
        public string UseReportPageSize
        {
            get { return m_useReportPageSize; }
            set { m_useReportPageSize = value; }
        }
        /// <summary>
        /// The first page of the report to render. A value of 0 indicates that all pages are rendered. The default value is 1.
        /// </summary> 
        public string StartPage
        {
            get { return m_startPage; }
            set { m_startPage = value; }
        }
        #endregion

        #region Methods

        /// <summary>
        /// Builds Parameter Collection 
        /// </summary>
        public override NameValueCollection BuildParamCollection()
        {
            NameValueCollection paramCollection = new NameValueCollection();
            if (this.ColorDepth != null) paramCollection.Add("ColorDepth", this.ColorDepth);
            if (this.Columns != null) paramCollection.Add("Columns", this.Columns);
            if (this.ColumnSpacing != null) paramCollection.Add("ColumnSpacing", this.ColumnSpacing);
            if (this.DpiX != null) paramCollection.Add("DpiX", this.DpiX);
            if (this.DpiY != null) paramCollection.Add("DpiY", this.DpiY);
            if (this.PrintDpiX != null) paramCollection.Add("PrintDpiX", this.PrintDpiX);
            if (this.PrintDpiY != null) paramCollection.Add("PrintDpiY", this.PrintDpiY);
            if (this.EndPage != null) paramCollection.Add("EndPage", this.EndPage);
            if (this.MarginBottom != null) paramCollection.Add("MarginBottom", this.MarginBottom);
            if (this.MarginLeft != null) paramCollection.Add("MarginLeft", this.MarginLeft);
            if (this.MarginRight != null) paramCollection.Add("MarginRight", this.MarginRight);
            if (this.MarginTop != null) paramCollection.Add("MarginTop", this.MarginTop);
            if (this.OutputFormat != null) paramCollection.Add("OutputFormat", this.OutputFormat);
            if (this.PageHeight != null) paramCollection.Add("PageHeight", this.PageHeight);
            if (this.PageWidth != null) paramCollection.Add("PageWidth", this.PageWidth);
            if (this.UseReportPageSize != null) paramCollection.Add("UseReportPageSize", this.UseReportPageSize);
            if (this.StartPage != null) paramCollection.Add("StartPage", this.StartPage);
            return paramCollection;
        }

        /// <summary>
        /// Returns format
        /// </summary>
        /// <returns></returns>
        public string getRenderFormat()
        {
            return "PPTX";
        }

        #endregion
    }
    #endregion

    #region MHTMLRender
    public class MHTMLRender : HTMLRenderBase
    {
        #region Members

        private String m_javaScript = "false";
        private String m_MHTMLFragment = "false";

        #endregion

        #region Constructors

        /// <summary>
        /// MHTML Url Render. 2 DeviceInfo Props: JavaScript, MHTML Fragment
        /// </summary>
        public MHTMLRender()
            : base()
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Return the format this RSRenderStruct outputs.
        /// </summary>
        public override string RenderFormat
        {
            get { return this.getRenderFormat(); }
        }

        /// <summary>
        /// Indicates whether JavaScript is supported in the rendered report.
        /// </summary> 
        public String JavaScript
        {
            get { return m_javaScript; }
            set { m_javaScript = value; }
        }

        /// <summary>
        /// Indicates whether an MHTML fragment is created in place of a full MHTML document. An MHTML fragment includes the report content in a TABLE element and omits the HTML and BODY elements. The default value is false.
        /// </summary> 
        public String MHTMLFragment
        {
            get { return m_MHTMLFragment; }
            set { m_MHTMLFragment = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Builds Parameter Collection 
        /// </summary>
        public override NameValueCollection BuildParamCollection()
        {
            NameValueCollection paramCollection = new NameValueCollection();
            if (this.JavaScript != null) paramCollection.Add("JavaScript", this.JavaScript);
            if (this.MHTMLFragment != null) paramCollection.Add("HTMLFragment", this.MHTMLFragment);    // Making it as HTMLFragment until PS BUG #487103 got fixed
            if (this.Section != null) paramCollection.Add("Section", this.Section);

            return paramCollection;
        }

        /// <summary>
        /// Returns format
        /// </summary>
        /// <returns></returns>
        public string getRenderFormat()
        {
            return "MHTML";
        }

        #endregion
    }
    #endregion

    #region PDFRender
    public class PDFRender : RSRenderStruct
    {
        #region Member Variables

        private String m_columns = null;
        private String m_columnSpacing = null;
        private String m_endPage = null;
        private String m_marginBottom = null;
        private String m_marginLeft = null;
        private String m_marginRight = null;
        private String m_marginTop = null;
        private String m_pageHeight = null;
        private String m_pageWidth = null;
        private String m_startPage = "0";
        private String m_humanReadablePDF = "false";
        private String m_embedFonts = null;


        #endregion

        #region Constructors

        /// <summary>
        /// PDF Url Render. 10 DeviceInfo Props: Columns, ColumnSpacing, EndPage, MarginBottom, MarginLeft, MarginRight, MarginTop, PageHeight, PageWidth, StartPage
        /// </summary>
        public PDFRender()
            : base()
        {

        }

        #endregion

        #region Public Properties
        /// <summary>
        /// Return the format this RSRenderStruct outputs.
        /// </summary>
        public override string RenderFormat
        {
            get { return this.getRenderFormat(); }
        }
        /// <summary>
        /// The number of columns to set for the report. This value overrides the report's original settings.
        /// </summary> 
        public String Columns
        {
            get { return m_columns; }
            set { m_columns = value; }
        }
        /// <summary>
        /// The column spacing to set for the report. This value overrides the report's original settings.
        /// </summary> 
        public String ColumnSpacing
        {
            get { return m_columnSpacing; }
            set { m_columnSpacing = value; }
        }
        /// <summary>
        /// The last page of the report to render. The default value is the value for StartPage.
        /// </summary> 
        public String EndPage
        {
            get { return m_endPage; }
            set { m_endPage = value; }
        }
        /// <summary>
        /// The bottom margin value, in inches, to set for the report. You must include an integer or decimal value followed by "in" (for example, 1in). This value overrides the report's original settings.
        /// </summary> 
        public String MarginBottom
        {
            get { return m_marginBottom; }
            set { m_marginBottom = value; }
        }
        /// <summary>
        /// The left margin value, in inches, to set for the report. You must include an integer or decimal value followed by "in" (for example, 1in). This value overrides the report's original settings.
        /// </summary> 
        public String MarginLeft
        {
            get { return m_marginLeft; }
            set { m_marginLeft = value; }
        }
        /// <summary>
        /// The right margin value, in inches, to set for the report. You must include an integer or decimal value followed by "in" (for example, 1in). This value overrides the report's original settings.
        /// </summary> 
        public String MarginRight
        {
            get { return m_marginRight; }
            set { m_marginRight = value; }
        }
        /// <summary>
        /// The top margin value, in inches, to set for the report. You must include an integer or decimal value followed by "in" (for example, 1in). This value overrides the report's original settings.
        /// </summary> 
        public String MarginTop
        {
            get { return m_marginTop; }
            set { m_marginTop = value; }
        }
        /// <summary>
        /// The page height, in inches, to set for the report. You must include an integer or decimal value followed by "in" (for example, 11in). This value overrides the report's original settings.
        /// </summary> 
        public String PageHeight
        {
            get { return m_pageHeight; }
            set { m_pageHeight = value; }
        }
        /// <summary>
        /// The page width, in inches, to set for the report. You must include an integer or decimal value followed by "in" (for example, 8.5in). This value overrides the report's original settings.
        /// </summary> 
        public String PageWidth
        {
            get { return m_pageWidth; }
            set { m_pageWidth = value; }
        }
        /// <summary>
        /// The first page of the report to render. A value of 0 indicates that all pages are rendered. The default value is 1.
        /// </summary> 
        public String StartPage
        {
            get { return m_startPage; }
            set { m_startPage = value; }
        }

        /// <summary>
        /// True indicates that the PDF should not be compressed and should be internally formatted to make the source more readable.  False indicates that the source will be compressed.
        /// </summary> 
        public String HumanReadablePDF
        {
            get { return m_humanReadablePDF; }
            set { m_humanReadablePDF = value; }
        }

        /// <summary>
        /// None indicates that fonts should not be embedded. Subset indicates that a subset of the fonts (only the characters used) should be embedded.
        /// </summary> 
        public String EmbedFonts
        {
            get { return m_embedFonts; }
            set { m_embedFonts = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Builds Parameter Collection 
        /// </summary>
        public override NameValueCollection BuildParamCollection()
        {
            NameValueCollection paramCollection = new NameValueCollection();
            if (this.Columns != null) paramCollection.Add("Columns", this.Columns);
            if (this.ColumnSpacing != null) paramCollection.Add("ColumnSpacing", this.ColumnSpacing);
            if (this.EndPage != null) paramCollection.Add("EndPage", this.EndPage);
            if (this.MarginBottom != null) paramCollection.Add("MarginBottom", this.MarginBottom);
            if (this.MarginLeft != null) paramCollection.Add("MarginLeft", this.MarginLeft);
            if (this.MarginRight != null) paramCollection.Add("MarginRight", this.MarginRight);
            if (this.MarginTop != null) paramCollection.Add("MarginTop", this.MarginTop);
            if (this.PageHeight != null) paramCollection.Add("PageHeight", this.PageHeight);
            if (this.PageWidth != null) paramCollection.Add("PageWidth", this.PageWidth);
            if (this.StartPage != null) paramCollection.Add("StartPage", this.StartPage);
            if (this.HumanReadablePDF != null) paramCollection.Add("HumanReadablePDF", this.HumanReadablePDF);
            if (this.EmbedFonts != null) paramCollection.Add("EmbedFonts", this.EmbedFonts);
            return paramCollection;
        }

        /// <summary>
        /// Returns format
        /// </summary>
        /// <returns></returns>
        public string getRenderFormat()
        {
            return "PDF";
        }

        #endregion
    }
    #endregion

    #region XMLRender
    public class XMLRender : RSRenderStruct
    {
        #region Members

        private String m_schema = "false";
        private String m_extension = "XML";
        private String m_omitschema = "false";
        private String m_omitnamespace = "false";
        private String m_useformattedvalues = "true";
        private String m_indented = "false";
        private String m_xslt = null;
        private String m_encoding = "UTF-8";
        private String m_mimetype = "text/xml";

        #endregion

        #region Constructors
        ///<summary>
        /// XML Url Render. 8 DeviceInfo Props: XSLT, MIMEType, UseFormattedValues, Indented, OmitSchema, Encoding, FileExtension, Schema
        ///</summary>
        public XMLRender()
            : base()
        {

        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Return the format this RSRenderStruct outputs.
        /// </summary>
        public override string RenderFormat
        {
            get { return this.getRenderFormat(); }
        }

        /// <summary>
        /// The path in the report server namespace of an XSLT to apply to the XML file, for example /Transforms/myxslt. The xsl file must be a published resource on the report server and you must access it through a report server item path. The value of this setting is applied after any XSLT that is specified in the report. If the XSLT setting is applied, the OmitSchema setting is ignored.
        /// </summary>          
        public String Xslt
        {
            get { return m_xslt; }
            set { m_xslt = value; }
        }

        /// <summary>
        /// The Multipurpose Internet Mail Extensions (MIME) type of the XML file.
        /// </summary> 
        public String Mimetype
        {
            get { return m_mimetype; }
            set { m_mimetype = value; }
        }

        /// <summary>
        /// Indicates whether to render the formatted value of a text box when generating the XML data. A value of false indicates that the underlying value of the text box is used.
        /// </summary> 
        public String Useformattedvalues
        {
            get { return m_useformattedvalues; }
            set { m_useformattedvalues = value; }
        }

        /// <summary>
        /// Indicates whether to generate indented XML. The default value of false generates non-indented, compressed XML.
        /// </summary> 
        public String Indented
        {
            get { return m_indented; }
            set { m_indented = value; }
        }

        /// <summary>
        /// Indicates whether to omit the schema name from the XML and to omit an XML schema. The default value is false.
        /// </summary> 
        public String Omitschema
        {
            get { return m_omitschema; }
            set { m_omitschema = value; }
        }

        /// <summary>
        /// Indicates whether to omit the namespace (xmlns) name from the XML. The default value is false.
        /// </summary> 
        public String Omitnamespace
        {
            get { return m_omitnamespace; }
            set { m_omitnamespace = value; }
        }

        /// <summary>
        /// One of the character encoding schemas: ASCII, UTF-8, or Unicode. The default value is UTF-8.
        /// </summary> 
        public String Encoding
        {
            get { return m_encoding; }
            set { m_encoding = value; }
        }

        /// <summary>
        /// The file extension to use for the generated file.
        /// </summary> 
        public String Extension
        {
            get { return m_extension; }
            set { m_extension = value; }
        }

        /// <summary>
        /// Indicates whether the XML schema definition (XSD) is rendered or whether the actual XML data is rendered. A value of true indicates that an XML schema is rendered. The default value is false.
        /// </summary> 
        public String Schema
        {
            get { return m_schema; }
            set { m_schema = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Builds Parameter Collection 
        /// </summary>
        public override NameValueCollection BuildParamCollection()
        {
            NameValueCollection paramCollection = new NameValueCollection();
            if (this.Schema != null) paramCollection.Add("Schema", this.Schema);
            if (this.Encoding != null) paramCollection.Add("Encoding", this.Encoding);
            if (this.Useformattedvalues != null) paramCollection.Add("UseFormattedValues", this.Useformattedvalues);
            if (this.Omitschema != null) paramCollection.Add("OmitSchema", this.Omitschema);
            if (this.Omitnamespace != null) paramCollection.Add("Omitnamespace", this.Omitnamespace);
            if (this.Extension != null) paramCollection.Add("FileExtension", this.Extension);
            if (this.Indented != null) paramCollection.Add("Indented", this.Indented);
            if (this.Mimetype != null) paramCollection.Add("MIMEType", this.Mimetype);
            if (this.Xslt != null) paramCollection.Add("XSLT", this.Xslt);
            return paramCollection;
        }
        /// <summary>
        /// Returns format
        /// </summary>
        /// <returns></returns>
        public string getRenderFormat()
        {
            return "XML";
        }

        #endregion
    }
    #endregion

    #region RGDIRender
    public class RGDIRender : RSRenderStruct, IRSMultiPageRenderStruct
    {
        #region Members

        string _rgdiVersion = null;
        string _page = null;
        string _autoClose = null;

        #endregion

        #region Constructor

        /// <summary>
        /// RGDI Url Render. 2 DeviceInfo Props: RGDIVersion & Page
        /// </summary>
        public RGDIRender()
            : base()
        {
        }

        #endregion

        #region Public Properties
        /// <summary>
        /// Return the format this RSRenderStruct outputs.
        /// </summary>
        public override string RenderFormat
        {
            get { return "RGDI"; }
        }

        /// <summary>
        /// Gets/Sets the version of RGDI that will be renderred
        /// </summary>
        public string RGDIVersion
        {
            get { return _rgdiVersion; }
            set { _rgdiVersion = value; }
        }

        /// <summary>
        /// Gets/Sets the page that will be renderred
        /// </summary>
        public string Page
        {
            get { return _page; }
            set { _page = value; }
        }

        /// <summary>
        /// Gets/Sets a debug setting for auto-closing the WinForm control
        /// </summary>
        public string AutoClose
        {
            get { return _autoClose; }
            set { _autoClose = value; }
        }


        #endregion

        #region Methods

        public void SetCurrentPage(int PageNumber)
        {
            Page = PageNumber.ToString();
        }

        /// <summary>
        /// Builds Parameter Collection 
        /// </summary>
        public override NameValueCollection BuildParamCollection()
        {
            NameValueCollection paramCollection = new NameValueCollection();

            if (_rgdiVersion != null)
                paramCollection.Add("RgdiVersion", this.RGDIVersion);

            if (_page != null)
                paramCollection.Add("Page", this._page);

            if (AutoClose != null)
                paramCollection.Add("AutoClose", this.AutoClose);

            return paramCollection;
        }

        #endregion
    }
    #endregion

    #region GDIStaticRender
    public class GDIStaticRender : GDIRender
    {
        public override string RenderFormat
        {
            get { return "GDIStatic"; }
        }
    }
    #endregion
    #region GDIRender - Custom RSRenderStruct for use only with RVC_GDI.MetafileControlAccessor
    public class GDIRender : RSRenderStruct, IRSMultiPageRenderStruct
    {
        #region Members

        string _localMode = null;
        string _page = null;
        string _zoom = null;
        string _autoClose = null;

        #endregion

        #region Public Properties
        /// <summary>
        /// Return the format this RSRenderStruct outputs.
        /// </summary>
        public override string RenderFormat
        {
            get { return "GDI"; }
        }

        /// <summary>
        /// Gets/Sets the version of RGDI that will be renderred
        /// </summary>
        public string LocalMode
        {
            get { return _localMode; }
            set { _localMode = value; }
        }

        /// <summary>
        /// Gets/Sets the page that will be renderred
        /// </summary>
        public string Page
        {
            get { return _page; }
            set { _page = value; }
        }

        /// <summary>
        /// Gets/Sets the Zoom value of the WinForm control
        /// </summary>
        public string Zoom
        {
            get { return _zoom; }
            set { _zoom = value; }
        }

        /// <summary>
        /// Gets/Sets a debug setting for auto-closing the WinForm control
        /// </summary>
        public string AutoClose
        {
            get { return _autoClose; }
            set { _autoClose = value; }
        }

        #endregion

        #region Methods

        public void SetCurrentPage(int PageNumber)
        {
            _page = PageNumber.ToString();
        }

        /// <summary>
        /// Builds Parameter Collection 
        /// </summary>
        public override NameValueCollection BuildParamCollection()
        {
            NameValueCollection paramCollection = new NameValueCollection();

            if (LocalMode != null)
                paramCollection.Add("LocalMode", this.LocalMode);

            if (Page != null)
                paramCollection.Add("Page", this.Page);

            if (Zoom != null)
                paramCollection.Add("Zoom", this.Zoom);

            if (AutoClose != null)
                paramCollection.Add("AutoClose", this.AutoClose);

            return paramCollection;
        }

        #endregion
    }
    #endregion

    #region RPLRender
    public class RPLRender : RSRenderStruct
    {

        #region Members

        #region CommonRPLProperties

        private String m_startPage = null;
        private String m_endPage = null;
        private String m_pageHeight = null;
        private bool m_serializeAsXml = false;
        private bool m_omitPageContent = false;
        private string m_useEmSquare = null;
        private string m_measureTextDPI = null;
        private string m_measureImageDpiX = null;
        private string m_measureImageDpiY = null;
        private string m_DpiX = null;
        private string m_DpiY = null;

        #endregion

        #region SPBProperties

        private String m_measureItems = null;
        private String m_emfDynamicImage = null;
        private String m_toggleItems = null;
        private String m_useInteractiveHeight = null;
        private String m_memoryStream = null;
        private String m_paginationType = null;
        private String m_returnType = null;
        private String m_generateRPLObject = null;
        private String m_addOriginalValues = null;
        private String m_secondaryStreams = null;
        private String m_streamNames = null;

        #endregion

        #region HPBProperties

        private string m_pageWidth = null;
        private string m_columns = null;
        private string m_columnSpacing = null;
        private string m_marginTop = null;
        private string m_marginLeft = null;
        private string m_marginBottom = null;
        private string m_marginRight = null;

        #endregion

        #endregion
        #region Constructors
        ///<summary>
        /// RPL Render Device Info
        ///</summary>
        public RPLRender()
            : base()
        {

        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Return the format this RSRenderStruct outputs.
        /// </summary>
        public override string RenderFormat
        {
            get { return this.getRenderFormat(); }
        }

        /// <summary>
        /// 
        /// </summary>
        public String StartPage
        {
            get { return m_startPage; }
            set { m_startPage = value; }

        }

        /// <summary>
        /// 
        /// </summary>
        public String EndPage
        {
            get { return m_endPage; }
            set { m_endPage = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public String MeasureItems
        {
            get { return m_measureItems; }
            set { m_measureItems = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public String EmfDynamicImage
        {
            get { return m_emfDynamicImage; }
            set { m_emfDynamicImage = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public String ToggleItems
        {
            get { return m_toggleItems; }
            set { m_toggleItems = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public String PageHeight
        {
            get { return m_pageHeight; }
            set { m_pageHeight = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public String MemoryStream
        {
            get { return m_memoryStream; }
            set { m_memoryStream = value; }

        }

        /// <summary>
        /// 
        /// </summary>
        public String PaginationType
        {
            get { return m_paginationType; }
            set { m_paginationType = value; }

        }

        /// <summary>
        /// 
        /// </summary>
        public String ReturnType
        {
            get { return m_returnType; }
            set { m_returnType = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public String GenerateRPLObject
        {
            get { return m_generateRPLObject; }
            set { m_generateRPLObject = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public String AddOriginalValues
        {
            get { return m_addOriginalValues; }
            set { m_addOriginalValues = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public String UseInteractiveHeight
        {
            get { return m_useInteractiveHeight; }
            set { m_useInteractiveHeight = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public String SecondaryStreams
        {
            get { return m_secondaryStreams; }
            set { m_secondaryStreams = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public String StreamNames
        {
            get { return m_streamNames; }
            set { m_streamNames = value; }
        }

        /// <summary>
        /// Toggles between binary and xml serialization
        /// </summary>
        public bool SerializeAsXml
        {
            get { return m_serializeAsXml; }
            set { m_serializeAsXml = value; }
        }

        public bool OmitPageContent
        {
            get { return m_omitPageContent; }
            set { m_omitPageContent = value; }
        }

        public string PageWidth
        {
            get { return m_pageWidth; }
            set { m_pageWidth = value; }
        }

        public string Columns
        {
            get { return m_columns; }
            set { m_columns = value; }
        }

        public string ColumnSpacing
        {
            get { return m_columnSpacing; }
            set { m_columnSpacing = value; }
        }

        public string MarginTop
        {
            get { return m_marginTop; }
            set { m_marginTop = value; }
        }

        public string MarginLeft
        {
            get { return m_marginLeft; }
            set { m_marginLeft = value; }
        }

        public string MarginBottom
        {
            get { return m_marginBottom; }
            set { m_marginBottom = value; }
        }

        public string MarginRight
        {
            get { return m_marginRight; }
            set { m_marginRight = value; }
        }

        public string UseEmSquare
        {
            get { return m_useEmSquare; }
            set { m_useEmSquare = value; }
        }

        public string MeasureTextDPI
        {
            get { return m_measureTextDPI; }
            set { m_measureTextDPI = value; }
        }
        public string MeasureImageDpiX
        {
            get { return m_measureImageDpiX; }
            set { m_measureImageDpiX = value; }
        }

        public string MeasureImageDpiY
        {
            get { return m_measureImageDpiY; }
            set { m_measureImageDpiY = value; }
        }

        public string DpiX
        {
            get { return m_DpiX; }
            set { m_DpiX = value; }
        }

        public string DpiY
        {
            get { return m_DpiY; }
            set { m_DpiY = value; }
        }
        #endregion

        #region Methods

        /// <summary>
        /// Builds Parameter Collection 
        /// </summary>
        public override NameValueCollection BuildParamCollection()
        {
            NameValueCollection paramCollection = new NameValueCollection();

            if (this.StartPage != null) paramCollection.Add("StartPage", this.StartPage);
            if (this.EndPage != null) paramCollection.Add("EndPage", this.EndPage);
            if (this.MeasureItems != null) paramCollection.Add("MeasureItems", this.MeasureItems);
            if (this.EmfDynamicImage != null) paramCollection.Add("EmfDynamicImage", this.EmfDynamicImage);
            if (this.ToggleItems != null) paramCollection.Add("ToggleItems", this.ToggleItems);
            if (this.MemoryStream != null) paramCollection.Add("MemoryStream", this.MemoryStream);
            if (this.PaginationType != null) paramCollection.Add("PaginationType", this.PaginationType);
            if (this.ReturnType != null) paramCollection.Add("ReturnType", this.ReturnType);
            if (this.GenerateRPLObject != null) paramCollection.Add("GenerateRPLObject", this.GenerateRPLObject);
            if (this.AddOriginalValues != null) paramCollection.Add("AddOriginalValues", this.AddOriginalValues);
            if (this.PageHeight != null) paramCollection.Add("PageHeight", this.PageHeight);
            if (this.UseInteractiveHeight != null) paramCollection.Add("UseInteractiveHeight", this.UseInteractiveHeight);
            if (this.SecondaryStreams != null) paramCollection.Add("SecondaryStreams", this.SecondaryStreams);
            if (this.StreamNames != null) paramCollection.Add("StreamNames", this.StreamNames);
            if (this.PageWidth != null) paramCollection.Add("PageWidth", this.PageWidth);
            if (this.Columns != null) paramCollection.Add("Columns", this.Columns);
            if (this.ColumnSpacing != null) paramCollection.Add("ColumnSpacing", this.ColumnSpacing);
            if (this.MarginTop != null) paramCollection.Add("MarginTop", this.MarginTop);
            if (this.MarginLeft != null) paramCollection.Add("MarginLeft", this.MarginLeft);
            if (this.MarginBottom != null) paramCollection.Add("MarginBottom", this.MarginBottom);
            if (this.MarginRight != null) paramCollection.Add("MarginRight", this.MarginRight);
            if (this.UseEmSquare != null) paramCollection.Add("UseEmSquare", this.UseEmSquare);
            if (this.MeasureTextDPI != null) paramCollection.Add("MeasureTextDPI", this.MeasureTextDPI);
            if (this.MeasureImageDpiX != null) paramCollection.Add("MeasureImageDpiX", this.MeasureImageDpiX);
            if (this.MeasureImageDpiY != null) paramCollection.Add("MeasureImageDpiX", this.MeasureImageDpiY);
            if (this.DpiX != null) paramCollection.Add("DpiX", this.DpiX);
            if (this.DpiY != null) paramCollection.Add("DpiX", this.DpiY);



            paramCollection.Add("SerializeAsXml", this.SerializeAsXml.ToString());
            paramCollection.Add("OmitPageContent", this.OmitPageContent.ToString());

            return paramCollection;
        }
        /// <summary>
        /// Returns format
        /// </summary>
        /// <returns></returns>
        public string getRenderFormat()
        {
            return "TestRPL";
        }

        #endregion
    }
    #endregion

    #region ROMRender
    public class ROMRender : RSRenderStruct
    {
        #region Constructors
        ///<summary>
        /// RPL Render Device Info
        ///</summary>
        public ROMRender()
            : base()
        {

        }
        #endregion

        #region Properties

        private bool m_serializeAsXml = false;
        private ROMTraverseVersion? m_traverseVersion = null;

        #endregion


        #region Public Properties
        /// <summary>
        /// Return the format this RSRenderStruct outputs.
        /// </summary>
        public override string RenderFormat
        {
            get { return this.getRenderFormat(); }
        }

        /// <summary>
        /// Toggles between binary and xml serialization
        /// </summary>
        public bool SerializeAsXml
        {
            get { return m_serializeAsXml; }
            set { m_serializeAsXml = value; }
        }

        public ROMTraverseVersion? TraverseVersion
        {
            get { return m_traverseVersion; }
            set { m_traverseVersion = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Builds Parameter Collection 
        /// </summary>
        public override NameValueCollection BuildParamCollection()
        {
            NameValueCollection paramCollection = new NameValueCollection();
            paramCollection.Add("SerializeAsXml", this.SerializeAsXml.ToString());

            if (m_traverseVersion.HasValue)
            {
                paramCollection.Add("ROMTraverseVersion",
                    Enum.GetName(typeof(ROMTraverseVersion), m_traverseVersion.Value));
            }

            return paramCollection;

        }
        /// <summary>
        /// Returns format
        /// </summary>
        /// <returns></returns>
        public string getRenderFormat()
        {
            return "ROM";
        }

        #endregion

        #region Enums

        /// <summary>
        /// Available device info values used to determine how the
        /// ROM is traversed.  Used for back-compat testing.
        /// </summary>
        public enum ROMTraverseVersion
        {
            // Traverse ROM using latest properties
            Current,
            // Traverse ROM using only RS2008 RTM properties/methods
            RS2008RTM,
            // always add new items at end
        }

        #endregion
    }
    #endregion

    #region WORDRender
    public class WORDRender : RSRenderStruct
    {
        #region Members

        private string m_autoFit = null;
        private string m_expandToggles = "false";
        private string m_fixedPageWidth = "false";
        private string m_omitHyperlinks = "false";
        private string m_omitDrillthroughs = "false";

        #endregion

        #region Constructor

        /// <summary>
        /// WORDR Url Render. 5 DeviceInfo Props: autoFit,expandToggles,fixedPageWidth,omitHyperlinks,omitDrillthroughs
        /// </summary>
        public WORDRender()
            : base()
        {

        }

        #endregion

        #region Public Properties
        /// <summary>
        /// Return the format this RSRenderStruct outputs.
        /// </summary>
        public override string RenderFormat
        {
            get { return this.getRenderFormat(); }
        }

        public string AutoFit
        {
            get { return m_autoFit; }
            set { m_autoFit = value; }
        }

        public string ExpandToggles
        {
            get { return m_expandToggles; }
            set { m_expandToggles = value; }
        }

        public string FixedPageWidth
        {
            get { return m_fixedPageWidth; }
            set { m_fixedPageWidth = value; }
        }

        public string OmitHyperlinks
        {
            get { return m_omitHyperlinks; }
            set { m_omitHyperlinks = value; }
        }

        public string OmitDrillthroughs
        {
            get { return m_omitDrillthroughs; }
            set { m_omitDrillthroughs = value; }
        }


        #endregion

        #region Methods

        /// <summary>
        /// Builds Parameter Collection 
        /// </summary>
        public override NameValueCollection BuildParamCollection()
        {
            NameValueCollection paramCollection = new NameValueCollection();
            if (this.AutoFit != null) paramCollection.Add("autoFit", this.AutoFit);
            if (this.ExpandToggles != null) paramCollection.Add("expandToggles", this.ExpandToggles);
            if (this.FixedPageWidth != null) paramCollection.Add("fixedPageWidth", this.FixedPageWidth);
            if (this.OmitHyperlinks != null) paramCollection.Add("omitHyperlinks", this.OmitHyperlinks);
            if (this.OmitDrillthroughs != null) paramCollection.Add("omitDrillthroughs", this.OmitDrillthroughs);
            return paramCollection;
        }

        /// <summary>
        /// Returns format
        /// </summary>
        /// <returns></returns>
        public virtual string getRenderFormat()
        {
            return "WORD";
        }

        #endregion
    }
    #endregion

    #region WORDOPENXMLRender
    public class WORDOPENXMLRender : WORDRender
    {
        #region Constructor

        /// <summary>
        /// WORDR Url Render. 5 DeviceInfo Props: autoFit,expandToggles,fixedPageWidth,omitHyperlinks,omitDrillthroughs
        /// </summary>
        public WORDOPENXMLRender()
            : base()
        {

        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns format
        /// </summary>
        /// <returns></returns>
        public override string getRenderFormat()
        {
            return "WORDOPENXML";
        }

        #endregion
    }
    #endregion

    #region RPDSRender
    public class RPDSRender : RSRenderStruct
    {
        #region Member Variables

        private String m_segmentationRequest = null;

        #endregion

        #region Constructors

        /// <summary>
        /// DSR Url Render
        /// </summary>
        public RPDSRender()
            : base()
        {

        }

        #endregion

        #region Public Properties
        /// <summary>
        /// Return the format this RSRenderStruct outputs.
        /// </summary>
        public override string RenderFormat
        {
            get { return this.getRenderFormat(); }
        }
        /// <summary>
        /// The number of columns to set for the report. This value overrides the report's original settings.
        /// </summary> 
        public String SegmentationRequest
        {
            get { return m_segmentationRequest; }
            set { m_segmentationRequest = value; }
        }
        /// <summary>
        /// The segmentation request to set for the report. This value overrides the report's original settings.
        /// </summary> 

        #endregion

        #region Methods

        /// <summary>
        /// Builds Parameter Collection 
        /// </summary>
        public override NameValueCollection BuildParamCollection()
        {
            NameValueCollection paramCollection = new NameValueCollection();
            if (this.SegmentationRequest != null) paramCollection.Add("SegmentationRequest", this.SegmentationRequest);
            return paramCollection;
        }

        /// <summary>
        /// Returns format
        /// </summary>
        /// <returns></returns>
        public string getRenderFormat()
        {
            return "TestRPDS";
        }

        #endregion
    }
    #endregion

    
}
