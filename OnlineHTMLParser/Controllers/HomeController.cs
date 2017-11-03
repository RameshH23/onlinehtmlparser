using Microsoft.CSharp;
using Microsoft.Web.WebPages.OAuth;
using OnlineHTMLParser.Parser;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebMatrix.WebData;

namespace OnlineHTMLParser.Controllers
{
    public class HomeController : Controller
    {
        /* Home Controller */

        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            return View();
        }

        public ActionResult About()
        {
            try {
            // Create an instance of StreamReader to read from a file.
            // The using statement also closes the StreamReader.
                using (StreamReader sr = new StreamReader("C:/Users/muthu/Desktop/Test.txt"))
                {
               string line;

               // Read and display lines from the file until 
               // the end of the file is reached. 
               while ((line = sr.ReadLine()) != null)
               {
                   if (line.Contains("<img"))
                   {
                       int start=line.IndexOf("title")+7;
                       int end=line.IndexOf('"',start);
                       string s=line.Substring(start,end-start);
                       Console.Write(s);
                   }
               }
            }
               
         } catch (Exception e) {
            // Let the user know what went wrong.
            Console.WriteLine("The file could not be read:");
            Console.WriteLine(e.Message);
         }
         
      
            return View();
        }
        public ActionResult Test()
        {
            return View();
        }
        public ActionResult AjaxFinder(string parsefolder = null, string type = "html")
        {
             ViewBag.parsefolder = parsefolder;
            try
            {
                 DirectoryInfo directory = new DirectoryInfo(parsefolder);
                    var filess = directory.GetFiles().ToList();
                    string line = "";
                   
                    IList<FormData> forms = new List<FormData>();
                    foreach (var f in filess)
                    {
                       
                        IList<FormTag> tags = new List<FormTag>();
                       
                        System.IO.StreamReader file = new System.IO.StreamReader(parsefolder + f.Name);
                        int lineno = 1;
                        
                        string append = "";
                     

                        while ((line = file.ReadLine()) != null)
                        {
                         
                            if (line.Contains("background:url("))
                            {
                                append = line + Environment.NewLine;
                                tags.Add(new FormTag { Tags = append, linenumber = lineno });
                            }


                            lineno++;
                        }
                        forms.Add(new FormData { FileName = f.Name, Extension = f.Extension, FileLocation = f.FullName, TotalLine = lineno,Tagsdata=tags });
                    }
                    ViewData["Forms"] = forms;
            }
                catch(Exception e)
            {
                }
            return View();
        }
        public ActionResult AjaxExport(string parsefolder = null, string type = "html")
        {

            var products = new System.Data.DataTable("Parser");
            products.Columns.Add("Name", typeof(string));
            products.Columns.Add("Location", typeof(string));
            products.Columns.Add("Extention", typeof(string));
            products.Columns.Add("Tags", typeof(string));
            products.Columns.Add("Line", typeof(string));
            products.Columns.Add("Original MarkUp", typeof(string));
            ViewBag.parsefolder = parsefolder;
            try
            {
                DirectoryInfo directory = new DirectoryInfo(parsefolder);
                var filess = directory.GetFiles().ToList();
                string line = "";

                IList<FormData> forms = new List<FormData>();
                foreach (var f in filess)
                {

                    IList<FormTag> tags = new List<FormTag>();

                    System.IO.StreamReader file = new System.IO.StreamReader(parsefolder + f.Name);
                    int lineno = 1;

                    string append = "";


                    while ((line = file.ReadLine()) != null)
                    {

                        if (line.Contains("background:url("))
                        {
                            append = line + Environment.NewLine;
                            tags.Add(new FormTag { Tags = append, linenumber = lineno });
                        
                        
                            products.Rows.Add(f.Name, f.FullName, f.Extension, "Form", lineno, line);
                   
                        }
                        lineno++;
                    }

                }
                ViewData["Forms"] = forms;
            }
            catch (Exception e)
            {
            }
            var grid = new GridView();

            products.Rows.Add("Not Found");

            grid.DataSource = products;
            grid.DataBind();

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=BackgroundImageParserResult.xls");
            Response.ContentType = "application/ms-excel";

            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            grid.RenderControl(htw);

            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return View("MyView");
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult ParseUrl()
        {
         /*   if (User.Identity.Name == "")
            {

                return RedirectToAction("Login", "Account");
            } */
            return View();
        }
        public ActionResult HtmlParser(string parsefolder = null, string type = "html")
        {
            ViewBag.parsefolder = parsefolder;
          /*  if (User.Identity.Name == "")
            {

                return RedirectToAction("Login","Account");
            } */
            ViewBag.Error="";
            if (parsefolder != null || parsefolder != "")
            {
                ViewBag.Type = type;
                IList<Files> files = new List<Files>();
           try
                { 
                    DirectoryInfo directory = new DirectoryInfo(parsefolder);
                    var filess = directory.GetFiles().ToList();
                    string FullContent = "";
                    foreach (var f in filess)
                    {


                        string html;

                        var fileStream = new FileStream(parsefolder + f.Name, FileMode.Open, FileAccess.Read);

                        using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                        {
                            html = streamReader.ReadToEnd();

                        }


                        HtmlParser parse = new HtmlParser(html, parsefolder + f.Name);

                        Tags t = new Tags(type);
                           var tags = t.tags.ToList();
                       
                        var loop = 0;
                        IList<TagsData> TagsDataIn = new List<TagsData>();
                        foreach (var ta in tags)
                        {
                            string Line = "";
                            HtmlTag tag = new HtmlTag();
                            IList<AttributeData> ad = new List<AttributeData>(); ;
                            IList<Lines> LineStrings = new List<Lines>();
                            IList<string> LineNo = new List<string>();
                            while (parse.ParseNext(ta, out tag))
                            {


                                foreach (var attribute in t.attributes.ToList())
                                {

                                    string value;
                                    if (tag.Attributes.TryGetValue(attribute, out value))
                                    {
                                        ad.Add(new AttributeData() { Name = attribute, Value = value });
                                    }

                                }

                            }
                            parse.Reset();
                            int counter = 1;
                            string line;

                            // Read the file and display it line by line.
                            System.IO.StreamReader file = new System.IO.StreamReader(parsefolder+f.Name);
                            while ((line = file.ReadLine()) != null)
                            {
                                /* Conditions */

                             

                                if (type == "html")
                                {
                                    if (line.Contains("<" + ta) && (line.Contains("</" + ta + ">") || line.Contains("/>")))
                                    {
                                        IList<AttributeData> attri = new List<AttributeData>();
                                        Console.WriteLine(counter.ToString() + ": " + line);
                                        foreach (var at in t.attributes)
                                        {
                                            string dynamicline = line;
                                            int startfinder = dynamicline.IndexOf("<" + ta);
                                            int endfinder = (dynamicline.IndexOf("</" + ta + ">", startfinder) < 0 ? dynamicline.IndexOf("/>", startfinder) : dynamicline.IndexOf("</" + ta + ">", startfinder));
                                            if (endfinder > 0)
                                            {
                                                dynamicline = dynamicline.Substring(startfinder, endfinder - startfinder);
                                                string attribute = at + "=" + '"';
                                                var attr = dynamicline.IndexOf(attribute);
                                                int start = attr + attribute.Length;
                                                if (attr > 0)
                                                {
                                                    int end = dynamicline.IndexOf('"', attr + attribute.Length);
                                                    var value = dynamicline.Substring(start, end - start);
                                                    Console.WriteLine(value);
                                                    attri.Add(new AttributeData { Name = at, Value = value });
                                                }
                                            }
                                            
                                        }
                                        LineStrings.Add(new Lines { LineString = line, attribute = attri });
                                        LineNo.Add(counter.ToString());
                                    }
                                }
                                else if (type == "razor")
                                {
                                    if (line.Contains("@" + ta) && line.Contains(")"))
                                    {
                                        IList<AttributeData> attri = new List<AttributeData>();
                                        Console.WriteLine(counter.ToString() + ": " + line);
                                        foreach (var at in t.attributes)
                                        {
                                            string dynamicline = line;
                                            int startfinder = dynamicline.IndexOf("@" + ta);
                                            int endfinder = (dynamicline.IndexOf(")", startfinder));
                                            if (endfinder > 0)
                                            {
                                                dynamicline = dynamicline.Substring(startfinder, endfinder - startfinder);
                                                string attribute = at + "=" + '"';
                                                var attr = dynamicline.IndexOf(attribute);
                                                int start = attr + attribute.Length;
                                                if (attr > 0)
                                                {
                                                    int end = dynamicline.IndexOf('"', attr + attribute.Length);
                                                    var value = dynamicline.Substring(start, end - start);
                                                    Console.WriteLine(value);
                                                    attri.Add(new AttributeData { Name = at, Value = value });
                                                }
                                            }

                                        }
                                        LineStrings.Add(new Lines { LineString = line, attribute = attri });
                                        LineNo.Add(counter.ToString());
                                    }
                                }
                                else if (type == "both")
                                {
                                    if ((line.Contains("<" + ta)) || line.Contains("@" + ta))
                                    {
                                        IList<AttributeData> attri=new List<AttributeData>();
                                        Console.WriteLine(counter.ToString() + ": " + line);
                                        foreach (var at in t.attributes)
                                        {
                                            string dynamicline = line;
                                            int startfinder = dynamicline.IndexOf("<" + ta);
                                            if (startfinder < 0)
                                            {
                                                startfinder = dynamicline.IndexOf("@" + ta);
                                            }
                                            int endfinder = (dynamicline.IndexOf(ta + ">", startfinder) < 0 ? dynamicline.IndexOf(">", startfinder) : dynamicline.IndexOf( ta + ">", startfinder));
                                            if (endfinder < 0)
                                            {
                                                endfinder = (dynamicline.IndexOf(")", startfinder));
                                            }
                                            if (endfinder > 0)
                                            {
                                                dynamicline = dynamicline.Substring(startfinder, endfinder - startfinder);
                                                string attribute = at + "=" + '"';
                                                var attr = dynamicline.IndexOf(attribute);
                                                int start = attr + attribute.Length;
                                                if (attr > 0)
                                                {
                                                    int end = dynamicline.IndexOf('"', attr + attribute.Length);
                                                    var value = dynamicline.Substring(start, end - start);
                                                    Console.WriteLine(value);
                                                    attri.Add(new AttributeData { Name = at, Value = value });
                                                }
                                            }

                                        }
                                        LineStrings.Add(new Lines { LineString = line, attribute = attri });
                                        LineNo.Add(counter.ToString());
                                    }
                                }
                                else
                                {
                                    if (line.Contains("<" + ta) && (line.Contains("</" + ta + ">") || line.Contains("/>")))
                                    {
                                        IList<AttributeData> attri = new List<AttributeData>();
                                        Console.WriteLine(counter.ToString() + ": " + line);
                                        foreach (var at in t.attributes)
                                        {
                                            string dynamicline = line;
                                            int startfinder = dynamicline.IndexOf("<" + ta);
                                            int endfinder = (dynamicline.IndexOf("</" + ta + ">", startfinder) < 0 ? dynamicline.IndexOf("/>", startfinder) : dynamicline.IndexOf("</" + ta + ">", startfinder));
                                            if (endfinder > 0)
                                            {
                                                dynamicline = dynamicline.Substring(startfinder, endfinder - startfinder);
                                                string attribute = at + "=" + '"';
                                                var attr = dynamicline.IndexOf(attribute);
                                                int start = attr + attribute.Length;
                                                if (attr > 0)
                                                {
                                                    int end = dynamicline.IndexOf('"', attr + attribute.Length);
                                                    var value = dynamicline.Substring(start, end - start);
                                                    Console.WriteLine(value);
                                                    attri.Add(new AttributeData { Name = at, Value = value });
                                                }
                                            }

                                        }
                                        LineStrings.Add(new Lines { LineString = line, attribute = attri });
                                        LineNo.Add(counter.ToString());
                                    }
                                }
 
                                if (loop == 0)
                                {
                                  
                                        FullContent += counter.ToString() + ": " + line + "\n";
                                }
                                counter++;
                            }

                            file.Close();
                            // Scan links on this page
                            TagsDataIn.Add(new TagsData() { TagName = ta, TagColour = t.tagscolour[0], AttributeData = ad.ToList(), TagCount = parse.NOT, LineNumber = LineNo.ToList(), LineS = LineStrings.ToList() });
                            parse.NOT = 0;
                            loop++;

                        }
                        files.Add(new Files() { FileName = f.Name, Extension = f.Extension, FileLocation = f.FullName, TotalLine = parse.lineCount, SourceCode = FullContent, Tagsdata = TagsDataIn.ToList() });
                        FullContent = "";
                    }
                    ViewBag.TotalFiles = files.Count;
                    ViewData["Files"] = files;
                    Tags te = new Tags(type);
                    ViewBag.Tags = te.tags;
            }
                catch(Exception e)
                {
                    ViewBag.SystemError = e.Message;
                } 
            }
            else
            {
                ViewBag.Error = "Invalid Url";
            }
            return View();
        }
        public ActionResult ExportToExcel(string parsefolder = null, string type = "html", string name = "", string[] filter=null)
        {
            var products = new System.Data.DataTable("Parser");
            products.Columns.Add("File Name", typeof(string));
            products.Columns.Add("Location", typeof(string));
            products.Columns.Add("Extention", typeof(string));
            products.Columns.Add("Lines", typeof(string));
            products.Columns.Add("Tags", typeof(string));
            products.Columns.Add("Line", typeof(string));
            products.Columns.Add("Original MarkUp", typeof(string));
            Tags t1 = new Tags(type);
            foreach (var s in t1.attributes)
            {
                products.Columns.Add(s, typeof(string));
            }
            if (parsefolder != null || parsefolder != "")
            {
                ViewBag.Type = type;
                IList<Files> files = new List<Files>();
       /*   try
                {  */
                    DirectoryInfo directory = new DirectoryInfo(parsefolder);
                    var filess = directory.GetFiles().ToList();
                    if (name != "")
                    {
                       filess = directory.GetFiles().Where(s => s.Name == name).ToList();
                    }
                  
                    string FullContent = "";
                    foreach (var f in filess)
                    {


                        string html;

                        var fileStream = new FileStream(parsefolder + f.Name, FileMode.Open, FileAccess.Read);

                        using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                        {
                            html = streamReader.ReadToEnd();

                        }


                        HtmlParser parse = new HtmlParser(html, parsefolder + f.Name);

                        Tags t = new Tags(type);
                        var tags = t.tags;
                      
                        var loop = 0;
                        IList<TagsData> TagsDataIn = new List<TagsData>();
                        if (filter == null)
                        {
                            filter = tags;
                        }
                        foreach (var ta in filter)
                        {
                            string Line = "";
                            HtmlTag tag = new HtmlTag();
                            IList<AttributeData> ad = new List<AttributeData>(); ;
                            IList<string> LineStrings = new List<string>();
                            IList<string> LineNo = new List<string>();
                            while (parse.ParseNext(ta, out tag))
                            {


                                foreach (var attribute in t.attributes.ToList())
                                {

                                    string value;
                                    if (tag.Attributes.TryGetValue(attribute, out value))
                                    {
                                        ad.Add(new AttributeData() { Name = attribute, Value = value });
                                    }

                                }

                            }
                            parse.Reset();
                            int counter = 1;
                            string line;
                       
                            // Read the file and display it line by line.
                            System.IO.StreamReader file = new System.IO.StreamReader(parsefolder + f.Name);
                            while ((line = file.ReadLine()) != null)
                            {
                                /* Conditions */


                                if (type == "html")
                                {
                                    if (line.Contains("<" + ta) && (line.Contains("</" + ta + ">") || line.Contains("/>")))
                                    {
                                       
                                        products.Rows.Add(f.Name, f.FullName, f.Extension, parse.lineCount, ta, counter.ToString(), line);
                                      
                                        foreach (var at in t.attributes)
                                        {
                                                    string dynamicline = line;
                                                    int startfinder = dynamicline.IndexOf("<" + ta);
                                                    int endfinder = (dynamicline.IndexOf("</" + ta + ">", startfinder) < 0 ? dynamicline.IndexOf("/>", startfinder) : dynamicline.IndexOf("</" + ta + ">", startfinder));
                                                    if (endfinder > 0)
                                                    {
                                                        dynamicline = dynamicline.Substring(startfinder, endfinder - startfinder);
                                                        string attribute = at + "=" + '"';
                                                        var attr = dynamicline.IndexOf(attribute);
                                                        int start = attr + attribute.Length;
                                                        if (attr > 0)
                                                        {
                                                            int end = dynamicline.IndexOf('"', attr + attribute.Length);
                                                            var value = dynamicline.Substring(start, end - start);
                                                            Console.WriteLine(value);
                                                            products.Rows[products.Rows.Count - 1][at] = value;
                                                        }
                                                    }
                                        }
                                    }
                                }
                                else if (type == "razor")
                                {
                                    if (line.Contains("@" + ta) && line.Contains(")"))
                                    {
                                        products.Rows.Add(f.Name, f.FullName, f.Extension, parse.lineCount, ta, counter.ToString(), line);
                                        foreach (var at in t.attributes)
                                        {
                                            string dynamicline = line;
                                            int startfinder = dynamicline.IndexOf("@" + ta);
                                            int endfinder = (dynamicline.IndexOf(")", startfinder));
                                            if (endfinder > 0)
                                            {
                                                dynamicline = dynamicline.Substring(startfinder, endfinder - startfinder);
                                                string attribute = at + "=" + '"';
                                                var attr = dynamicline.IndexOf(attribute);
                                                int start = attr + attribute.Length;
                                                if (attr > 0)
                                                {
                                                    int end = dynamicline.IndexOf('"', attr + attribute.Length);
                                                    var value = dynamicline.Substring(start, end - start);
                                                    Console.WriteLine(value);
                                                    products.Rows[products.Rows.Count - 1][at] = value;
                                                    //  attri.Add(new AttributeData { Name = at, Value = value });
                                                }
                                            }

                                        }
                                       
                                    }
                                }
                                else if (type == "both")
                                {
                                    if ((line.Contains("<" + ta) ) || line.Contains("@" + ta))
                                    {
                                        products.Rows.Add(f.Name, f.FullName, f.Extension, parse.lineCount, ta, counter.ToString(), line);
                                        foreach (var at in t.attributes)
                                        {
                                            string dynamicline = line;
                                            int startfinder = dynamicline.IndexOf("<" + ta);
                                            if (startfinder < 0)
                                            {
                                                startfinder = dynamicline.IndexOf("@" + ta);
                                            }
                                            int endfinder = (dynamicline.IndexOf(ta + ">", startfinder) < 0 ? dynamicline.IndexOf(">", startfinder) : dynamicline.IndexOf(ta + ">", startfinder));
                                            if (endfinder < 0)
                                            {
                                                endfinder = (dynamicline.IndexOf(")", startfinder));
                                            }
                                            if (endfinder > 0)
                                            {
                                                dynamicline = dynamicline.Substring(startfinder, endfinder - startfinder);
                                                string attribute = at + "=" + '"';
                                                var attr = dynamicline.IndexOf(attribute);
                                                int start = attr + attribute.Length;
                                                if (attr > 0)
                                                {
                                                    int end = dynamicline.IndexOf('"', attr + attribute.Length);
                                                    var value = dynamicline.Substring(start, end - start);
                                                    Console.WriteLine(value);
                                                    products.Rows[products.Rows.Count - 1][at] = value;
                                                    // attri.Add(new AttributeData { Name = at, Value = value });
                                                }
                                            }

                                        }
                                       
                                    
                                    }
                                }
                                else
                                {
                                    if (line.Contains("<" + ta) && (line.Contains("</" + ta + ">") || line.Contains("/>")))
                                    {
                                        products.Rows.Add(f.Name, f.FullName, f.Extension, parse.lineCount, ta, counter.ToString(), line);
                                        foreach (var at in t.attributes)
                                        {
                                            string dynamicline = line;
                                            int startfinder = dynamicline.IndexOf("<" + ta);
                                            int endfinder = (dynamicline.IndexOf("</" + ta + ">", startfinder) < 0 ? dynamicline.IndexOf("/>", startfinder) : dynamicline.IndexOf("</" + ta + ">", startfinder));
                                            if (endfinder > 0)
                                            {
                                                dynamicline = dynamicline.Substring(startfinder, endfinder - startfinder);
                                                string attribute = at + "=" + '"';
                                                var attr = dynamicline.IndexOf(attribute);
                                                int start = attr + attribute.Length;
                                                if (attr > 0)
                                                {
                                                    int end = dynamicline.IndexOf('"', attr + attribute.Length);
                                                    var value = dynamicline.Substring(start, end - start);
                                                    Console.WriteLine(value);
                                                    products.Rows[products.Rows.Count - 1][at] = value;
                                                    //   attri.Add(new AttributeData { Name = at, Value = value });
                                                }
                                            }

                                        }
                                       
                                       
                                    }
                                }

                                if (loop == 0)
                                {

                                    FullContent += counter.ToString() + ": " + line + "\n";
                                }
                                counter++;
                            }

                        file.Close();
                            // Scan links on this page
                          

                        }
                       
                    }
                  
 /* }
                catch (Exception e)
                {
                    return RedirectToAction("ParseUrl", "home");
                } */
            } 
            else
            {
                return RedirectToAction("ParseUrl", "home");
            }

            var grid = new GridView();
           
                products.Rows.Add("Not Found");
     
            grid.DataSource = products;
            grid.DataBind();

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=ParserResult.xls");
            Response.ContentType = "application/ms-excel";

            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            grid.RenderControl(htw);

            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return View("MyView");
        }
        public ActionResult parse()
        {
          
            return View();
        }
        [HttpPost]
        public ActionResult Parse( string type = "html",HttpPostedFileBase[] files=null)
        {
            string foldername = "F-" + DateTime.Now.Millisecond;
            Directory.CreateDirectory(Server.MapPath("~/UploadedFiles/") + foldername);
                foreach (HttpPostedFileBase file in files)
                {
                    if (file.ContentLength > 0)
                    {
                        string _FileName = Path.GetDirectoryName(file.FileName) + "\\" + Path.GetFileName(file.FileName);
                        _FileName = _FileName.Replace("\\", "-");
                        string _path = Path.Combine(Server.MapPath("~/UploadedFiles/" + Convert.ToString(foldername)), _FileName);
                        file.SaveAs(_path);
                    }
                }
                string fullurl = Server.MapPath("~/UploadedFiles/") + foldername+"/";
                return this.RedirectToAction
    ("HtmlParser", new { parsefolder = fullurl, type = type });
         
        }
        public ActionResult FormFinder(string parsefolder = null, string type = "html")
        {
            ViewBag.parsefolder = parsefolder;
            try
            {
                 DirectoryInfo directory = new DirectoryInfo(parsefolder);
                    var filess = directory.GetFiles().ToList();
                    string line = "";
                    string FullContent = "";
                    IList<FormData> forms = new List<FormData>();
                    foreach (var f in filess)
                    {
                       
                        IList<FormTag> tags = new List<FormTag>();
                        int formlineno = 1;
                        System.IO.StreamReader file = new System.IO.StreamReader(parsefolder + f.Name);
                        int lineno = 1;
                        
                        string append = "";
                        bool form = false;
                       
                        while ((line = file.ReadLine()) != null)
                        {
                            bool final = false;
                            if (line.Contains("<form"))
                            {
                                append += line+Environment.NewLine;
                                form = true;
                                formlineno = lineno;
                                final = true;
                            }
                            if (line.Contains("</form>"))
                            {
                                append += line + Environment.NewLine;
                                form = false;
                             
                            }
                            if (line.Replace(" ", "").Contains("@using(Html.BeginForm"))
                            {
                                tags.Add(new FormTag { Tags = line, linenumber = formlineno });
                            }
                            if (form == true && final==false)
                            {
                                append += line + Environment.NewLine;
                            }
                            if(form==false)
                            {
                                tags.Add(new FormTag { Tags = append, linenumber = formlineno });
                                append = "";
        
                            }
                            lineno++;
                        }
                        forms.Add(new FormData { FileName = f.Name, Extension = f.Extension, FileLocation = f.FullName, TotalLine = lineno,Tagsdata=tags });
                    }
                    ViewData["Forms"] = forms;
            }
                catch(Exception e)
            {
                }
            return View();
        }

        public ActionResult ValidationFinder(string parsefolder = null, string type = "html")
        {
              ViewBag.parsefolder = parsefolder;
            try
            {
                 DirectoryInfo directory = new DirectoryInfo(parsefolder);
                    var filess = directory.GetFiles().ToList();
                    string line = "";
                    string FullContent = "";
                    IList<FormData> forms = new List<FormData>();
                    foreach (var f in filess)
                    {
                       
                        IList<FormTag> tags = new List<FormTag>();
                        int formlineno = 1;
                        System.IO.StreamReader file = new System.IO.StreamReader(parsefolder + f.Name);
                        int lineno = 1;
                        
                        string append = "";
                        bool form = false;
                        string EndTag = "";
                        while ((line = file.ReadLine()) != null)
                        {
                            bool final = false;

                            if (line.Contains("info-box-warning") || line.Contains("mp-text-error") || line.Contains("errortext") || line.Contains("errorText") || line.Contains("border:2px solid #cc0000; font-weight: bold;padding:10px;color:#cc0000;"))
                            {
                                
                                append += line+Environment.NewLine;
                                form = true;
                                formlineno = lineno;
                                final = true;
                                int start = line.IndexOf("<")+1;
                                int end = line.IndexOf(" ",start);
                                EndTag=line.Substring(start, end - start);
                            }
                            if (line.Contains("</" + EndTag + ">"))
                            {
                                EndTag = "";
                                append += line + Environment.NewLine;
                                form = false;
                               
                            }

                            if (form == true && final==false)
                            {
                                append += line + Environment.NewLine;
                            }
                            if(form==false)
                            {
                                tags.Add(new FormTag { Tags = append, linenumber = formlineno });
                                append = "";
        
                            }
                            lineno++;
                        }
                        forms.Add(new FormData { FileName = f.Name, Extension = f.Extension, FileLocation = f.FullName, TotalLine = lineno,Tagsdata=tags });
                    }
                    ViewData["Forms"] = forms;
            }
                catch(Exception e)
            {
                }
            return View();
        }

        public ActionResult FormExport(string parsefolder = null, string type = "html")
        {
            var products = new System.Data.DataTable("Parser");
            products.Columns.Add("Name", typeof(string));
            products.Columns.Add("Location", typeof(string));
            products.Columns.Add("Extention", typeof(string));
            products.Columns.Add("Tags", typeof(string));
            products.Columns.Add("Line", typeof(string));
            products.Columns.Add("Original MarkUp", typeof(string));

          
            ViewBag.parsefolder = parsefolder;
            try
            {
                DirectoryInfo directory = new DirectoryInfo(parsefolder);
                var filess = directory.GetFiles().ToList();
                string line = "";
                string FullContent = "";
                IList<FormData> forms = new List<FormData>();
                foreach (var f in filess)
                {

                    IList<FormTag> tags = new List<FormTag>();
                    int formlineno = 1;
                    System.IO.StreamReader file = new System.IO.StreamReader(parsefolder + f.Name);
                    int lineno = 1;

                    string append = "";
                    bool form = false;

                    while ((line = file.ReadLine()) != null)
                    {
                        bool final = false;
                        if (line.Contains("<form"))
                        {
                            append += line + Environment.NewLine;
                            form = true;
                            formlineno = lineno;
                            final = true;
                        }
                        if (line.Contains("</form>"))
                        {
                            append += line + Environment.NewLine;
                            form = false;

                        }
                        if (line.Replace(" ", "").Contains("@using(Html.BeginForm"))
                        {
                            if (line != "")
                            {
                                products.Rows.Add(f.Name, f.FullName, f.Extension, "Form", formlineno, line);
                            }
                           // tags.Add(new FormTag { Tags = line, linenumber = formlineno });
                        }
                        if (form == true && final == false)
                        {
                            append += line + Environment.NewLine;
                        }
                        if (form == false)
                        {
                            //tags.Add(new FormTag { Tags = append, linenumber = formlineno });
                            if (append != "")
                            {
                                products.Rows.Add(f.Name, f.FullName, f.Extension, "Form", formlineno, append);
                            }
                            append = "";

                        }

                       
                        lineno++;
                    }
                  //  forms.Add(new FormData { FileName = f.Name, Extension = f.Extension, FileLocation = f.FullName, TotalLine = lineno, Tagsdata = tags });
                }
              
            }
            catch (Exception e)
            {
            }
             var grid = new GridView();
           
                products.Rows.Add("Not Found");
     
            grid.DataSource = products;
            grid.DataBind();

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=FormParserResult.xls");
            Response.ContentType = "application/ms-excel";

            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            grid.RenderControl(htw);

            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return View("MyView");
        }
        public ActionResult ValidationExport(string parsefolder = null, string type = "html")
        {
            var products = new System.Data.DataTable("Parser");
            products.Columns.Add("Name", typeof(string));
            products.Columns.Add("Location", typeof(string));
            products.Columns.Add("Extention", typeof(string));
            products.Columns.Add("Line", typeof(string));
            products.Columns.Add("Original MarkUp", typeof(string));


            ViewBag.parsefolder = parsefolder;
            try
            {
                DirectoryInfo directory = new DirectoryInfo(parsefolder);
                var filess = directory.GetFiles().ToList();
                string line = "";
                string FullContent = "";
                IList<FormData> forms = new List<FormData>();
                foreach (var f in filess)
                {

                    IList<FormTag> tags = new List<FormTag>();
                    int formlineno = 1;
                    System.IO.StreamReader file = new System.IO.StreamReader(parsefolder + f.Name);
                    int lineno = 1;

                    string append = "";
                    bool form = false;
                    string EndTag = "";
                    while ((line = file.ReadLine()) != null)
                    {
                        bool final = false;

                        if (line.Contains("info-box-warning") || line.Contains("mp-text-error") || line.Contains("errortext") || line.Contains("errorText") || line.Contains("border:2px solid #cc0000; font-weight: bold;padding:10px;color:#cc0000;"))
                        {

                            append += line + Environment.NewLine;
                            form = true;
                            formlineno = lineno;
                            final = true;
                            int start = line.IndexOf("<") + 1;
                            int end = line.IndexOf(" ", start);
                            EndTag = line.Substring(start, end - start);
                        }
                        if (line.Contains("</" + EndTag + ">"))
                        {
                            EndTag = "";
                            append += line + Environment.NewLine;
                            form = false;

                        }

                        if (form == true && final == false)
                        {
                            append += line + Environment.NewLine;
                        }
                        if (form == false)
                        {
                           // tags.Add(new FormTag { Tags = append, linenumber = formlineno });
                            if (append != "")
                            {
                                products.Rows.Add(f.Name, f.FullName, f.Extension,formlineno, append);
                            }
                            append = "";

                        }
                        lineno++;
                    }
                   // forms.Add(new FormData { FileName = f.Name, Extension = f.Extension, FileLocation = f.FullName, TotalLine = lineno, Tagsdata = tags });
                }
                ViewData["Forms"] = forms;
            }
            catch (Exception e)
            {
            }
            var grid = new GridView();

            products.Rows.Add("Not Found");

            grid.DataSource = products;
            grid.DataBind();

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=ValidationParserResult.xls");
            Response.ContentType = "application/ms-excel";

            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            grid.RenderControl(htw);

            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return View("MyView");
        }
        public ActionResult Testing()
        {
            ViewBag.Error = "Please enter Password!!";
            return View();
        }
        public ActionResult FormParser()
        {
            return View();
        }
        [HttpPost]
        public ActionResult FormParser(string type = "html", HttpPostedFileBase[] files = null)
        {
            string foldername = "F-" + DateTime.Now.Millisecond;
            Directory.CreateDirectory(Server.MapPath("~/UploadedFiles/") + foldername);
            foreach (HttpPostedFileBase file in files)
            {
                if (file.ContentLength > 0)
                {
                    string _FileName = Path.GetDirectoryName(file.FileName) + "\\" + Path.GetFileName(file.FileName);
                    _FileName = _FileName.Replace("\\", "-");
                    string _path = Path.Combine(Server.MapPath("~/UploadedFiles/" + Convert.ToString(foldername)), _FileName);
                    file.SaveAs(_path);
                }
            }
            string fullurl = Server.MapPath("~/UploadedFiles/") + foldername + "/";
            return this.RedirectToAction
("FormFinder", new { parsefolder = fullurl, type = type });

        }
    }
    public class Files
    {
        public string FileName { get;set;}
        public string FileLocation { get; set; }
        public string Extension { get;set;}
        public int TotalLine { get; set; }
        public virtual IList<TagsData> Tagsdata { get; set; }
        public string SourceCode { get; set; }
    }
    public class TagsData
    {
        public string TagName { get; set; }
        public virtual IList<AttributeData> AttributeData { get; set; }
        public string TagColour { get; set; }
        public int TagCount { get; set; }
        public IList<string> LineNumber { get; set; }
        public IList<Lines> LineS { get; set; }
    }
    public class Lines
    {
        public string LineString { get; set; }
        public IList<AttributeData> attribute { get; set; }
    }
    public class AttributeData
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
    public class FormData
    {
        public string FileName { get; set; }
        public string FileLocation { get; set; }
        public string Extension { get; set; }
        public int TotalLine { get; set; }
        public virtual IList<FormTag> Tagsdata { get; set; }
        public string SourceCode { get; set; }
    }
    public class FormTag
    {
        public string Tags { get; set; }
        public int linenumber { get; set; }
    }
}
