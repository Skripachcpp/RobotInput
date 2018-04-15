using System;
using System.Net;

namespace WorkingTools.Classes.FtpProviderPart
{
    public enum FtpRequestType
    {
        /// <summary>
        /// ѕредставл€ет метод протокола FTP APPE, который используетс€ дл€ присоединени€ файла к существующему файлу на FTP-сервере.
        /// </summary>
        DownloadFile,
        /// <summary>
        /// ѕредставл€ет метод протокола FTP DELE, который используетс€ дл€ удалени€ файла на FTP-сервере.
        /// </summary>
        ListDirectory,
        /// <summary>
        /// ѕредставл€ет метод протокола FTP RETR, который используетс€ дл€ загрузки файла с FTP-сервера.
        /// </summary>
        UploadFile,
        /// <summary>
        /// ѕредставл€ет метод протокола FTP MDTM, который используетс€ дл€ получени€ штампа даты и времени из файла на FTP-сервере.
        /// </summary>
        DeleteFile,
        /// <summary>
        /// ѕредставл€ет метод протокола FTP SIZE, который используетс€ дл€ получени€ размера файла на FTP-сервере.
        /// </summary>
        AppendFile,
        /// <summary>
        /// ѕредставл€ет метод протокола FTP NLIST, который возвращает краткий список файлов на FTP-сервере.
        /// </summary>
        GetFileSize,
        /// <summary>
        /// ѕредставл€ет метод протокола FTP LIST, который возвращает подробный список файлов на FTP-сервере.
        /// </summary>
        UploadFileWithUniqueName,
        /// <summary>
        /// ѕредставл€ет метод протокола FTP MKD, который создает каталог на FTP-сервере.
        /// </summary>
        MakeDirectory,
        /// <summary>
        /// ѕредставл€ет метод протокола FTP PWD, который отображает им€ текущего рабочего каталога.
        /// </summary>
        RemoveDirectory,
        /// <summary>
        /// ѕредставл€ет метод протокола FTP RMD, который удал€ет каталог.
        /// </summary>
        ListDirectoryDetails,
        /// <summary>
        /// ѕредставл€ет метод протокола FTP RENAME, который переименовывает каталог.
        /// </summary>
        GetDateTimestamp,
        /// <summary>
        /// ѕредставл€ет метод протокола FTP STOR, который выгружает файл на FTP-сервер.
        /// </summary>
        PrintWorkingDirectory,
        /// <summary>
        /// ѕредставл€ет метод протокола FTP STOU, который выгружает файл с уникальным именем на FTP-сервер.
        /// </summary>
        Rename,
    }

    public static class FtpRequestTypeExtention
    {
        public static string ToFtpWebRequestMethod(this FtpRequestType ftpRequestType)
        {
            switch (ftpRequestType)
            {
                case FtpRequestType.DownloadFile:
                    return WebRequestMethods.Ftp.DownloadFile;
                case FtpRequestType.ListDirectory:
                    return WebRequestMethods.Ftp.ListDirectory;
                case FtpRequestType.UploadFile:
                    return WebRequestMethods.Ftp.UploadFile;
                case FtpRequestType.DeleteFile:
                    return WebRequestMethods.Ftp.DeleteFile;
                case FtpRequestType.AppendFile:
                    return WebRequestMethods.Ftp.AppendFile;
                case FtpRequestType.GetFileSize:
                    return WebRequestMethods.Ftp.GetFileSize;
                case FtpRequestType.UploadFileWithUniqueName:
                    return WebRequestMethods.Ftp.UploadFileWithUniqueName;
                case FtpRequestType.MakeDirectory:
                    return WebRequestMethods.Ftp.MakeDirectory;
                case FtpRequestType.RemoveDirectory:
                    return WebRequestMethods.Ftp.RemoveDirectory;
                case FtpRequestType.ListDirectoryDetails:
                    return WebRequestMethods.Ftp.ListDirectoryDetails;
                case FtpRequestType.GetDateTimestamp:
                    return WebRequestMethods.Ftp.GetDateTimestamp;
                case FtpRequestType.PrintWorkingDirectory:
                    return WebRequestMethods.Ftp.PrintWorkingDirectory;
                case FtpRequestType.Rename:
                    return WebRequestMethods.Ftp.Rename;
                default:
                    throw new ArgumentOutOfRangeException("ftpRequestType", "значение не предусмотрено");
            }
        }
    }
}