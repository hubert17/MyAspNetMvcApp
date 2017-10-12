// *** Snippets ***
// public byte[] MyFile { get; set; }
// <form action="/controller/create" method="post" enctype="multipart/form-data">
// <input type="file" name="FileUpload" />
// [HttpPost]
// public ActionResult Create(MyClass myObject, HttpPostedFileBase FileUpload)
// myObject.MyFile = FileUpload.ToFileByteArrayy();

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

public static class FileUploadExtension
{
    public const string FOLDER = "UploadedFiles";

    public static byte[] ToFileByteArray(this HttpPostedFileBase File)
    {
        try
        {
            // Convert Uploaded File to byte array
            byte[] file = new byte[File.ContentLength];
            File.InputStream.Read(file, 0, File.ContentLength);
            return file;
        }
        catch { }

        return null;
    }

    public static string SaveToFolder(this HttpPostedFileBase File, string strFileName = "", string strFolder = "")
    {
        try
        {
            var file = ToFileByteArray(File);

            string folder = string.IsNullOrEmpty(strFolder) ? HttpContext.Current.Server.MapPath("~/" + FOLDER) : HttpContext.Current.Server.MapPath("~/" + strFolder);
            string filename = string.IsNullOrEmpty(strFileName) ? Path.GetFileNameWithoutExtension(File.FileName) : strFileName;
            string filenameExt = filename + "_" + GenerateUniqueChars() + Path.GetExtension(File.FileName);
            string path = Path.Combine(folder, filenameExt);

            System.IO.Directory.CreateDirectory(folder);
            System.IO.File.WriteAllBytes(path, file);

            return filenameExt;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    private static string GenerateUniqueChars()
    {
        char[] padding = { '=' };
        return Convert.ToBase64String(Guid.NewGuid().ToByteArray()).TrimEnd(padding).Replace('+', '-').Replace('/', '_'); ;
    }

}
